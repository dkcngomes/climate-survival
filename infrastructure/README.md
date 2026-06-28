# ── AWS Infrastructure for ClimateBuy ──

This deploys the backend (ECS Fargate) and frontend (S3 + CloudFront).

## Prerequisites

- AWS CLI configured with appropriate credentials
- Terraform v1.5+
- Docker
- A domain name (optional, for CloudFront)

## Usage

```bash
cd infrastructure
terraform init
terraform plan -var="domain_name=climatebuy.yourdomain.com"
terraform apply -var="domain_name=climatebuy.yourdomain.com"
```

## Architecture

```
CloudFront ──► S3 (Frontend static files)
         └──► ALB ──► ECS Fargate (Backend API)
```

