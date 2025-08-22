provider "azurerm" {
  features {}

  skip_provider_registration = true
}

resource "random_pet" "main" {
}

data "azuread_client_config" "current" {}

data "http" "myip" {
  url = "http://ipv4.icanhazip.com"
}

resource "azuread_application" "main" {
  display_name            = "${lower(random_pet.main.id)}-app"
  group_membership_claims = ["SecurityGroup"]
  owners                  = [data.azuread_client_config.current.object_id]
}

resource "azuread_service_principal" "main" {
  client_id = azuread_application.main.client_id
  owners    = [data.azuread_client_config.current.object_id]
}

resource "azuread_service_principal_password" "main" {
  service_principal_id = azuread_service_principal.main.object_id
}

resource "azurerm_resource_group" "main" {
  name     = "${lower(random_pet.main.id)}-rg"
  location = "West Europe"
}

resource "azurerm_servicebus_namespace" "main" {
  name                = "${lower(random_pet.main.id)}-sbns"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  sku                 = "Standard"
}

resource "azurerm_servicebus_topic" "main" {
  name         = "topic"
  namespace_id = azurerm_servicebus_namespace.main.id
}

resource "azurerm_role_assignment" "main_sp_to_servicebus_topic" {
  scope                = azurerm_servicebus_topic.main.id
  role_definition_name = "Azure Service Bus Data Sender"
  principal_id         = azuread_service_principal.main.object_id
}

resource "azurerm_eventhub_namespace" "main" {
  name                = "${lower(random_pet.main.id)}-evhns"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  sku                 = "Standard"
  capacity            = 2
}

resource "azurerm_eventhub_namespace_schema_group" "main" {
  name                 = "main"
  namespace_id         = azurerm_eventhub_namespace.main.id
  schema_compatibility = "Forward"
  schema_type          = "Avro"
}

resource "azurerm_role_assignment" "main_sp_to_schema_registry" {
  scope                = azurerm_eventhub_namespace.main.id
  role_definition_name = "Schema Registry Contributor"
  principal_id         = azuread_service_principal.main.object_id
}

resource "azurerm_eventhub" "main" {
  name                = "hub"
  namespace_name      = azurerm_eventhub_namespace.main.name
  resource_group_name = azurerm_resource_group.main.name
  partition_count     = 2
  message_retention   = 1
}

resource "azurerm_role_assignment" "main_sp_to_eventhub" {
  scope                = azurerm_eventhub.main.id
  role_definition_name = "Azure Event Hubs Data Sender"
  principal_id         = azuread_service_principal.main.object_id
}

resource "azurerm_storage_account" "main" {
  name                     = "${lower(replace(random_pet.main.id, "/[_-]/", ""))}sa"
  resource_group_name      = azurerm_resource_group.main.name
  location                 = azurerm_resource_group.main.location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  network_rules {
    default_action = "Deny"
    ip_rules       = [chomp(data.http.myip.response_body)]
  }
}

output "AzureAd__ClientId" {
  value = azuread_application.main.client_id
}

output "AzureAd__ClientSecret" {
  value = nonsensitive(azuread_service_principal_password.main.value)
}

output "AzureAd__TenantId" {
  value = data.azuread_client_config.current.tenant_id
}

output "EventHub__ConnectionString" {
  value = nonsensitive(azurerm_eventhub_namespace.main.default_primary_connection_string)
}

output "EventHub__Url" {
  value = "sb://${azurerm_eventhub_namespace.main.name}.servicebus.windows.net"
}

output "EventHub__HubName" {
  value = azurerm_eventhub.main.name
}

output "ServiceBus__ConnectionString" {
  value = nonsensitive(azurerm_servicebus_namespace.main.default_primary_connection_string)
}

output "ServiceBus__Url" {
  value = "sb://${azurerm_servicebus_namespace.main.name}.servicebus.windows.net"
}

output "ServiceBus__TopicName" {
  value = azurerm_servicebus_topic.main.name
}

output "SchemaRegistry__Namespace" {
  value = "${azurerm_eventhub_namespace.main.name}.servicebus.windows.net"
}

output "SchemaRegistry__GroupName" {
  value = azurerm_eventhub_namespace_schema_group.main.name
}

output "StorageAccount__ConnectionString" {
  value = nonsensitive(azurerm_storage_account.main.primary_connection_string)
}
