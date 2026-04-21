variable "resource_group_name"          { type = string }
variable "location"                      { type = string }
variable "name_prefix"                   { type = string }
variable "tags"                          { type = map(string) }
variable "jwt_secret"                    { type = string; sensitive = true }
variable "postgresql_connection_string"  { type = string; sensitive = true }
variable "acs_connection_string"         { type = string; sensitive = true }
variable "application_insights_conn_str" { type = string; sensitive = true }
