output "connection_string" {
  value     = azurerm_communication_service.this.primary_connection_string
  sensitive = true
}

output "sender_address" {
  value = var.sender_address
}

output "email_domain" {
  value = azurerm_email_communication_service_domain.managed.from_sender_domain
}
