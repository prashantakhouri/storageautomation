import ReactDOM from "react-dom";
import { mergeStyles } from "@fluentui/react";
import reportWebVitals from "./reportWebVitals";
import { initializeIcons } from "@fluentui/react/lib/Icons";
import axios from "axios";
import App from "../src/app/App";
import { Provider } from "react-redux";
import store from "./redux/store.js";

mergeStyles({
  ":global(body,html,#root)": {
    margin: 0,
    padding: 0,
    height: "100vh",
  },
});
initializeIcons();
axios.interceptors.request.use((request) => {
  const token = localStorage.getItem("okta-token-storage");
  if (token == null) {
    window.location.href = "login";
  }
  return request;
});

ReactDOM.render(
  <Provider store={store}>
    <App />
  </Provider>,
  document.getElementById("root")
);

reportWebVitals();
