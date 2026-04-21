resource "azurerm_container_app_environment" "frontend" {
  name                = "${var.name_prefix}-frontend-cae"
  resource_group_name = var.resource_group_name
  location            = var.location
  tags                = var.tags
}

resource "azurerm_container_app" "frontend" {
  name                         = "${var.name_prefix}-frontend"
  resource_group_name          = var.resource_group_name
  container_app_environment_id = azurerm_container_app_environment.frontend.id
  revision_mode                = "Single"
  tags                         = var.tags

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
    min_replicas = 1
    max_replicas = 3

    container {
      name   = "frontend"
      image  = "${var.acr_login_server}/servicematch-frontend:${var.frontend_image_tag}"
      cpu    = 0.25
      memory = "0.5Gi"

      env {
        name  = "NUXT_PUBLIC_API_BASE_URL"
        value = var.api_base_url
      }

      env {
        name  = "NODE_ENV"
        value = "production"
      }
    }
  }

  ingress {
    external_enabled = true
    target_port      = 3000

    traffic_weight {
      percentage      = 100
      latest_revision = true
    }
  }
}
