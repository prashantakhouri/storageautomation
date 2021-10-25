# #############################################################################
# # OUTPUTS Application Insights
# #############################################################################

output "instrumentation_key" {
  value = "${azurerm_application_insights.this.instrumentation_key}"
}

output "name" {
  value = "${azurerm_application_insights.this.name}"
}

output "location" {
  value = "${azurerm_application_insights.this.location}"
}

output "connection_string" {
  value = "${azurerm_application_insights.this.connection_string}"
}
