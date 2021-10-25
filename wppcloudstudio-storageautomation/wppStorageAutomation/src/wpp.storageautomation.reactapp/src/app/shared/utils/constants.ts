const CREATE_PRODUCTION = {
  EMPTY_ERROR:
    "Production Name cannot be empty. Please enter a production name.",
  SPECIAL_CHARS_ERROR:
    "Only regular characters can be used for a name, please avoid using special characters like '\\ / : | < > * ?'",
  CREATE_PRODUCTION_SUCCESS: "created successfully.",
  DUPLICATE_NAME:
    "The name provided has already been used, please give a different name.",
  MAX_LIMIT: "Maximum character limit is 255.",
  MIN_LIMIT: "Minimum character limit is 8.",
  SECURITY_IDENTIFIER:
    "Something went wrong while creating the Production, please contact the system administrator.",
  SECURITY_NOTAUTHORIZED:
    "Sorry, you don't have access to any ProductionStores.",
  ERROR_WENTWRONG: "Sorry, something went wrong.",
};

const Restore_Prod = {
  Success: "restored successfully.",
  Error: "Restore failed. Production already exists in WIP.",
  NotExists: "Restore failed. Production does not exist in Archive.",
};

const MakeOffline_Prod = {
  Success: "taken offline successfully.",
  Error:
    "Make offline failed because some file/folder(s) are open. Please close all files and folders and try again.",
  NotExists:
    "Make offline failed. Production or production store does not exist in WIP.",
};
const Delete_Prod = {
  Success: "deleted successfully.",
  ForbiddenError:"You are not authorized to delete any production in this production store.",
  Error:
    "Deletion failed. Please try again"
};

export { CREATE_PRODUCTION, Restore_Prod, MakeOffline_Prod, Delete_Prod };
