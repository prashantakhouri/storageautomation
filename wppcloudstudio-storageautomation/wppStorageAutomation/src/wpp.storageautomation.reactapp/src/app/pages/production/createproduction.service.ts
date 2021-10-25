import axios from "axios";

const getDefaultTemplate = (productionname: string) => {
  return {
    productionStoreUri: "//productionStore/",
    productionName: productionname,
    directoryTree: [
      {
        type: "folder",
        name: "production",
        path: "/{productionToken}",
        subitems: [
          {
            type: "folder",
            name: "rawAssets",
            path: "/raw assets",
            subitems: [
              {
                type: "folder",
                name: "images",
                path: "/Image",
                subitems: [],
              },
              {
                type: "folder",
                name: "video",
                path: "/Video",
                subitems: [],
              },
            ],
          },
        ],
      },
    ],
    tokens: [
      {
        productionToken: productionname,
      },
    ],
  };
};

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

const createproduction = (productionStoreId: any, productionName: string) => {
  const apimBaseUrl = process.env.REACT_APP_APIM_URL;
  const url = `${apimBaseUrl}/prodcontroller/production-stores/${productionStoreId}/productions/`;
  const createPro = getDefaultTemplate(productionName);

  //set Autentication before API call
  setAPIAuthentication();
  return axios.post(url, JSON.stringify(createPro));
};

const listproduction = async (productionStoreId: string) => {
  const apimBaseUrl = process.env.REACT_APP_APIM_URL;
  const url = `${apimBaseUrl}/prodcontroller/production-stores/${productionStoreId}/productions`;

  //set Autentication before API call
  setAPIAuthentication();
  //return axios.get(url);
  return new Promise((resolve, reject) => {
    axios
      .get(url)
      .then((res) => {
        resolve(res);
      })
      .catch((err) => reject(err));
  });
};

const listproductionstore = async () => {
  const apimBaseUrl = process.env.REACT_APP_APIM_URL;
  const url = `${apimBaseUrl}/productionstore/production-stores`;

  //set Autentication before API call
  setAPIAuthentication();
  // return await axios.get(url);
  return new Promise((resolve, reject) => {
    axios
      .get(url)
      .then((res) => {
        resolve(res.data);
      })
      .catch((err) => reject(err));
  });
};

export {
  getDefaultTemplate,
  createproduction,
  listproduction,
  listproductionstore,
};
