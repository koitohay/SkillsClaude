resource "azurerm_postgresql_flexible_server" "this" {
  name                   = "${var.name_prefix}-psql"
  resource_group_name    = var.resource_group_name
  location               = var.location
  version                = "16"
  administrator_login    = "psqladmin"
  administrator_password = var.admin_password
  sku_name               = var.sku_name
  storage_mb             = var.storage_mb
  backup_retention_days  = 7
  zone                   = "1"
  tags                   = var.tags
}

resource "azurerm_postgresql_flexible_server_database" "app" {
  name      = "servicematch"
  server_id = azurerm_postgresql_flexible_server.this.id
  charset   = "UTF8"
  collation = "en_US.utf8"
}

resource "azurerm_postgresql_flexible_server_firewall_rule" "azure_services" {
  name             = "AllowAzureServices"
  server_id        = azurerm_postgresql_flexible_server.this.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}
