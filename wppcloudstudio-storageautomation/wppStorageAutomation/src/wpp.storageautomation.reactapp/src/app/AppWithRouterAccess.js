import React from "react";
import { Route, useHistory, Switch } from "react-router-dom";
import { Security, SecureRoute, LoginCallback } from "@okta/okta-react";
import { OktaAuth, toRelativeUrl } from "@okta/okta-auth-js";
import { mergeStyles, ThemeProvider } from "@fluentui/react";
import { darkTheme } from "./themes";
import Home from "../app/pages/Home";
import Login from "../app/auth/Login";
import { CreateProduction } from "./pages/production/createproduction";
import { ListProduction } from "./pages/production/list-production";
//import Protected from "./Protected";
import { oktaAuthConfig, oktaSignInConfig } from "../app/auth/config";

const oktaAuth = new OktaAuth(oktaAuthConfig);

const AppWithRouterAccess = () => {
  const history = useHistory();

  const customAuthHandler = () => {
    history.push("/login");
  };
  
  const restoreOriginalUri = async (_oktaAuth, originalUri) => {
    history.replace(toRelativeUrl(originalUri, window.location.origin));
  };

  return (
    <Security
      oktaAuth={oktaAuth}
      onAuthRequired={customAuthHandler}
      restoreOriginalUri={restoreOriginalUri}
    >
      <div className="container">
        <ThemeProvider applyTo="body" theme={darkTheme}>
          <Route path="/" exact={true} component={Home} />

          <SecureRoute
            path="/create-production/:productionStoreId"
            exact={true}
            component={CreateProduction}
          />
          <SecureRoute path="/:id" exact={true} component={ListProduction} />
          <Route
            path="/login"
            render={() => <Login config={oktaSignInConfig} />}
          />
          <Route path="/implicit/callback" component={LoginCallback} />
        </ThemeProvider>
      </div>
    </Security>
  );
};
export default AppWithRouterAccess;
