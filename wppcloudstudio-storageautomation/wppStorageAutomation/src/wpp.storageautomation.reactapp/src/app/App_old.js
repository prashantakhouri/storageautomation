import React, { Component } from "react";
import { BrowserRouter as Router, Route } from "react-router-dom";
import { mergeStyles, ThemeProvider } from "@fluentui/react";
import { Security, SecureRoute, ImplicitCallback } from "@okta/okta-react";
import Home from "./pages/Home";
import Login from "./auth/Login";
import "./App.css";
import { CreateProduction } from "./pages/production/createproduction";
import { ListProduction } from "./pages/production/list-production";
import { darkTheme } from "./themes";
import { Layout } from "./shared/layout/layout";

// Inject some global styles
mergeStyles({
  ":global(body,html,#root)": {
    margin: 0,
    padding: 0,
    height: "100vh",
  },
});

function onAuthRequired({ history }) {
  history.push("/login");
}

const authServerBaseUrl = process.env.REACT_APP_AUTH_SERVER_URL;
const authServerUrl = `${authServerBaseUrl}`;
const authServerClientId = process.env.REACT_APP_APP_AUTH_CLIENTID;

class App extends Component {
  render() {
    return (
      <Router>
        <Security
          issuer={authServerUrl}
          client_id={authServerClientId}
          redirect_uri={window.location.origin + "/implicit/callback"}
          scope={["openid", "email", "profile", "groups"]}
                onAuthRequired={onAuthRequired}
        >
          <div className="container">
            <ThemeProvider applyTo="body" theme={darkTheme}>
              <Route path="/" exact={true} component={Home} />

              <SecureRoute
                path="/create-production/:productionStoreId"
                exact={true}
                component={CreateProduction}
              />
              <SecureRoute
                path="/:id"
                exact={true}
                component={ListProduction}
              />
              <Route
                path="/login"
                render={() => <Login baseUrl={authServerBaseUrl} />}
              />
              <Route path="/implicit/callback" component={ImplicitCallback} />
            </ThemeProvider>
          </div>
        </Security>
      </Router>
    );
  }
}

export default App;
