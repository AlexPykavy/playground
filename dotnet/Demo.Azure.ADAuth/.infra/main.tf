locals {
  api_url   = "http://localhost:8000/"
  app_scope = "access_as_user"
  app_role  = "WeatherForecast.Reader"
}

provider "azurerm" {
  features {}

  skip_provider_registration = true
}

data "azuread_client_config" "current" {}

resource "random_id" "main" {
  byte_length = 8
}

resource "random_uuid" "access_as_user_scope" {}
resource "random_uuid" "app_role" {}

resource "azuread_application_registration" "main" {
  display_name            = lower(random_id.main.hex)
  group_membership_claims = ["SecurityGroup"]

  homepage_url                           = local.api_url
  implicit_access_token_issuance_enabled = true
  implicit_id_token_issuance_enabled     = true
}

resource "azuread_application_owner" "main" {
  application_id  = azuread_application_registration.main.id
  owner_object_id = data.azuread_client_config.current.object_id
}

resource "azuread_application_identifier_uri" "main" {
  application_id = azuread_application_registration.main.id
  identifier_uri = "api://${azuread_application_registration.main.client_id}"
}

resource "azuread_application_permission_scope" "main" {
  application_id = azuread_application_registration.main.id
  scope_id       = random_uuid.access_as_user_scope.id
  value          = local.app_scope

  admin_consent_description  = "Access as an admin."
  admin_consent_display_name = "Access as an admin."
  user_consent_description   = "Access as a user."
  user_consent_display_name  = "Access as a user."
}

resource "azuread_application_app_role" "main" {
  application_id = azuread_application_registration.main.id
  role_id        = random_uuid.app_role.id

  allowed_member_types = ["Application", "User"]
  description          = "Used to check if application can access ${local.app_role}"
  display_name         = local.app_role
  value                = local.app_role
}

resource "azuread_application_redirect_uris" "main_spa" {
  application_id = azuread_application_registration.main.id
  type           = "SPA"

  redirect_uris = [
    "${local.api_url}swagger/oauth2-redirect.html"
  ]
}

resource "azuread_service_principal" "main" {
  client_id = azuread_application_registration.main.client_id
  owners    = [data.azuread_client_config.current.object_id]
}

resource "azuread_app_role_assignment" "current_sp_to_main" {
  app_role_id         = random_uuid.app_role.id
  principal_object_id = data.azuread_client_config.current.object_id
  resource_object_id  = azuread_service_principal.main.object_id
}

output "AzureAd__Audience" {
  value = azuread_application_registration.main.client_id
}

output "AzureAd__ClientId" {
  value = azuread_application_registration.main.client_id
}

output "AzureAd__Domain" {
  value = azuread_application_registration.main.publisher_domain
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
