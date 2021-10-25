import { combineReducers } from "redux";
import productionStores from "../redux/reducers/productionStores";

const rootReducer = combineReducers({
  productionStores: productionStores,
});

export default rootReducer;
