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

output "ApplicationInsights__ConnectionString" {
  value = nonsensitive(azurerm_application_insights.main.connection_string)
}
