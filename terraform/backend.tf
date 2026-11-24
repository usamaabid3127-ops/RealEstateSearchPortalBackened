terraform {
  backend "s3" {
    bucket       = "re-tfstate-656446902115"
    key          = "real-estate/terraform.tfstate"
    region       = "us-east-1"
    encrypt      = true
    use_lockfile = true
  }
}