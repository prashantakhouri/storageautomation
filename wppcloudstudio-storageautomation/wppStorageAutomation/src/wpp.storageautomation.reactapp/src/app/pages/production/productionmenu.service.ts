import axios from "axios";

const setAPIAuthentication = () => {
  const token = localStorage.getItem("okta-token-storage");
  if (token != null) {
    const idToken = JSON.parse(token!);
    const bearerToken = idToken.accessToken.accessToken;
    axios.defaults.headers.common = { AppAuthToken: `Bearer ${bearerToken}` };
  } else {
    window.location.href = "/login";
  }
};

const restoreAPI = (productionStoreId: string, productionId: string) => {
  const apimBaseUrl = process.env.REACT_APP_APIM_URL;

  const url = `${apimBaseUrl}/datamovement/production-stores/${productionStoreId}/productions/${productionId}/Restore`;
  return axios.post(url);
};

const makeOfflineAPI = (productionStoreId: string, productionId: string) => {
  const apimBaseUrl = process.env.REACT_APP_APIM_URL;

  const url = `${apimBaseUrl}/datamovement/production-stores/${productionStoreId}/productions/${productionId}/make-offline`;
  return axios.post(url);
};

const deleteAPI = (productionStoreId: string, productionId: string) => {
  const apimBaseUrl = process.env.REACT_APP_APIM_URL;

  const url = `${apimBaseUrl}/prodcontroller/production-stores/${productionStoreId}/productions/${productionId}`;
  return axios.delete(url);
};

const pollURL = (url: string) => {
  setAPIAuthentication();
  return axios.get(url);
};

export { restoreAPI, pollURL, makeOfflineAPI, deleteAPI };
