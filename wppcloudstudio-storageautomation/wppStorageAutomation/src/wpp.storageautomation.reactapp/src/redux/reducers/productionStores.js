import * as type from "../types";

const initialState = {
  productionStores: [],
};

export default function productionStores(state = initialState, action) {
  switch (action.type) {
    case type.GET_PRODUCTIONSTORES:
      return {
        ...state,
        productionStores: action.payload,
      };
    default:
      return state;
  }
}
