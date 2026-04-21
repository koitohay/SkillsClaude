output "api_url" {
  value = "https://${azurerm_container_app.api.latest_revision_fqdn}"
}

output "container_app_environment_id" {
  value = azurerm_container_app_environment.this.id
}

output "managed_identity_id" {
  value = azurerm_user_assigned_identity.api.id
}
