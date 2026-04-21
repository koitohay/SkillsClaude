variable "resource_group_name"  { type = string }
variable "location"              { type = string }
variable "name_prefix"           { type = string }
variable "tags"                  { type = map(string) }
variable "acr_login_server"      { type = string }
variable "acr_admin_username"    { type = string }
variable "acr_admin_password"    { type = string; sensitive = true }
variable "api_image_tag"         { type = string }
variable "cpu"                   { type = number }
variable "memory"                { type = string }
variable "min_replicas"          { type = number }
variable "max_replicas"          { type = number }
variable "key_vault_id"          { type = string }
variable "applicationinsights_connection_string" { type = string; sensitive = true }
variable "allowed_origins"       { type = list(string); default = [] }

variable "environment_variables" {
  type    = map(string)
  default = {}
}

variable "key_vault_secret_refs" {
  type    = map(string)
  default = {}
}
