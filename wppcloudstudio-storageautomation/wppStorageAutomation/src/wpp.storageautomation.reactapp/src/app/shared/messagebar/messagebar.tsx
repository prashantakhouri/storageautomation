import * as React from "react";
import {
  MessageBar,
  MessageBarType,
  MessageBarButton,
  IIconProps,
  PrimaryButton,
} from "@fluentui/react";
const cancelIcon: IIconProps = { iconName: "Cancel" };
const OkayIcon: IIconProps = { iconName: "Accept" };

export const StatusMessage = (props: any) => (
  <MessageBar
    messageBarType={
      props.Type === "true"
        ? MessageBarType.success
        : props.Type === "false"
        ? MessageBarType.error
        : MessageBarType.warning
    }
    isMultiline={false}
    actions={
      props.Type == "warning" ? (
        <div>
          <MessageBarButton
            id="btn_Ok"
            iconProps={OkayIcon}
            onClick={function handleOff() {
              props.HandleWarning(props.SelectedItem, true, props.MenuAction);
            }}
          >
            OK
          </MessageBarButton>
          <MessageBarButton
            id="btn_Cancel"
            iconProps={cancelIcon}
            onClick={function handleOff() {
              props.HandleWarning(props.SelectedItem, false, props.MenuAction);
            }}
            selected={true}
          >
            Cancel
          </MessageBarButton>
        </div>
      ) : (
        <div></div>
      )
    }
  >
    {props.Message}
  </MessageBar>
);
