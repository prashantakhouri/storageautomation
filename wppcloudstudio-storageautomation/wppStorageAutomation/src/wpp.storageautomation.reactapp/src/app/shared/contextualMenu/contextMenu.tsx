import * as React from "react";
import { IconButton, IButtonStyles } from "@fluentui/react/lib/Button";
import { Link } from "@fluentui/react/lib/Link";
import {
  IOverflowSetItemProps,
  OverflowSet,
} from "@fluentui/react/lib/OverflowSet";
// import { IListProductionItem } from "../../pages/production/list-production";
// import { IDetailsListProps } from "@fluentui/react";
// import axios from "axios";
// import { AnyARecord, AnyCnameRecord, AnyPtrRecord } from "dns";

// const noOp = () => undefined;

const onRenderItem = (item: IOverflowSetItemProps): JSX.Element => {
  return (
    <Link
      role="menuitem"
      styles={{ root: { marginRight: 10 } }}
      onClick={item.onClick}
    >
      {item.name}
    </Link>
  );
};

const onRenderOverflowButton = (
  overflowItems: any[] | undefined
): JSX.Element => {
  const buttonStyles: Partial<IButtonStyles> = {
    root: {
      minWidth: 0,
      padding: "0 4px",
      alignSelf: "stretch",
      height: "auto",
    },
  };
  return (
    <IconButton
      title="More options"
      role="menuitem"
      styles={buttonStyles}
      id="btn_MoreOptions"
      menuIconProps={{ iconName: "MoreVertical" }}
      menuProps={{ items: overflowItems! }}
    />
  );
};

export class ContextMenu extends React.Component<any, any> {
  eventClicked = (menuitem: string) => {
    this.props.handleContextMenu(menuitem, this.props.selectedRow);
  };
  managerMenu = (
    itemStatus: string,
    deleteDisable: boolean,
    disableStatus: boolean
  ) => [
    {
      key: "restore",
      name: `Make ${itemStatus}`,
      onClick: () => this.eventClicked(`Make${itemStatus}`),
      id: `btn_Make${itemStatus}`,
      disabled: disableStatus,
    },
    {
      key: "rename",
      name: "Rename",
      disabled: true,
      id: "btn_Rename",
    },
    {
      key: "delete",
      name: "Delete",
      disabled: deleteDisable,
      onClick: () => this.eventClicked("Delete"),
      id: "btn_Delete",
    },
  ];
  userMenu = (itemStatus: string, disableStatus: boolean) => [
    {
      key: "restore",
      name: `Make ${itemStatus}`,
      onClick: () => this.eventClicked(`Make${itemStatus}`),
      id: `btn_Make${itemStatus}`,
      disabled: disableStatus,
    },
    {
      key: "rename",
      name: "Rename",
      disabled: true,
      id: "btn_Rename",
    },
  ];
  public render(): JSX.Element {
    {
      let itemStatus = "";
      let deleteDisable = true;
      let isManager = false;
      let disableStatus = true;

      if (this.props.selectedRow.status === "Online") {
        deleteDisable = false;
        itemStatus = "Offline";
        disableStatus = false;
      }
      if (this.props.selectedRow.status === "Offline") {
        itemStatus = "Online";
        deleteDisable = false;
        disableStatus = false;
      }
      if (this.props.selectedRow.isManager) {
        isManager = true;
      }

      return (
        <OverflowSet
          aria-label="Productions Context Menu"
          role="menubar"
          items={[
            {
              key: "menu",
              name: "",
            },
          ]}
          overflowItems={
            isManager
              ? this.managerMenu(itemStatus, deleteDisable, disableStatus)
              : this.userMenu(itemStatus, disableStatus)
          }
          onRenderOverflowButton={onRenderOverflowButton}
          onRenderItem={onRenderItem}
        />
      );
    }
  }
}
