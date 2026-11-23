resource "random_password" "db" {
  length           = 24
  special          = true
  override_special = "!#%^*-_"
}

resource "random_password" "jwt" {
  length  = 48
  special = false
}

resource "aws_db_subnet_group" "main" {
  name       = "re-db-subnets"
  subnet_ids = data.aws_subnets.default.ids
}

resource "aws_db_instance" "main" {
  identifier              = "re-postgres"
  engine                  = "postgres"
  engine_version          = "16"
  instance_class          = "db.t3.micro"
  allocated_storage       = 20
  storage_type            = "gp2"
  db_name                 = "realestatedb"
  username                = "dbadmin"
  password                = random_password.db.result
  db_subnet_group_name    = aws_db_subnet_group.main.name
  vpc_security_group_ids  = [aws_security_group.database.id]
  publicly_accessible     = false
  multi_az                = false
  backup_retention_period = 1
  skip_final_snapshot     = true
  deletion_protection     = false
  apply_immediately       = true
}

resource "aws_ssm_parameter" "db_password" {
  name  = "/real-estate/db-password"
  type  = "SecureString"
  value = random_password.db.result
}

resource "aws_ssm_parameter" "jwt_secret" {
  name  = "/real-estate/jwt-secret"
  type  = "SecureString"
  value = random_password.jwt.result
}

output "db_endpoint" {
  value = aws_db_instance.main.address
}