variable "resource_group_name"  { type = string }
variable "location"              { type = string }
variable "name_prefix"           { type = string }
variable "tags"                  { type = map(string) }
variable "api_base_url"          { type = string }
variable "frontend_image_tag"    { type = string }
variable "acr_login_server"      { type = string }
variable "acr_admin_username"    { type = string }
variable "acr_admin_password"    { type = string; sensitive = true }
