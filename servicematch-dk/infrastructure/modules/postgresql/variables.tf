variable "resource_group_name" { type = string }
variable "location"            { type = string }
variable "name_prefix"         { type = string }
variable "tags"                { type = map(string) }
variable "sku_name"            { type = string }
variable "storage_mb"          { type = number }
variable "admin_password"      { type = string; sensitive = true }
