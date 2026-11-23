data "aws_ssm_parameter" "al2023" {
  name = "/aws/service/ami-amazon-linux-latest/al2023-ami-kernel-default-x86_64"
}

resource "aws_ecr_repository" "backend" {
  name         = "real-estate-backend"
  force_delete = true

  image_scanning_configuration {
    scan_on_push = true
  }
}

resource "aws_ecr_lifecycle_policy" "backend" {
  repository = aws_ecr_repository.backend.name
  policy = jsonencode({
    rules = [{
      rulePriority = 1
      description  = "Keep only the last 3 images"
      selection = {
        tagStatus   = "any"
        countType   = "imageCountMoreThan"
        countNumber = 3
      }
      action = { type = "expire" }
    }]
  })
}

resource "aws_iam_role" "backend" {
  name = "re-backend-role"
  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Effect    = "Allow"
      Action    = "sts:AssumeRole"
      Principal = { Service = "ec2.amazonaws.com" }
    }]
  })
}

resource "aws_iam_role_policy_attachment" "ssm_core" {
  role       = aws_iam_role.backend.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore"
}

resource "aws_iam_role_policy_attachment" "ecr_read" {
  role       = aws_iam_role.backend.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonEC2ContainerRegistryReadOnly"
}

resource "aws_iam_role_policy" "read_params" {
  name = "read-real-estate-params"
  role = aws_iam_role.backend.id
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Effect   = "Allow"
      Action   = ["ssm:GetParameter", "ssm:GetParameters"]
      Resource = "arn:aws:ssm:${var.aws_region}:${data.aws_caller_identity.current.account_id}:parameter/real-estate/*"
    }]
  })
}

resource "aws_iam_instance_profile" "backend" {
  name = "re-backend-profile"
  role = aws_iam_role.backend.name
}

resource "aws_vpc_security_group_ingress_rule" "backend_http" {
  security_group_id = aws_security_group.backend.id
  description       = "Backend API (only reachable from CloudFront)"
  from_port         = 8080
  to_port           = 8080
  ip_protocol       = "tcp"
  prefix_list_id    = data.aws_ec2_managed_prefix_list.cloudfront.id
}

resource "aws_instance" "backend" {
  ami                         = data.aws_ssm_parameter.al2023.value
  instance_type               = "t3.micro"
  subnet_id                   = data.aws_subnets.default.ids[0]
  vpc_security_group_ids      = [aws_security_group.backend.id]
  iam_instance_profile        = aws_iam_instance_profile.backend.name
  associate_public_ip_address = true
  user_data_replace_on_change = true

  metadata_options {
    http_tokens = "required"
  }

  tags = {
    Name = "re-backend"
  }

  user_data = <<EOT
#!/bin/bash
dnf install -y docker
systemctl enable --now docker
cat > /opt/deploy.sh <<'DEPLOY'
#!/bin/bash
set -e
REGION="${var.aws_region}"
REGISTRY="${data.aws_caller_identity.current.account_id}.dkr.ecr.${var.aws_region}.amazonaws.com"
DB_HOST="${aws_db_instance.main.address}"
IMAGE="$REGISTRY/real-estate-backend:latest"
aws ecr get-login-password --region "$REGION" | docker login --username AWS --password-stdin "$REGISTRY"
docker pull "$IMAGE"
DB_PASS=$(aws ssm get-parameter --name /real-estate/db-password --with-decryption --query Parameter.Value --output text --region "$REGION")
JWT=$(aws ssm get-parameter --name /real-estate/jwt-secret --with-decryption --query Parameter.Value --output text --region "$REGION")
docker rm -f backend 2>/dev/null || true
docker run -d --name backend --restart unless-stopped -p 8080:8080 \
  -e "ConnectionStrings__DefaultConnection=Host=$DB_HOST;Port=5432;Database=realestatedb;Username=dbadmin;Password=$DB_PASS;SSL Mode=Require;Trust Server Certificate=true" \
  -e "Jwt__Secret=$JWT" "$IMAGE"
DEPLOY
chmod +x /opt/deploy.sh
EOT
}

resource "aws_eip" "backend" {
  domain   = "vpc"
  instance = aws_instance.backend.id
}

output "ecr_repository_url" {
  value = aws_ecr_repository.backend.repository_url
}

output "backend_instance_id" {
  value = aws_instance.backend.id
}

output "backend_public_dns" {
  value = aws_eip.backend.public_dns
}
data "aws_ec2_managed_prefix_list" "cloudfront" {
  name = "com.amazonaws.global.cloudfront.origin-facing"
}
