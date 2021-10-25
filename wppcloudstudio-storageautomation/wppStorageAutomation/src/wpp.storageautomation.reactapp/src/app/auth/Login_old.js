import React, { Component } from 'react';
import { Redirect } from 'react-router-dom';
import SignInWidget from './SignInWidget';
import { withAuth } from '@okta/okta-react';
import { OktaAuth } from '@okta/okta-auth-js';

export default withAuth(
  class Login extends Component {
    constructor(props) {
      super(props);
      this.state = {
        authenticated: null
      };
      this.checkAuthentication();
    }

    async checkAuthentication() {
      console.log("auth");
      const authenticated = await this.props.auth.isAuthenticated();
      console.log(authenticated, "auth result");
      if (authenticated !== this.state.authenticated) {
        this.setState({ authenticated });
      }
    }

    componentDidUpdate() {
      this.checkAuthentication();
    }

    onSuccess = res => {
      // if(res.status === 'IDP_DISCOVERY'){
      //   console.log("idp");
      //   console.log(res, "res after idp");
      //  // console.log(res.session.token, "token after idp");
      //   res.idpDiscovery.redirectToIdp();
      if(res.session !=null)
      {
      return this.props.auth.redirect({
        sessionToken: res.session.token
      })};    
    };

    onError = err => {
      console.log('error logging in', err);
    };

    render() {
      if (this.state.authenticated === null) return null;
      return this.state.authenticated ? (
        <Redirect to={{ pathname: '/' }} />
      ) : (
        <SignInWidget
          baseUrl={this.props.baseUrl}
          onSuccess={this.onSuccess}
          onError={this.onError}
        />        
      );
    }
  }
);
