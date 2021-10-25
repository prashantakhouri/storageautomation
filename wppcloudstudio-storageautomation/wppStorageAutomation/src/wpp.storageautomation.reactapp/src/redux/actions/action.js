import * as type from "../types";

export function getProductionStores(productionStores) {
  return {
    type: type.GET_PRODUCTIONSTORES,
    payload: productionStores,
  };
}
