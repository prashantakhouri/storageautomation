import * as React from "react";
import { useState, useEffect } from "react";
import { StatusMessage } from "../../shared/messagebar/messagebar";
import { Label } from "@fluentui/react/lib/Label";
import "./createproduction.css";
import { Icon, IIconStyles } from "@fluentui/react/lib/Icon";
import { validInput } from "../../shared/utils/regex";
import { memoizeFunction } from "@fluentui/react/lib/Utilities";
import { CREATE_PRODUCTION } from "../../shared/utils/constants";
import { createproduction } from "./createproduction.service";
import { ILabelStyles, ILabelStyleProps } from "@fluentui/react/lib/Label";
import {
  ITextFieldStyleProps,
  ITextFieldStyles,
  TextField,
} from "@fluentui/react/lib/TextField";
import {
  Stack,
  IStackProps,
  IStackTokens,
  IStackStyles,
} from "@fluentui/react/lib/Stack";
import { IIconProps } from "@fluentui/react";
import { getTheme, ITheme } from "@fluentui/react/lib/Styling";
import { Text } from "@fluentui/react/lib/Text";
import { PrimaryButton } from "@fluentui/react/lib/Button";
import { Layout } from "../../shared/layout/layout";
import { LoadingSpinner } from "../../shared/spinner/spinner";
import { SpinnerSize } from "@fluentui/react/lib/Spinner";

const saveIcon: IIconProps = { iconName: "Save" };
const cancelIcon: IIconProps = { iconName: "Cancel" };

const horizontalStackProps: IStackProps = {
  horizontal: true,
  styles: { root: { overflow: "hidden", width: "100%" } },
  tokens: { childrenGap: 50 },
};
const verticalStackProps: IStackProps = {
  styles: { root: { overflow: "hidden", width: "100%" } },
  tokens: { childrenGap: 30 },
};

const horizontalButtonStackProps: IStackProps = {
  horizontal: true,
  styles: { root: { marginTop: "13px" } },
  tokens: { childrenGap: 20, padding: "m 70px" },
};

const columnProps: Partial<IStackProps> = {
  tokens: { childrenGap: 15 },
  styles: { root: { width: 400 } },
};
const theme = getTheme();
const getDescriptionStyles = memoizeFunction((theme: ITheme) => ({
  root: { color: "#29e6ff" },
}));

function getStyles(props: ITextFieldStyleProps): Partial<ITextFieldStyles> {
  const { hasErrorMessage } = props;

  return {
    fieldGroup: [
      hasErrorMessage && {
        borderColor: "#ffa6a6",
        selectors: {
          "::after, &:focus, &:hover": {
            borderColor: "#ffa6a6",
          },
        },
      },
    ],
    errorMessage: {
      color: "#ffa6a6",
    },
    subComponentStyles: {
      label: getLabelStyles,
    },
  };
}

function getLabelStyles(props: ILabelStyleProps): ILabelStyles {
  const { required } = props;
  return {
    root: required && {
      selectors: {
        "::after": {
          color: "#ffa6a6",
        },
      },
    },
  };
}

export const CreateProduction: React.FunctionComponent = (props: any) => {
  const [productionName, setproductionName] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const [isInputValid, setisInputValid] = useState(false);
  const [statusMessage, setstatusMessage] = useState("");
  const [status, setStatus] = useState("");
  const [productionStoreId, setProductionStoreId] = useState<string | null>(
    null
  );
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const selectedProductionStoreid =
      localStorage.getItem("seletedItem") !== null &&
      localStorage.getItem("seletedItem") !== undefined
        ? localStorage.getItem("seletedItem")
        : "Test";

    setProductionStoreId(selectedProductionStoreid);
  }, []);

  const validate = (inputValue: any) => {
    let isValid =
      inputValue.trim() !== "" &&
      inputValue.trim() !== undefined &&
      inputValue.trim().length >= 8
        ? true
        : false;
    setErrorMessage("");
    setisInputValid(isValid);

    if (inputValue.trim() === "") {
      setErrorMessage(CREATE_PRODUCTION.EMPTY_ERROR);
    }

    if (inputValue.trim() !== "" && inputValue.trim().length < 8) {
      setErrorMessage(CREATE_PRODUCTION.MIN_LIMIT);
    }

    if (!validInput.test(inputValue.trim())) {
      setisInputValid(false);
      setErrorMessage(CREATE_PRODUCTION.SPECIAL_CHARS_ERROR);
    }
    return errorMessage;
  };

  const richErrorIconStyles: Partial<IIconStyles> = {
    root: { color: "#29e6ff" },
  };
  const richErrorStackStyles: Partial<IStackStyles> = { root: { height: 24 } };
  const richErrorStackTokens: IStackTokens = { childrenGap: 8 };

  const getRichErrorMessage = (value: string) => {
    return value.length === 255 ? (
      <Stack
        styles={richErrorStackStyles}
        verticalAlign="center"
        horizontal
        tokens={richErrorStackTokens}
      >
        <Icon iconName="Info" styles={richErrorIconStyles} />
        <Text variant="smallPlus" styles={getDescriptionStyles(theme)}>
          {CREATE_PRODUCTION.MAX_LIMIT}
        </Text>
      </Stack>
    ) : (
      validate(value)
    );
  };

  function handleSubmit(e: any) {
    setLoading(true);
    e.preventDefault();
    validate(productionName);
    setisInputValid(false);
    try {
      if (isInputValid) {
        createproduction(productionStoreId, productionName)
          .then(
            (response) => {
              setLoading(false);
              if (response.data.success === true) {
                setStatus("true");
                setstatusMessage(
                  `${productionName} ${CREATE_PRODUCTION.CREATE_PRODUCTION_SUCCESS}`
                );
              } else {
                setStatus("false");

                if (
                  response.data.error.includes("Security Identifiers not found")
                ) {
                  console.log(response.data.error);
                  setstatusMessage(CREATE_PRODUCTION.SECURITY_IDENTIFIER);
                  return;
                }

                if (
                  response.data.error.includes("Duplicate name not allowed")
                ) {
                  setstatusMessage(CREATE_PRODUCTION.DUPLICATE_NAME);
                } else {
                  setstatusMessage(response.data.error);
                }
              }
            },
            (response) => {
              setLoading(false);
              setStatus("false");
              console.log(response.message);
              if (
                response.message.includes("403") ||
                response.message.includes("404")
              ) {
                if (
                  response.response.data.error != null &&
                  response.response.data.error != ""
                ) {
                  setstatusMessage(response.response.data.error);
                } else {
                  setstatusMessage(
                    `You are not authorized to perform any action for ${response.response.data.name} Production store.`
                  );
                }
              } else {
                setstatusMessage(CREATE_PRODUCTION.ERROR_WENTWRONG);
              }
            }
          )
          .catch((err) => {
            setStatus("false");
            setstatusMessage(err);
          });
      }
    } catch (ex) {
      console.log(ex);
      setstatusMessage(CREATE_PRODUCTION.ERROR_WENTWRONG);
    }
  }

  const setProduction = (e: any) => {
    validate(e.target.value);
    setproductionName(e.target.value);
  };

  return (
    <Layout>
      <Stack {...verticalStackProps}>
        <Stack>
          <Label className="LabelFont">Create Production</Label>
        </Stack>
        <Stack {...verticalStackProps}>
          {statusMessage !== "" && (
            <StatusMessage Message={statusMessage} Type={status} />
          )}
        </Stack>
        {loading ? (
          <LoadingSpinner
            spinnerLabel="Creating..."
            spinnerSize={SpinnerSize.small}
          />
        ) : (
          <div />
        )}
        <Stack {...horizontalStackProps}>
          <Stack {...columnProps}>
            <TextField
              label="Production Name"
              value={productionName}
              styles={getStyles}
              id="txt_ProductionName"
              maxLength={255}
              minLength={8}
              onChange={setProduction}
              onBlur={setProduction}
              required
              onGetErrorMessage={getRichErrorMessage}
            />

            <TextField
              label="Template"
              name="template"
              id="txt_DefaultTemplate"
              readOnly
              placeholder="Default Template"
            />
          </Stack>
          <Stack {...horizontalButtonStackProps}>
            <PrimaryButton
              iconProps={cancelIcon}
              id="btn_Discard"
              text="Discard"
              onClick={() => (window.location.href = `/${productionStoreId}`)}
            />
            <PrimaryButton
              iconProps={saveIcon}
              text="Save"
              id="btn_Save"
              disabled={!isInputValid}
              onClick={handleSubmit}
            />
          </Stack>
        </Stack>
      </Stack>
    </Layout>
  );
};
