output "frontend_url" {
  value = "https://${azurerm_container_app.frontend.latest_revision_fqdn}"
}
