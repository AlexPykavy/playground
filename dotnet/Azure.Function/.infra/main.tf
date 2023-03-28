locals {
  app_name = "azurefunc-local-test"
}

provider "azurerm" {
  features {}
}

resource "random_id" "storage_account" {
  byte_length = 8
}

data "http" "myip" {
  url = "http://ipv4.icanhazip.com"
}

resource "azurerm_resource_group" "main" {
  name     = local.app_name
  location = "West Europe"
}

resource "azurerm_storage_account" "main" {
  name                     = "${lower(random_id.storage_account.hex)}testsa"
  resource_group_name      = azurerm_resource_group.main.name
  location                 = azurerm_resource_group.main.location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  network_rules {
    default_action = "Deny"
    ip_rules       = [chomp(data.http.myip.response_body)]
  }
}

resource "azurerm_storage_queue" "main" {
  name                 = "weather-anomalies"
  storage_account_name = azurerm_storage_account.main.name
}

output "AzureWebJobsStorage" {
  value = nonsensitive(azurerm_storage_account.main.primary_connection_string)
}
