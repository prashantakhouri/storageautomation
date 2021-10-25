import {
  Spinner,
  SpinnerSize,
  ISpinnerStyleProps,
  ISpinnerStyles,
} from "@fluentui/react/lib/Spinner";
import { memoizeFunction } from "@fluentui/react/lib/Utilities";
import {
  HighContrastSelector,
  getHighContrastNoAdjustStyle,
  getGlobalClassNames,
  keyframes,
} from "@fluentui/react/lib/Styling";
import { IStackProps, Stack } from "@fluentui/react/lib/Stack";

export const LoadingSpinner = (props : any ) => {
  // This is just for laying out the label and spinner (spinners don't have to be inside a Stack)
  const rowProps: IStackProps = { horizontal: true, verticalAlign: "center" };

  const tokens = {
    sectionStack: {
      childrenGap: 10,
    },
    spinnerStack: {
      childrenGap: 20,
    },
  };

  const GlobalClassNames = {
    root: "ms-Spinner",
    circle: "ms-Spinner-circle",
    label: "ms-Spinner-label",
  };
  const spinAnimation = memoizeFunction(() =>
    keyframes({
      "0%": {
        transform: "rotate(0deg)",
      },
      "100%": {
        transform: "rotate(360deg)",
      },
    })
  );
  const getStyles = (props: ISpinnerStyleProps): ISpinnerStyles => {
    const { theme, size, className, labelPosition } = props;

    const { palette } = theme;

    const classNames = getGlobalClassNames(GlobalClassNames, theme);

    return {
      root: [
        classNames.root,
        {
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          justifyContent: "center",
        },
        labelPosition === "top" && {
          flexDirection: "column-reverse",
        },
        labelPosition === "right" && {
          flexDirection: "row",
        },
        labelPosition === "left" && {
          flexDirection: "row-reverse",
        },
        className,
      ],
      circle: [
        classNames.circle,
        {
          boxSizing: "border-box",
          borderRadius: "50%",
          border: "2.2px solid " + palette.themePrimary,
          borderTopColor: palette.neutralPrimary,
          animationName: spinAnimation(),
          animationDuration: "1.3s",
          animationIterationCount: "infinite",
          animationTimingFunction: "cubic-bezier(.53,.21,.29,.67)",
          selectors: {
            [HighContrastSelector]: {
              borderTopColor: "Highlight",
              ...getHighContrastNoAdjustStyle(),
            },
          },
        },
        
        size === SpinnerSize.small && [
          "ms-Spinner--small",
          {
            width: 16,
            height: 16,
          },
        ],
        size === SpinnerSize.medium && [
          "ms-Spinner--medium",
          {
            width: 20,
            height: 20,
          },
        ],
        size === SpinnerSize.large && [
          "ms-Spinner--large",
          {
            width: 40,
            height: 40,
          },
        ],
      ],
      label: [
        classNames.label,
        theme.fonts.small,
        {
          color: palette.themePrimary,
          margin: "8px 0 0",
          textAlign: "center",
        },
        labelPosition === "top" && {
          margin: "0 0 8px",
        },
        labelPosition === "right" && {
          margin: "0 0 0 8px",
        },
        labelPosition === "left" && {
          margin: "0 8px 0 0",
        },
      ],
    };
  };
  return (
    <Stack tokens={tokens.sectionStack}>
      <Stack {...rowProps} tokens={tokens.spinnerStack}>
        {/* <Label>Large spinner</Label> */}
        <Spinner size={props.spinnerSize} styles={getStyles} label={props.spinnerLabel} labelPosition ="right" />
      </Stack>
    </Stack>
  );
};
