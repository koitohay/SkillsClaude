terraform {
  backend "azurerm" {
    resource_group_name  = "servicematch-tfstate-rg"
    storage_account_name = "servicematchtfstate"
    container_name       = "tfstate"
    key                  = "servicematch.terraform.tfstate"
  }
}
