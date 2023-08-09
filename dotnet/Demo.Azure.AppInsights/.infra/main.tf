provider "azurerm" {
  features {}
}

data "azuread_client_config" "current" {}

resource "random_id" "main" {
  byte_length = 8
}

resource "azurerm_resource_group" "main" {
  name     = "${lower(random_id.main.hex)}-rg"
  location = "West Europe"
}

resource "azurerm_log_analytics_workspace" "main" {
  name                = "${lower(random_id.main.hex)}-law"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_application_insights" "main" {
  name                = "${lower(random_id.main.hex)}-ai"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  workspace_id        = azurerm_log_analytics_workspace.main.id
  application_type    = "web"
}

resource "azuread_application" "main" {
  display_name            = "${lower(random_id.main.hex)}-app"
  group_membership_claims = ["SecurityGroup"]
  identifier_uris         = ["api://${lower(random_id.main.hex)}-app"]
  owners                  = [data.azuread_client_config.current.object_id]
}

resource "azuread_service_principal" "main" {
  application_id = azuread_application.main.application_id
  owners         = [data.azuread_client_config.current.object_id]
}

resource "azuread_service_principal_password" "main" {
  service_principal_id = azuread_service_principal.main.object_id
}

resource "azurerm_role_assignment" "main_sp_to_app_insights" {
  scope                = azurerm_application_insights.main.id
  role_definition_name = "Reader"
  principal_id         = azuread_service_principal.main.object_id
}

resource "azurerm_role_assignment" "main_sp_to_law" {
  scope                = azurerm_log_analytics_workspace.main.id
  role_definition_name = "Reader"
  principal_id         = azuread_service_principal.main.object_id
}

output "ApplicationInsights__ConnectionString" {
  value = nonsensitive(azurerm_application_insights.main.connection_string)
}

output "ApplicationInsights__ResourceId" {
  value = azurerm_application_insights.main.id
}

output "AzureAd__ClientId" {
  value = azuread_application.main.application_id
}

output "AzureAd__ClientSecret" {
  value = nonsensitive(azuread_service_principal_password.main.value)
}

output "AzureAd__TenantId" {
  value = data.azuread_client_config.current.tenant_id
}
