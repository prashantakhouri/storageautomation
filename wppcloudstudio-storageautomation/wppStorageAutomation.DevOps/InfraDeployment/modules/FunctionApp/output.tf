# #############################################################################
# # OUTPUTS Function App
# #############################################################################

output "appserviceplan_id" {
  value = "${azurerm_app_service_plan.this.id}"
}

output "name" {
  value = "${azurerm_function_app.this.name}"
}

output "Functionapp_id" {
  value = "${azurerm_function_app.this.id}"
}

output "location" {
  value = "${azurerm_function_app.this.location}"
}

output "principal_id" {
  value = azurerm_function_app.this.identity[0].principal_id
}
output "tenant_id" {
  value = azurerm_function_app.this.identity[0].tenant_id
}

output "kv_id" {
  value = "${data.azurerm_key_vault.this1.id}"
}



