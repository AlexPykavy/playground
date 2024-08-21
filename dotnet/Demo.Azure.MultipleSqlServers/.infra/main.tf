provider "azurerm" {
  features {}
}

locals {
  administrator_login = "superuser"
}

data "http" "myip" {
  url = "http://ipv4.icanhazip.com"
}

resource "random_id" "main" {
  byte_length = 8
}

resource "azurerm_resource_group" "main" {
  name     = "${lower(random_id.main.hex)}-rg"
  location = "West Europe"
}

resource "random_password" "administrator_login_password" {
  length           = 40
  override_special = "!$#%"
}

resource "azurerm_mssql_server" "main" {
  for_each = toset(["main", "readonly"])

  name                         = "${lower(random_id.main.hex)}-${each.key}-sql"
  resource_group_name          = azurerm_resource_group.main.name
  location                     = each.key == "readonly" ? "North Europe" : azurerm_resource_group.main.location
  version                      = "12.0"
  administrator_login          = local.administrator_login
  administrator_login_password = random_password.administrator_login_password.result
}

resource "azurerm_sql_firewall_rule" "main" {
  for_each = toset(["main", "readonly"])

  name                = "myip"
  resource_group_name = azurerm_resource_group.main.name
  server_name         = azurerm_mssql_server.main[each.key].name
  start_ip_address    = chomp(data.http.myip.response_body)
  end_ip_address      = chomp(data.http.myip.response_body)
}

resource "azurerm_mssql_database" "main" {
  name      = "main"
  server_id = azurerm_mssql_server.main["main"].id
}


resource "azurerm_mssql_database" "readonly" {
  name                        = "readonly"
  server_id                   = azurerm_mssql_server.main["readonly"].id
  create_mode                 = "Secondary"
  creation_source_database_id = azurerm_mssql_database.migration.id
}

output "Databases__Main__ConnectionString" {
  value = nonsensitive("Server=tcp:${azurerm_mssql_server.main["main"].name}.database.windows.net,1433;Initial Catalog=${azurerm_mssql_database.main.name};Persist Security Info=False;User ID=${local.administrator_login};Password=${random_password.administrator_login_password.result};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")
}

output "Databases__ReadOnly__ConnectionString" {
  value = nonsensitive("Server=tcp:${azurerm_mssql_server.main["readonly"].name}.database.windows.net,1433;Initial Catalog=${azurerm_mssql_database.readonly.name};Persist Security Info=False;User ID=${local.administrator_login};Password=${random_password.administrator_login_password.result};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")
}
