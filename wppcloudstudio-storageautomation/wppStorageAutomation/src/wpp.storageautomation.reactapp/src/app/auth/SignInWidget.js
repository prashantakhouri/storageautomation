import React, { Component } from "react";
import ReactDOM from "react-dom";
import OktaSignIn from "@okta/okta-signin-widget";
import "@okta/okta-signin-widget/dist/css/okta-sign-in.min.css";
import "@okta/okta-signin-widget/dist/css/okta-theme.css";
import { OktaAuth } from "@okta/okta-auth-js";

class SignInWidget extends Component {
  componentDidMount() {
    const el = ReactDOM.findDOMNode(this);
    this.widget = new OktaSignIn({
      baseUrl: this.props.baseUrl,
      logo: "okta-logo.png",
      features: {
        idpDiscovery: true,
      },
      idpDiscovery: {
        requestContext: window.location.href,
      },
    });
    // const authServerBaseUrl = process.env.REACT_APP_AUTH_SERVER_URL;
    // const authServerUrl = `${authServerBaseUrl}`;
    // const authServerClientId = process.env.REACT_APP_APP_AUTH_CLIENTID;
    // var config = {
    //   // Required config
    //   issuer: `${authServerUrl}/api/v1/authorizationServers/default`,

    //   // Required for login flow using getWithRedirect()
    //   clientId: authServerClientId,
    //   redirectUri: `${window.location.origin + "/implicit/callback"}`,
    // };

    // this.widget.authClient = new OktaAuth(config);
    // console.log(this.widget, " Widget");
    // console.log(this.widget.authClient, "authclie");
    // //  console.log(this.widget.authClient.session.exists,"Session");

    // try {
    //   //  if(sessionEx)
    //   //  {
    //   //    console.log(sessionEx, "")
    //   //   this.widget.authClient.getWithoutPrompt().then(function (response) {
    //   //           this.widget.authClient.tokenManager.setTokens(response.tokens);
    //   //         });
    //   //  }
    //   this.widget.authClient.session.exists().then(function (sessionExists) {
    //     if (sessionExists) {
    //       console.log("session exist condition");
    //       this.widget.authClient.token
    //         .getWithoutPrompt()
    //         .then(function (response) {
    //           console.log("token", response.tokens);
    //           this.widget.authClient.tokenManager.setTokens(response.tokens);
    //         });
    //     }
    //   });
    // } catch (ex) {
    //   console.log(ex, "exception");
    // }

    this.widget.renderEl({ el }, this.props.onSuccess, this.props.onError);
  }

  componentWillUnmount() {
    this.widget.remove();
  }

  render() {
    return <div />;
  }
}

export default SignInWidget;
