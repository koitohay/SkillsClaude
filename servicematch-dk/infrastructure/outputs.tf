output "resource_group_name" {
  value = azurerm_resource_group.main.name
}

output "api_url" {
  value = module.container_app.api_url
}

output "frontend_url" {
  value = module.static_web_app.frontend_url
}

output "postgresql_fqdn" {
  value     = module.postgresql.fqdn
  sensitive = false
}

output "key_vault_uri" {
  value = module.key_vault.key_vault_uri
}

output "application_insights_instrumentation_key" {
  value     = module.application_insights.instrumentation_key
  sensitive = true
}
