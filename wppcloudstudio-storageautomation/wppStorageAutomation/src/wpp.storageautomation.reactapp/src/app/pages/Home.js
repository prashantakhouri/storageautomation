import React, { useState } from "react";
import { Link } from "react-router-dom";
import { useHistory } from "react-router-dom";
import { useOktaAuth } from "@okta/okta-react";
import { LoadingSpinner } from "../../app/shared/spinner/spinner";
import { SpinnerSize } from "@fluentui/react/lib/Spinner";
import "../App.css";

const Home = () => {
  const history = useHistory();
  const { oktaAuth, authState } = useOktaAuth();
  const [loading, setloading] = useState(true);

  if (!authState) return null;

  const login = async () => history.push("/login");

  const logout = async () => oktaAuth.signOut();

  const button = authState.isAuthenticated ? (
    <button onClick={logout}>Logout</button>
  ) : (
    <button onClick={login}>Login</button>
  );

  const mainContent = authState.isAuthenticated
    ? (window.location.href = `/home`)
    : (window.location.href = "/login");

  return (
    <div className="App">
      {this.state.loading ? (
        <LoadingSpinner spinnerSize={SpinnerSize.large} />
      ) : (
        mainContent
      )}
    </div>
  );
};
export default Home;
