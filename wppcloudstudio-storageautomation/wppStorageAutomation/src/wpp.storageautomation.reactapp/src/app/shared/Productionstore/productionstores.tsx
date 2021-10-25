import React, { useEffect, useState } from "react";
import { listproductionstore } from "../../pages/production/createproduction.service";
import { useDispatch } from "react-redux";
import { getProductionStores } from "../../../redux/actions/action";
import { resolveProjectReferencePath } from "typescript";

export const LoadProductionStore: React.FunctionComponent = (props) => {
  const dispatch = useDispatch();
  const [prodstores, setProdstores] = useState([]);

  useEffect(() => {
    try {
      listproductionstore()
        .then((res: any) => {
          if (res.success === true) {
            const hasStores =
              res.data[0].productionStoreList.length > 0 ? "true" : "false";
            localStorage.setItem("hasStores", hasStores);

            dispatch(getProductionStores(res.data[0].productionStoreList));
          } else {
          }
        })
        .catch((err) => {
          localStorage.setItem("hasStores", "false");
        });
    } catch (ex) {
      console.log(ex);
    }
  }, [prodstores]);

  return <div></div>;
};
