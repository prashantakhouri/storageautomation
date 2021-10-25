import React, { useEffect, useRef, useState } from "react";
import OktaSignIn from "@okta/okta-signin-widget";
import "@okta/okta-signin-widget/dist/css/okta-sign-in.min.css";
import { useOktaAuth } from "@okta/okta-react";

const OktaSignInWidget = ({ config, onSuccess, onError }) => {
  const widgetRef = useRef();
  const { oktaAuth, authState } = useOktaAuth();
  const params = new URLSearchParams(window.location.search);
  const [idpUser, setidpUser] = useState(false);

  useEffect(() => {
    if (!widgetRef.current) return false;

    const widget = new OktaSignIn(config);

    const fromLogin = params.get("fromLogin");
    const iss = params.get("iss");
    //const url = widget.hasTokensInUrl();
    if (fromLogin === "true" || iss === config.baseUrl.split("/oauth2")[0]) {
      setidpUser(true);
      oktaAuth.token
        .getWithRedirect({
          scopes: ["openid", "email", "profile", "groups", "offline_access"],
          // sessionToken: res.sessionToken,
        })
        .then((tokens) => {
          oktaAuth.tokenManager.setTokens(tokens);
          console.log(tokens);
          widget.remove();
        })
        .catch((error) => {
          // eslint-disable-next-line
          console.log(error);
        });
    } else if (fromLogin === null && !idpUser) {
      widget
        .showSignInToGetTokens({
          el: widgetRef.current,
        })
        .then(onSuccess)
        .catch(onError);
    }
    return () => widget.remove();
  }, [config, onSuccess, onError]);

  return <div ref={widgetRef} />;
};
export default OktaSignInWidget;
