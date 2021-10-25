import { Component } from "react";
import { withAuth } from "@okta/okta-react";
import { LoadingSpinner } from "../../app/shared/spinner/spinner";
import {  SpinnerSize} from "@fluentui/react/lib/Spinner";
import "../App.css";

export default withAuth(
  class Home extends Component {
    state = { authenticated: null };
    state = { loading: true };

    checkAuthentication = async () => {
      const authenticated = await this.props.auth.isAuthenticated();
      if (authenticated !== this.state.authenticated) {
        this.setState({ authenticated });
        this.setState({ loading: true });
      }
    };

    async componentDidMount() {
      this.setState({ loading: true });
      this.checkAuthentication();
    }

    login = async () => {
      this.props.auth.login("/");
    };

    logout = async () => {
      this.props.auth.logout("/");
    };

    render() {
      if (
        this.state.authenticated === null &&
        this.state.authenticated === undefined
      )
        return null;

      const mainContent = this.state.authenticated
        ? (window.location.href = `/home`)
        : (window.location.href = "/login");

      return (
        <div className="App">
          {this.state.loading ? <LoadingSpinner spinnerSize={SpinnerSize.large} /> : mainContent}
        </div>
      );
    }
  }
);
