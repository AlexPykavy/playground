locals {
  api_url  = "http://localhost:8000/"
}

provider "azurerm" {
  features {}
}

data "azuread_client_config" "current" {}

resource "random_id" "main" {
  byte_length = 8
}

resource "random_uuid" "access_as_user" {}

resource "azuread_application" "main" {
  display_name            = lower(random_id.main.hex)
  group_membership_claims = ["SecurityGroup"]
  identifier_uris         = ["api://${lower(random_id.main.hex)}"]
  owners                  = [data.azuread_client_config.current.object_id]

  api {
    oauth2_permission_scope {
      admin_consent_description  = "Access as an admin."
      admin_consent_display_name = "Access as an admin."
      id                         = random_uuid.access_as_user.id
      type                       = "User"
      user_consent_description   = "Access as a user."
      user_consent_display_name  = "Access as a user."
      value                      = "access_as_user"
    }
  }

  web {
    homepage_url = local.api_url

    implicit_grant {
      access_token_issuance_enabled = true
      id_token_issuance_enabled     = true
    }
  }

  single_page_application {
    redirect_uris = ["${local.api_url}swagger/oauth2-redirect.html"]
  }
}

output "AzureAd__Audience" {
  value = "api://${lower(random_id.main.hex)}"
}

output "AzureAd__ClientId" {
  value = azuread_application.main.application_id
}

output "AzureAd__Domain" {
  value = azuread_application.main.publisher_domain
}

output "AzureAd__Scopes" {
  value = "access_as_user"
}

output "AzureAd__TenantId" {
  value = data.azuread_client_config.current.tenant_id
}
