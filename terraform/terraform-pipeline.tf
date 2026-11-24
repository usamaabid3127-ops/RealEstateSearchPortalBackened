data "aws_iam_policy_document" "gha_terraform_trust" {
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
      values = [
        "repo:${local.github_owner}/${local.backend_repo}:ref:refs/heads/${local.branch}",
        "repo:${local.github_owner}@${local.github_owner_id}/${local.backend_repo}@${local.backend_repo_id}:ref:refs/heads/${local.branch}",
      ]
    }
  }
}

resource "aws_iam_role" "gha_terraform" {
  name               = "re-gha-terraform"
  assume_role_policy = data.aws_iam_policy_document.gha_terraform_trust.json
}

resource "aws_iam_role_policy_attachment" "gha_terraform_admin" {
  role       = aws_iam_role.gha_terraform.name
  policy_arn = "arn:aws:iam::aws:policy/AdministratorAccess"
}

output "gha_terraform_role_arn" {
  value = aws_iam_role.gha_terraform.arn
}