resource "azurerm_container_app_environment" "this" {
  name                = "${var.name_prefix}-cae"
  resource_group_name = var.resource_group_name
  location            = var.location
  tags                = var.tags
}

resource "azurerm_user_assigned_identity" "api" {
  name                = "${var.name_prefix}-api-identity"
  resource_group_name = var.resource_group_name
  location            = var.location
  tags                = var.tags
}

resource "azurerm_key_vault_access_policy" "api_identity" {
  key_vault_id = var.key_vault_id
  tenant_id    = azurerm_user_assigned_identity.api.tenant_id
  object_id    = azurerm_user_assigned_identity.api.principal_id

  secret_permissions = ["Get", "List"]
}

resource "azurerm_container_app" "api" {
  name                         = "${var.name_prefix}-api"
  resource_group_name          = var.resource_group_name
  container_app_environment_id = azurerm_container_app_environment.this.id
  revision_mode                = "Single"
  tags                         = var.tags

  identity {
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.api.id]
  }

  registry {
    server               = var.acr_login_server
    username             = var.acr_admin_username
    password_secret_name = "acr-password"
  }

  secret {
    name  = "acr-password"
    value = var.acr_admin_password
  }

  template {
    min_replicas = var.min_replicas
    max_replicas = var.max_replicas

    container {
      name   = "api"
      image  = "${var.acr_login_server}/servicematch-api:${var.api_image_tag}"
      cpu    = var.cpu
      memory = var.memory

      dynamic "env" {
        for_each = var.environment_variables
        content {
          name  = env.key
          value = env.value
        }
      }

      dynamic "env" {
        for_each = var.key_vault_secret_refs
        content {
          name        = env.key
          secret_name = env.value
        }
      }

      env {
        name  = "APPLICATIONINSIGHTS_CONNECTION_STRING"
        value = var.applicationinsights_connection_string
      }

      liveness_probe {
        path      = "/api/v1/health"
        port      = 8080
        transport = "HTTP"
      }

      readiness_probe {
        path      = "/api/v1/health"
        port      = 8080
        transport = "HTTP"
      }
    }
  }

  ingress {
    external_enabled = true
    target_port      = 8080

    traffic_weight {
      percentage      = 100
      latest_revision = true
    }

    dynamic "custom_domain" {
      for_each = var.allowed_origins
      content {
        name = custom_domain.value
      }
    }
  }

  depends_on = [azurerm_key_vault_access_policy.api_identity]
}
