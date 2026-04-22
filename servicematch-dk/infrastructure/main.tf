terraform {
  required_version = ">= 1.7.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.100"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = "~> 2.47"
    }
  }
}

provider "azurerm" {
  features {
    key_vault {
      purge_soft_delete_on_destroy    = false
      recover_soft_deleted_key_vaults = true
    }
  }
}

locals {
  name_prefix = "${var.project}-${var.environment}"
  tags = {
    project     = var.project
    environment = var.environment
    managed_by  = "terraform"
  }
}

resource "azurerm_resource_group" "main" {
  name     = "${local.name_prefix}-rg"
  location = var.location
  tags     = local.tags
}

# ACR reference (shared, not managed per-env)
data "azurerm_container_registry" "acr" {
  name                = var.acr_name
  resource_group_name = var.acr_resource_group
}

module "application_insights" {
  source              = "./modules/application-insights"
  resource_group_name = azurerm_resource_group.main.name
  location            = var.location
  name_prefix         = local.name_prefix
  tags                = local.tags
}

module "key_vault" {
  source              = "./modules/key-vault"
  resource_group_name = azurerm_resource_group.main.name
  location            = var.location
  name_prefix         = local.name_prefix
  tags                = local.tags

  jwt_secret                     = var.jwt_secret
  postgresql_connection_string   = module.postgresql.connection_string
  acs_connection_string          = module.communication_services.connection_string
  application_insights_conn_str  = module.application_insights.connection_string
  anthropic_api_key              = var.anthropic_api_key
}

module "postgresql" {
  source              = "./modules/postgresql"
  resource_group_name = azurerm_resource_group.main.name
  location            = var.location
  name_prefix         = local.name_prefix
  tags                = local.tags

  sku_name   = var.postgresql_sku
  storage_mb = var.postgresql_storage_mb
  admin_password = var.postgresql_admin_password
}

module "communication_services" {
  source              = "./modules/communication-services"
  resource_group_name = azurerm_resource_group.main.name
  name_prefix         = local.name_prefix
  tags                = local.tags
  sender_address      = var.acs_sender_address
}

module "container_app" {
  source              = "./modules/container-app"
  resource_group_name = azurerm_resource_group.main.name
  location            = var.location
  name_prefix         = local.name_prefix
  tags                = local.tags

  acr_login_server = data.azurerm_container_registry.acr.login_server
  acr_admin_username = data.azurerm_container_registry.acr.admin_username
  acr_admin_password = data.azurerm_container_registry.acr.admin_password
  api_image_tag    = var.api_image_tag

  cpu          = var.container_app_cpu
  memory       = var.container_app_memory
  min_replicas = var.container_app_min_replicas
  max_replicas = var.container_app_max_replicas

  key_vault_id           = module.key_vault.key_vault_id
  applicationinsights_connection_string = module.application_insights.connection_string
  allowed_origins        = var.allowed_origins

  environment_variables = {
    ASPNETCORE_ENVIRONMENT = var.environment == "prod" ? "Production" : "Development"
    Email__Provider        = "acs"
  }

  key_vault_secret_refs = {
    ConnectionStrings__DefaultConnection = module.key_vault.secret_name_postgresql
    Jwt__Secret                          = module.key_vault.secret_name_jwt
    Email__AcsConnectionString           = module.key_vault.secret_name_acs
    ApplicationInsights__ConnectionString = module.key_vault.secret_name_appinsights
    Anthropic__ApiKey                    = module.key_vault.secret_name_anthropic
  }
}

module "static_web_app" {
  source              = "./modules/static-web-app"
  resource_group_name = azurerm_resource_group.main.name
  location            = var.location
  name_prefix         = local.name_prefix
  tags                = local.tags

  api_base_url    = module.container_app.api_url
  frontend_image_tag = var.frontend_image_tag
  acr_login_server   = data.azurerm_container_registry.acr.login_server
  acr_admin_username = data.azurerm_container_registry.acr.admin_username
  acr_admin_password = data.azurerm_container_registry.acr.admin_password
}
