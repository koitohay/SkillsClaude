resource "azurerm_email_communication_service" "this" {
  name                = "${var.name_prefix}-ecs"
  resource_group_name = var.resource_group_name
  data_location       = "Europe"
  tags                = var.tags
}

resource "azurerm_email_communication_service_domain" "managed" {
  name              = "AzureManagedDomain"
  email_service_id  = azurerm_email_communication_service.this.id
  domain_management = "AzureManaged"
}

resource "azurerm_communication_service" "this" {
  name                = "${var.name_prefix}-acs"
  resource_group_name = var.resource_group_name
  data_location       = "Europe"
  tags                = var.tags
}
