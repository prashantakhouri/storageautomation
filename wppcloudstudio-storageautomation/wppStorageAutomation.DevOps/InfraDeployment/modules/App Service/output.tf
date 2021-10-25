# #############################################################################
# # OUTPUTS App Service
# #############################################################################

output "appserviceplan_id" {
  value = "${azurerm_app_service_plan.this.id}"
}

output "name" {
  value = "${azurerm_app_service.this.name}"
}

output "location" {
  value = "${azurerm_app_service.this.location}"
}



