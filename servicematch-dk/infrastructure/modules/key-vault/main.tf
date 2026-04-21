data "azurerm_client_config" "current" {}

resource "azurerm_key_vault" "this" {
  name                        = "${var.name_prefix}-kv"
  resource_group_name         = var.resource_group_name
  location                    = var.location
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  sku_name                    = "standard"
  soft_delete_retention_days  = 7
  purge_protection_enabled    = false
  tags                        = var.tags

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    secret_permissions = ["Get", "List", "Set", "Delete", "Purge"]
  }
}

resource "azurerm_key_vault_secret" "postgresql" {
  name         = "postgresql-connection-string"
  value        = var.postgresql_connection_string
  key_vault_id = azurerm_key_vault.this.id
}

resource "azurerm_key_vault_secret" "jwt" {
  name         = "jwt-secret"
  value        = var.jwt_secret
  key_vault_id = azurerm_key_vault.this.id
}

resource "azurerm_key_vault_secret" "acs" {
  name         = "acs-connection-string"
  value        = var.acs_connection_string
  key_vault_id = azurerm_key_vault.this.id
}

resource "azurerm_key_vault_secret" "appinsights" {
  name         = "appinsights-connection-string"
  value        = var.application_insights_conn_str
  key_vault_id = azurerm_key_vault.this.id
}
