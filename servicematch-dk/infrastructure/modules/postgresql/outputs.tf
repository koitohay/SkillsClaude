output "fqdn" {
  value = azurerm_postgresql_flexible_server.this.fqdn
}

output "connection_string" {
  value     = "Host=${azurerm_postgresql_flexible_server.this.fqdn};Database=servicematch;Username=psqladmin;Password=${var.admin_password};SSL Mode=Require;"
  sensitive = true
}
