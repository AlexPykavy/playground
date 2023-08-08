locals {
  api_url   = "http://localhost:8000/"
  app_scope = "access_as_user"
  app_role  = "WeatherForecast.Reader"
}

provider "azurerm" {
  features {}
}

data "azuread_client_config" "current" {}

resource "random_id" "main" {
  byte_length = 8
}

resource "random_uuid" "access_as_user_scope" {}
resource "random_uuid" "app_role" {}

resource "azuread_application" "main" {
  display_name            = lower(random_id.main.hex)
  group_membership_claims = ["SecurityGroup"]
  identifier_uris         = ["api://${lower(random_id.main.hex)}"]
  owners                  = [data.azuread_client_config.current.object_id]

  api {
    oauth2_permission_scope {
      admin_consent_description  = "Access as an admin."
      admin_consent_display_name = "Access as an admin."
      id                         = random_uuid.access_as_user_scope.id
      type                       = "User"
      user_consent_description   = "Access as a user."
      user_consent_display_name  = "Access as a user."
      value                      = local.app_scope
    }
  }

  app_role {
    allowed_member_types = ["Application", "User"]
    description          = "Used to check if application can access ${local.app_role}"
    display_name         = local.app_role
    enabled              = true
    id                   = random_uuid.app_role.id
    value                = local.app_role
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

resource "azuread_service_principal" "main" {
  application_id = azuread_application.main.application_id
  owners         = [data.azuread_client_config.current.object_id]
}

resource "azuread_app_role_assignment" "current_sp_to_main" {
  app_role_id         = random_uuid.app_role.id
  principal_object_id = data.azuread_client_config.current.object_id
  resource_object_id  = azuread_service_principal.main.object_id
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

output "AzureAd__Role" {
  value = local.app_role
}

output "AzureAd__Scopes" {
  value = local.app_scope
}

output "AzureAd__TenantId" {
  value = data.azuread_client_config.current.tenant_id
}
