variable "environment" {
  description = "Deployment environment (dev, test, prod)"
  type        = string
  validation {
    condition     = contains(["dev", "test", "prod"], var.environment)
    error_message = "Environment must be dev, test, or prod."
  }
}

variable "location" {
  description = "Azure region"
  type        = string
  default     = "northeurope"
}

variable "project" {
  description = "Project name used in resource naming"
  type        = string
  default     = "servicematch"
}

variable "postgresql_sku" {
  description = "SKU for PostgreSQL Flexible Server"
  type        = string
  default     = "B_Standard_B1ms"
}

variable "postgresql_storage_mb" {
  description = "Storage in MB for PostgreSQL"
  type        = number
  default     = 32768
}

variable "postgresql_admin_password" {
  description = "PostgreSQL admin password"
  type        = string
  sensitive   = true
}

variable "jwt_secret" {
  description = "JWT signing secret (min 32 chars)"
  type        = string
  sensitive   = true
}

variable "container_app_cpu" {
  description = "CPU allocation for API container app"
  type        = number
  default     = 0.25
}

variable "container_app_memory" {
  description = "Memory allocation for API container app"
  type        = string
  default     = "0.5Gi"
}

variable "container_app_min_replicas" {
  description = "Minimum replicas for API container app"
  type        = number
  default     = 0
}

variable "container_app_max_replicas" {
  description = "Maximum replicas for API container app"
  type        = number
  default     = 3
}

variable "acr_name" {
  description = "Azure Container Registry name (shared across environments)"
  type        = string
}

variable "acr_resource_group" {
  description = "Resource group of the shared ACR"
  type        = string
}

variable "api_image_tag" {
  description = "Docker image tag to deploy for the API"
  type        = string
  default     = "latest"
}

variable "frontend_image_tag" {
  description = "Docker image tag to deploy for the frontend"
  type        = string
  default     = "latest"
}

variable "acs_sender_address" {
  description = "Azure Communication Services sender email address"
  type        = string
  default     = "no-reply@servicematch.dk"
}

variable "allowed_origins" {
  description = "CORS allowed origins for the API"
  type        = list(string)
  default     = []
}
