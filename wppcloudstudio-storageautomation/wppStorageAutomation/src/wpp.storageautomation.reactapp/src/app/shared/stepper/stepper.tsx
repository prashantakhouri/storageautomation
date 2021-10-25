import React from "react";
import { navStyles } from "./stepper.component";
import { Nav, INavLink } from "@fluentui/react/lib/Nav";
import { Stack, IStackProps } from "@fluentui/react/lib/Stack";
import "./stepper.css";
import { INavLinkGroup } from "@fluentui/react/lib/Nav";
import { useSelector } from "react-redux";
import { IListProductionStoreItem } from "../../models/IListProductionStoreItem";

const verticalStackProps: IStackProps = {
  styles: { root: { overflow: "auto", height: "100%" } },
  tokens: { childrenGap: 20 },
};

export const Stepper: React.FunctionComponent = (props: any) => {
  
  const productionStores: IListProductionStoreItem[] = useSelector(
    (state: any) => state.productionStores.productionStores
  );

  const navLinkGroups: INavLinkGroup[] = [
    {
      links: [],
    },
  ];
  let regionLinks: INavLink[] = navLinkGroups[0].links;

  var routeParam = window.location.pathname;

  const defaultKey = "Key2";
  const selectedItem =
    localStorage.getItem("seletedItem") !== null &&
    localStorage.getItem("seletedItem") !== undefined
      ? localStorage.getItem("seletedItem")
      : defaultKey;

  productionStores.forEach(function (v, i) {
    if (i === 0) {
      var storeId = v["id"];
      if (routeParam === "/home") {
        localStorage.setItem("seletedItem", storeId);
        localStorage.setItem("selectedItemRegion", v["region"]);
        window.location.href = `/${storeId}`;
      }
    }
    const prodStoreLink: INavLink = {
      key: v["id"],
      name: v["name"],
      url: "",
      links: [],
      isExpanded: true,
      target: "_blank",
      id: `btn_${v["name"]}`,
    };

    if (regionLinks.filter((item) => item.name === v["region"]).length === 0) {
      regionLinks.push({
        name: v["region"],
        url: "",
        links: [],
        isExpanded:
          v["region"] == localStorage.getItem("selectedItemRegion")
            ? true
            : false,
        target: "_blank",
        id: `li_${v["region"]}`,
      });
    }

    var region: INavLink[] = regionLinks.filter(
      (item) => item.name === v["region"]
    );
    if (region.length > 0) {
      region[0].links?.push(prodStoreLink);
    }
  });

  async function _onLinkClick(
    ev?: React.MouseEvent<HTMLElement>,
    item?: INavLink
  ) {
    if (item?.key) {
      localStorage.setItem("seletedItem", item?.key);
      navLinkGroups[0].links.forEach(function (v, i) {
        v.links?.forEach(function (w, j) {
          if (w.key == item?.key) {
            localStorage.setItem("selectedItemRegion", v.name);
          }
        });
      });
      window.location.href = `/${item?.key}`;
    }
  }
  
  return (
    <Stack {...verticalStackProps}>
      <Nav
        onLinkClick={_onLinkClick}
        selectedKey={
          selectedItem !== null && selectedItem !== undefined
            ? selectedItem
            : defaultKey
        }
        ariaLabel="Nav bar"
        styles={navStyles}
        groups={navLinkGroups}
      />
    </Stack>
  );
};
