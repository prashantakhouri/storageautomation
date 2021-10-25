import React, { useState, useEffect } from "react";
import { IStackTokens, Stack } from "@fluentui/react";
import { Stepper } from "../stepper/stepper";
import { Header } from "../header/header";
import { IStackProps } from "@fluentui/react/lib/Stack";
import { listproductionstore } from "../../pages/production/createproduction.service";
import { useDispatch } from "react-redux";
import { getProductionStores } from "../../../redux/actions/action";
import {
  Shimmer,
  ShimmerElementType,
  ShimmerElementsGroup,
} from "@fluentui/react";

const stackTokens: IStackTokens = {
  childrenGap: "0.2%",
  padding: "m 20px",
};
const mainContentProps: IStackProps = {
  styles: { root: { overflow: "hidden", height: "83vh" } },
};

const customStepperTokens: IStackProps = {
  styles: { root: { overflow: "hidden", width: "15%" } },
};
const customSpacingStackTokens: IStackProps = {
  padding: "m 40px",
  styles: { root: { overflow: "hidden", width: "85%" } },
};

export const Layout: React.FunctionComponent = ({ children }) => {
  const dispatch = useDispatch();
  const [loadingProductionStores, setLoadingProductionStores] = useState(true);
  const [hasProductions, setHasProductions] = useState(true);

  useEffect(() => {
    try {
      const fetchProductionStores = async () => {
        await listproductionstore()
          .then((res: any) => {
            if (res.success === true) {
              dispatch(getProductionStores(res.data[0].productionStoreList));
              const hasData =
                res.data[0].productionStoreList.length > 0 ? true : false;
              setHasProductions(hasData);
              setLoadingProductionStores(false);
            } else {
            }
          })
          .catch((err) => {
            setHasProductions(false);
            console.log(err);
          });
      };

      fetchProductionStores();
    } catch (ex) {
      setHasProductions(false);
      console.log(ex);
    }
  });

  const getCustomStepperShimmer = (): JSX.Element => {
    return (
      <div>
        <ShimmerElementsGroup
          shimmerElements={[
            { type: ShimmerElementType.gap, width: "1%", height: 40 },
            { type: ShimmerElementType.line, width: "98%", height: 14 },
            { type: ShimmerElementType.gap, width: "1%", height: 40 },
          ]}
        />
        <ShimmerElementsGroup
          shimmerElements={[
            { type: ShimmerElementType.gap, width: "25%", height: 45 },
            { type: ShimmerElementType.line, width: "74%", height: 14 },
            { type: ShimmerElementType.gap, width: "1%", height: 45 },
          ]}
        />
        <ShimmerElementsGroup
          shimmerElements={[
            { type: ShimmerElementType.gap, width: "25%", height: 45 },
            { type: ShimmerElementType.line, width: "74%", height: 14 },
            { type: ShimmerElementType.gap, width: "1%", height: 45 },
          ]}
        />
        <ShimmerElementsGroup
          shimmerElements={[
            { type: ShimmerElementType.gap, width: "25%", height: 45 },
            { type: ShimmerElementType.line, width: "74%", height: 14 },
            { type: ShimmerElementType.gap, width: "1%", height: 45 },
          ]}
        />
        <ShimmerElementsGroup
          shimmerElements={[
            { type: ShimmerElementType.gap, width: "1%", height: 45 },
            { type: ShimmerElementType.line, width: "98%", height: 14 },
            { type: ShimmerElementType.gap, width: "1%", height: 45 },
          ]}
        />
        <ShimmerElementsGroup
          shimmerElements={[
            { type: ShimmerElementType.gap, width: "1%", height: 45 },
            { type: ShimmerElementType.line, width: "98%", height: 14 },
            { type: ShimmerElementType.gap, width: "1%", height: 45 },
          ]}
        />
        <ShimmerElementsGroup
          shimmerElements={[
            { type: ShimmerElementType.gap, width: "1%", height: 45 },
            { type: ShimmerElementType.line, width: "98%", height: 14 },
            { type: ShimmerElementType.gap, width: "1%", height: 45 },
          ]}
        />
      </div>
    );
  };

  return (
    <Stack tokens={stackTokens}>
      <Header />
      <Stack horizontal {...mainContentProps}>
        <Stack {...customStepperTokens}>
          {loadingProductionStores && hasProductions ? (
            <Shimmer customElementsGroup={getCustomStepperShimmer()} />
          ) : (
            <Stepper />
          )}
        </Stack>
        <Stack {...customSpacingStackTokens}>
          <main> {children}</main>
        </Stack>
      </Stack>
    </Stack>
  );
};
