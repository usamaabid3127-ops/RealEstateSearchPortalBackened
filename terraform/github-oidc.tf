resource "aws_iam_openid_connect_provider" "github" {
  url             = "https://token.actions.githubusercontent.com"
  client_id_list  = ["sts.amazonaws.com"]
  thumbprint_list = ["6938fd4d98bab03faadb97b34396831e3780aea1", "1c58a3a8518e8759bf075b76b750d4f2df264fcd"]
}

locals {
  github_oidc_arn = aws_iam_openid_connect_provider.github.arn
}
locals {
  github_owner  = "usamaabid3127-ops"
  backend_repo  = "RealEstateSearchPortalBackened"
  frontend_repo = "RealEstateSearchPortalFrontend"
  branch        = "master"
}

# ---------- Backend pipeline role ----------
data "aws_iam_policy_document" "gha_backend_trust" {
  statement {
    actions = ["sts:AssumeRoleWithWebIdentity"]
    principals {
      type        = "Federated"
      identifiers = [local.github_oidc_arn]
    }
    condition {
      test     = "StringEquals"
      variable = "token.actions.githubusercontent.com:aud"
      values   = ["sts.amazonaws.com"]
    }
    condition {
      test     = "StringEquals"
      variable = "token.actions.githubusercontent.com:sub"
      values   = ["repo:${local.github_owner}/${local.backend_repo}:ref:refs/heads/${local.branch}", "repo:${local.github_owner}@${local.github_owner_id}/${local.backend_repo}@${local.backend_repo_id}:ref:refs/heads/${local.branch}"]
    }
  }
}

resource "aws_iam_role" "gha_backend" {
  name               = "re-gha-backend"
  assume_role_policy = data.aws_iam_policy_document.gha_backend_trust.json
}

data "aws_iam_policy_document" "gha_backend" {
  statement {
    actions   = ["ecr:GetAuthorizationToken"]
    resources = ["*"]
  }
  statement {
    actions = [
      "ecr:BatchCheckLayerAvailability",
      "ecr:InitiateLayerUpload",
      "ecr:UploadLayerPart",
      "ecr:CompleteLayerUpload",
      "ecr:PutImage",
      "ecr:BatchGetImage",
      "ecr:GetDownloadUrlForLayer",
    ]
    resources = [aws_ecr_repository.backend.arn]
  }
  statement {
    actions = ["ssm:SendCommand"]
    resources = [
      "arn:aws:ec2:${var.aws_region}:${data.aws_caller_identity.current.account_id}:instance/${aws_instance.backend.id}",
      "arn:aws:ssm:${var.aws_region}::document/AWS-RunShellScript",
    ]
  }
  statement {
    actions   = ["ssm:GetCommandInvocation", "ssm:ListCommandInvocations", "ssm:ListCommands", "ec2:DescribeInstances"]
    resources = ["*"]
  }
}

resource "aws_iam_role_policy" "gha_backend" {
  name   = "deploy-backend"
  role   = aws_iam_role.gha_backend.id
  policy = data.aws_iam_policy_document.gha_backend.json
}

# ---------- Frontend pipeline role ----------
data "aws_iam_policy_document" "gha_frontend_trust" {
  statement {
    actions = ["sts:AssumeRoleWithWebIdentity"]
    principals {
      type        = "Federated"
      identifiers = [local.github_oidc_arn]
    }
    condition {
      test     = "StringEquals"
      variable = "token.actions.githubusercontent.com:aud"
      values   = ["sts.amazonaws.com"]
    }
    condition {
      test     = "StringEquals"
      variable = "token.actions.githubusercontent.com:sub"
      values   = ["repo:${local.github_owner}/${local.frontend_repo}:ref:refs/heads/${local.branch}", "repo:${local.github_owner}@${local.github_owner_id}/${local.frontend_repo}@${local.frontend_repo_id}:ref:refs/heads/${local.branch}"]
    }
  }
}

resource "aws_iam_role" "gha_frontend" {
  name               = "re-gha-frontend"
  assume_role_policy = data.aws_iam_policy_document.gha_frontend_trust.json
}

data "aws_iam_policy_document" "gha_frontend" {
  statement {
    actions   = ["s3:ListBucket"]
    resources = [aws_s3_bucket.frontend.arn]
  }
  statement {
    actions   = ["s3:PutObject", "s3:DeleteObject"]
    resources = ["${aws_s3_bucket.frontend.arn}/*"]
  }
  statement {
    actions   = ["cloudfront:CreateInvalidation"]
    resources = [aws_cloudfront_distribution.main.arn]
  }
}

resource "aws_iam_role_policy" "gha_frontend" {
  name   = "deploy-frontend"
  role   = aws_iam_role.gha_frontend.id
  policy = data.aws_iam_policy_document.gha_frontend.json
}

output "gha_backend_role_arn" {
  value = aws_iam_role.gha_backend.arn
}

output "gha_frontend_role_arn" {
  value = aws_iam_role.gha_frontend.arn
}
locals {
  github_owner_id  = "307182016"
  backend_repo_id  = "1451474457"
  frontend_repo_id = "1451473318"
}
