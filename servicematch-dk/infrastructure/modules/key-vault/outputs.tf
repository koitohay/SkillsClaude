output "key_vault_id"  { value = azurerm_key_vault.this.id }
output "key_vault_uri" { value = azurerm_key_vault.this.vault_uri }

output "secret_name_postgresql"  { value = azurerm_key_vault_secret.postgresql.name }
output "secret_name_jwt"         { value = azurerm_key_vault_secret.jwt.name }
output "secret_name_acs"         { value = azurerm_key_vault_secret.acs.name }
output "secret_name_appinsights" { value = azurerm_key_vault_secret.appinsights.name }
