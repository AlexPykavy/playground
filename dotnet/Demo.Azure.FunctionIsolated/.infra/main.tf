variable "use_managed_identity" {
  default = true
}

provider "azurerm" {
  features {}
  storage_use_azuread = true
}

data "azuread_client_config" "current" {}

resource "random_id" "main" {
  byte_length = 8
}

data "http" "myip" {
  url = "http://ipv4.icanhazip.com"
}

resource "azurerm_resource_group" "main" {
  name     = "${lower(random_id.main.hex)}-rg"
  location = "West Europe"
}

resource "azurerm_storage_account" "main" {
  name                            = "${lower(random_id.main.hex)}sa"
  resource_group_name             = azurerm_resource_group.main.name
  location                        = azurerm_resource_group.main.location
  account_tier                    = "Standard"
  account_replication_type        = "LRS"
  default_to_oauth_authentication = var.use_managed_identity ? true : false
  shared_access_key_enabled       = var.use_managed_identity ? false : true

  network_rules {
    default_action = "Deny"
    ip_rules       = [chomp(data.http.myip.response_body)]
  }
}

resource "azurerm_role_assignment" "current_to_sa" {
  for_each = var.use_managed_identity ? toset(["Storage Account Contributor", "Storage Blob Data Owner", "Storage Queue Data Contributor"]) : []

  scope                = azurerm_storage_account.main.id
  role_definition_name = each.key
  principal_id         = data.azuread_client_config.current.object_id
}

resource "azurerm_storage_queue" "main" {
  name                 = "weather-anomalies"
  storage_account_name = azurerm_storage_account.main.name
}

output "AzureWebJobsStorage" {
  value = var.use_managed_identity ? null : (azurerm_storage_account.main.primary_connection_string)
}

output "AzureWebJobsStorage__blobServiceUri" {
  value = var.use_managed_identity ? azurerm_storage_account.main.primary_blob_endpoint : null
}

output "AzureWebJobsStorage__queueServiceUri" {
  value = var.use_managed_identity ? azurerm_storage_account.main.primary_queue_endpoint : null
}