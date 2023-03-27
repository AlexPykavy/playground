# .NET projects

Prerequisites:
- Docker and Docker Compose
- Terraform

To build and run the project `<PROJECT>` you need to perform the following actions:
1. Open `<PROJECT>/.infra` directory.
1. Run `terraform init`, `terraform apply` and `terraform output > .env`.
1. Run `docker compose up <PROJECT_IN_LOWERCASE> --build`.