# #############################################################################
# # OUTPUT APIM
# #############################################################################

output "apim_id" {
  value = "${azurerm_api_management.this.id}"
}

output "name" {
  value = "${azurerm_api_management.this.name}"
}

output "location" {
  value = "${azurerm_api_management.this.location}"
}

output "instrumentation_key" {
  value = "${data.azurerm_application_insights.this.instrumentation_key}"
}