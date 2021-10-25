import React from "react";
import { Announced } from "@fluentui/react/lib/Announced";
import { TextField, ITextFieldStyles } from "@fluentui/react/lib/TextField";
import { listproductionstore } from "../production/createproduction.service";
import {
  IDetailsListStyles,
  DetailsListLayoutMode,
  Selection,
  IColumn,
  CheckboxVisibility,
} from "@fluentui/react/lib/DetailsList";
import { MarqueeSelection } from "@fluentui/react/lib/MarqueeSelection";
import { mergeStyles } from "@fluentui/react/lib/Styling";
import "./createproduction.css";
import { Label, IIconProps } from "@fluentui/react";
import { IconButton } from "@fluentui/react/lib/Button";
import { Stack, IStackProps } from "@fluentui/react/lib/Stack";
import { listproduction } from "./createproduction.service";
import "./createproduction.css";
import { ContextMenu } from "../../shared/contextualMenu/contextMenu";
import {
  pollURL,
  restoreAPI,
  makeOfflineAPI,
  deleteAPI,
} from "./productionmenu.service";
import { StatusMessage } from "../../shared/messagebar/messagebar";
import {
  Restore_Prod,
  MakeOffline_Prod,
  Delete_Prod,
} from "../../shared/utils/constants";
import { Layout } from "../../shared/layout/layout";
import { CREATE_PRODUCTION } from "../../shared/utils/constants";
import { LoadingSpinner } from "../../shared/spinner/spinner";
import { SpinnerSize } from "@fluentui/react/lib/Spinner";
import { ShimmeredDetailsList } from "@fluentui/react/lib/ShimmeredDetailsList";
import { darkTheme } from "../../themes";

const addIcon: IIconProps = { iconName: "Add" };
const refreshIcon: IIconProps = { iconName: "Refresh" };
const cancelIcon: IIconProps = { iconName: "Cancel" };
const filterIcon: IIconProps = { iconName: "Filter" };

const horizontalStackAddFilterProps: IStackProps = {
  horizontal: true,
  horizontalAlign: "end",
  styles: {
    root: {
      overflow: "hidden",
      width: "100%",
      verticalAlign: "center",
    },
  },
  tokens: { childrenGap: 10 },
};

const horizontalStackAddProps: IStackProps = {
  horizontal: true,
  styles: {
    root: {
      overflow: "hidden",
      verticalAlign: "center",

      marginRight: "20px",
      marginTop: "5px",
      marginLeft: "20px",
      marginBottom: "5px",
    },
  },
  tokens: { childrenGap: 5 },
};
const horizontalStackSearchProps: IStackProps = {
  horizontal: true,
  horizontalAlign: "end",
  styles: {
    root: {
      overflow: "hidden",
      width: "100%",
      verticalAlign: "center",
    },
  },
  tokens: { childrenGap: 15 },
};

const horizontalStackSeachChildProps: IStackProps = {
  horizontal: true,
  styles: {
    root: {
      overflow: "hidden",
      verticalAlign: "center",
      marginRight: "20px",
      marginTop: "5px",
      marginLeft: "20px",
      marginBottom: "5px",
    },
  },
  tokens: { childrenGap: 5 },
};
const verticalStackProps: IStackProps = {
  styles: { root: { overflow: "hidden", width: "100%" } },
  tokens: { childrenGap: 20 },
};
const childVerticalStackProps: IStackProps = {
  styles: { root: { overflow: "hidden", width: "100%", height: "80%" } },
  tokens: { childrenGap: 4 },
};

const textFieldClass = mergeStyles({
  display: "block",
  marginBottom: "10px",
});

const textFieldStyles: Partial<ITextFieldStyles> = {
  root: { maxWidth: "300px", minHeight: "22px", maxHeight: "25px" },
};
const gridStyles: Partial<IDetailsListStyles> = {
  root: {
    selectors: {
      "& [role=grid]": {
        display: "flex",
        flexDirection: "column",
        alignItems: "start",
        height: "68vh",
      },
      "& [role=row]": {
        background: "transparent",
      },
      "& [data-icon-name=SortUp] , [data-icon-name=SortDown]": {
        color: darkTheme.palette.themePrimary,
      },
    },
  },
  headerWrapper: {
    flex: "0 0 auto",
  },
  contentWrapper: {
    flex: "1 1 auto",
    overflowY: "auto",
    overflowX: "hidden",
  },
};

const emptyGridStyles: Partial<IDetailsListStyles> = {
  root: {
    selectors: {
      "& [role=grid]": {
        display: "flex",
        flexDirection: "column",
        alignItems: "start",
        overflowY: "auto",
        overflowX: "hidden",
      },
      "& [role=row]": {
        background: "transparent",
      },
      "& [data-icon-name=SortUp] , [data-icon-name=SortDown]": {
        color: darkTheme.palette.themePrimary,
      },
    },
  },
  headerWrapper: {
    flex: "0 0 auto",
  },
  contentWrapper: {
    flex: "1 1 auto",
    overflowY: "auto",
    overflowX: "hidden",
  },
};

const shimmerGridStyles: Partial<IDetailsListStyles> = {
  root: {
    selectors: {
      "& [role=presentation]": {
        background: "transparent",
      },
    },
  },
};

export interface IListProductionItem {
  id: string;
  name: string;
  wipurl: string;
  productionStoreId: string;
  status: string;
  createdDate: Date;
  modifiedDate: Date;
  sizeInBytes: string;
  isManager: boolean;
}

export interface IListProductionState {
  items: IListProductionItem[];
  selectionDetails: string;
  setstatusMessage: string;
  setStatus: string;
  showHideFilter: boolean;
  contextMenuItem: string;
  selectedProductionStoreId: string;
  selectedProductionStoreName: string;
  selectedItem: any;
  statusCode: number;
  showGridPanel: boolean;
  loadingRestoreResponse: boolean;
  loadingMakeOfflineResponse: boolean;
  loadingListProduction: boolean;
  sortByColumnName: string;
  sortIsDescending: boolean;
  filterText: string;
  hasStoresAccess: boolean;
  //selectedRow: any;
}

export class ListProduction extends React.Component<{}, IListProductionState> {
  private _selection: Selection;
  private _allItems: IListProductionItem[];
  private _columns: IColumn[];
  private params: any;

  constructor(props: {}) {
    super(props);

    this.handleContextMenuEvent = this.handleContextMenuEvent.bind(this);
    this.handleWarning = this.handleWarning.bind(this);

    this._selection = new Selection({
      onSelectionChanged: () =>
        this.setState({ selectionDetails: this._getSelectionDetails() }),
    });

    //Populate with items for demos.
    this._allItems = [];
    this.params = this.props;

    const id = this.params.match.params.id;
    localStorage.setItem("seletedItem", id);

    this.state = {
      items: this._allItems,
      selectionDetails: this._getSelectionDetails(),
      setstatusMessage: "",
      setStatus: "",
      showHideFilter: false,
      contextMenuItem: "",
      selectedProductionStoreId: id,
      selectedProductionStoreName: "",
      selectedItem: "",
      statusCode: 0,
      showGridPanel: true,
      loadingRestoreResponse: false,
      loadingMakeOfflineResponse: false,
      loadingListProduction: true,
      sortByColumnName: "name",
      sortIsDescending: false,
      filterText: "",
      hasStoresAccess: false,
    };
    this._columns = this.getColumns();
  }

  async componentDidMount() {
    this.validateProductionStoreId(this.state.selectedProductionStoreId);
    await this.getProductions(
      this.state.selectedProductionStoreId,
      this.state.sortByColumnName,
      this.state.sortIsDescending
    );
  }

  handleWarning = (selectedItem: any, action: boolean, menuitem: string) => {
    this.setState({ setstatusMessage: "" });
    if (action) {
      if (menuitem === "MakeOffline") {
        this.makeProductionOffline(
          selectedItem.productionStoreId,
          selectedItem.id
        );
      }
      if (menuitem === "Delete") {
        this.deleteProduction(selectedItem.productionStoreId, selectedItem.id);
      }
    }
  };

  handleContextMenuEvent = (menuitem: string, selectedRow: any) => {
    this.setState({ contextMenuItem: menuitem });
    this.setState({ setstatusMessage: "" });
    switch (menuitem) {
      case "MakeOnline": {
        this.restoreProduction(selectedRow.productionStoreId, selectedRow.id);
        break;
      }
      case "MakeOffline": {
        this.setState({ setStatus: "warning" });
        this.setState({
          setstatusMessage: `Are you sure you want to make ${selectedRow.name} offline?`,
        });
        this.setState({ selectedItem: selectedRow });
        break;
      }
      case "Delete": {
        this.setState({ setStatus: "warning" });
        this.setState({
          setstatusMessage: `Are you sure you want to delete ${selectedRow.name}?`,
        });
        this.setState({ selectedItem: selectedRow });
        break;
      }
      case "rename": {
        break;
      }
      default: {
        break;
      }
    }
  };

  private _onColumnClick = (
    ev: React.MouseEvent<HTMLElement>,
    column: IColumn
  ): void => {
    const newColumns: IColumn[] = this._columns.slice();
    const currColumn: IColumn = newColumns.filter(
      (currCol) => column.key === currCol.key
    )[0];
    this.setState({ sortByColumnName: currColumn.fieldName! });
    newColumns.forEach((newCol: IColumn) => {
      if (newCol === currColumn) {
        currColumn.isSortedDescending = !currColumn.isSortedDescending;
        currColumn.isSorted = true;
        this.setState({ sortIsDescending: currColumn.isSortedDescending });
      } else {
        newCol.isSorted = false;
        newCol.isSortedDescending = true;
      }
    });
    const newItems = _copyAndSort(
      this.state.items,
      currColumn.fieldName!,
      currColumn.isSortedDescending
    );
    this.setState({
      //columns: newColumns,
      items: newItems,
    });
  };

  getColumns = () => {
    return [
      {
        key: "name",
        name: "Name",
        fieldName: "name",
        minWidth: 200,
        maxWidth: 400,
        isResizable: true,
        isSorted: true,
        isSortedDescending: false,
        sortAscendingAriaLabel: "Sorted A to Z",
        sortDescendingAriaLabel: "Sorted Z to A",
        onColumnClick: this._onColumnClick,
      },
      {
        key: "created",
        name: "Created",
        fieldName: "createdDateTime",
        minWidth: 200,
        maxWidth: 200,
        isResizable: true,
        isSorted: false,
        isSortedDescending: true,
        sortAscendingAriaLabel: "Sorted A to Z",
        sortDescendingAriaLabel: "Sorted Z to A",
        onColumnClick: this._onColumnClick,
        onRender: (item: any) => {
          return this.formatDate(item.createdDateTime);
        },
      },
      {
        key: "modified",
        name: "Modified",
        fieldName: "modifiedDateTime",
        minWidth: 200,
        maxWidth: 200,
        isResizable: true,
        isSorted: false,
        isSortedDescending: true,
        sortAscendingAriaLabel: "Sorted A to Z",
        sortDescendingAriaLabel: "Sorted Z to A",
        onColumnClick: this._onColumnClick,
        onRender: (item: any) => {
          return this.formatDate(item.modifiedDateTime);
        },
      },
      {
        key: "size",
        name: "Size",
        fieldName: "sizeInBytes",
        minWidth: 50,
        maxWidth: 50,
        isResizable: true,
        isSorted: false,
        isSortedDescending: true,
        sortAscendingAriaLabel: "Sorted A to Z",
        sortDescendingAriaLabel: "Sorted Z to A",
        onColumnClick: this._onColumnClick,
        onRender: (item: any) => {
          if (item.sizeInBytes == null) {
            return "0 B";
          }

          const unitUpperLimit = 1024;
          const conversionRate = 1024;

          if (item.sizeInBytes < unitUpperLimit) {
            return item.sizeInBytes + " B";
          }

          var sizeInKb = item.sizeInBytes / conversionRate;
          if (Math.round(sizeInKb) < unitUpperLimit) {
            return sizeInKb.toFixed(1).endsWith("0")
              ? sizeInKb.toFixed(0) + " KB"
              : sizeInKb.toFixed(1) + " KB";
          }

          var sizeInMb = sizeInKb / conversionRate;
          if (Math.round(sizeInMb) < unitUpperLimit) {
            return sizeInMb.toFixed(1).endsWith("0")
              ? sizeInMb.toFixed(0) + " MB"
              : sizeInMb.toFixed(1) + " MB";
          }

          var sizeInGb = sizeInMb / conversionRate;
          if (Math.round(sizeInGb) < unitUpperLimit) {
            return sizeInGb.toFixed(1).endsWith("0")
              ? sizeInGb.toFixed(0) + " GB"
              : sizeInGb.toFixed(1) + " GB";
          }

          var sizeInTb = sizeInGb / conversionRate;
          return sizeInTb.toFixed(1).endsWith("0")
            ? sizeInTb.toFixed(0) + " TB"
            : sizeInTb.toFixed(1) + " TB";
        },
      },
      {
        key: "status",
        name: "Status",
        fieldName: "status",
        minWidth: 100,
        maxWidth: 100,
        isResizable: true,
        isSorted: false,
        isSortedDescending: true,
        sortAscendingAriaLabel: "Sorted A to Z",
        sortDescendingAriaLabel: "Sorted Z to A",
        onColumnClick: this._onColumnClick,
        onRender: (item: any) => {
          if(item.status == 'MakingOffline')
          {
            return <LoadingSpinner
            spinnerLabel="Making Offline..."
            spinnerSize={SpinnerSize.small}
          />;
          }
          if(item.status == 'MakingOnline')
          {
            return <LoadingSpinner
            spinnerLabel="Restoring..."
            spinnerSize={SpinnerSize.small}
          />;
          }
          if(item.status == 'Archiving')
          {
            return "Online";
          }
          else
          {
            return item.status;
          }
        }
      },
      {
        key: "column6",
        name: "",
        fieldName: "",
        minWidth: 100,
        maxWidth: 200,
        isResizable: true,
        onRender: (item: any) => (
          <Stack>
            <ContextMenu
              handleContextMenu={this.handleContextMenuEvent}
              selectedRow={item}
            />
          </Stack>
        ),
      },
    ];
  };

  validateProductionStoreId = (productionStoreId: string) => {
    try {
      listproductionstore()
        .then((res: any) => {
          if (res.success !== true) {
            this.setErrorState(CREATE_PRODUCTION.SECURITY_NOTAUTHORIZED);
          } else {
            this.setState({ hasStoresAccess: true });
          }
        })
        .catch((err) => {
          console.log(err); //Logging the error message as per the requirement.
          if (err.message.includes("403") || err.message.includes("404")) {
            this.setErrorState(CREATE_PRODUCTION.SECURITY_NOTAUTHORIZED);
          } else {
            this.setErrorState(CREATE_PRODUCTION.ERROR_WENTWRONG);
          }
        });
    } catch (ex) {
      this.setErrorState(CREATE_PRODUCTION.ERROR_WENTWRONG);
    }
  };

  setErrorState = (errMsg: string) => {
    this.setState({ showGridPanel: false });
    this.setState({ setStatus: "false" });
    this.setState({
      setstatusMessage: errMsg,
    });
  };

  getProductions = async (
    productionStoreId: string,
    sortColumnName: string = "name",
    isSortDescending: boolean = false
  ) => {
    if (this.params.match.params.id !== "home") {
      try {
        await listproduction(productionStoreId).then(
          (response: any) => {
            this.setState({ loadingListProduction: false });
            if (response.data.success === true) {
              this._allItems = response.data.data[0].productionList;

              if (
                this._columns.filter((x) => x.fieldName == sortColumnName)
                  .length == 0
              ) {
                sortColumnName = "name";
              }
              this._allItems = _copyAndSort(
                this._allItems,
                sortColumnName,
                isSortDescending
              );
              this.setState({
                selectedProductionStoreName:
                  response.data.data[0].productionStore.name,
              });
              this.setState({ items: this._allItems });
            } else {
              this.setState({ setStatus: "false" });
              this.setState({ setstatusMessage: response.data.error });
            }
          },
          (response) => {
            console.log(response.message); //Logging the error message as per the requirement.
            if (
              response.message.includes("403") ||
              response.message.includes("404")
            ) {
              this.setState({ setStatus: "false" });
              this.setState({ showGridPanel: false });
              this.setState({ statusCode: response.response.status });
              if (
                response.response.data.error != null &&
                response.response.data.error != ""
              ) {
                this.setState({
                  setstatusMessage: response.response.data.error,
                });
              } else {
                this.setState({
                  setstatusMessage: `You are not authorized to view ${response.response.data.name} Production store.`,
                });
              }
            } else {
              this.setErrorState(CREATE_PRODUCTION.ERROR_WENTWRONG);
            }
          }
        );
      } catch (ex) {
        this.setErrorState(CREATE_PRODUCTION.ERROR_WENTWRONG);
      }
    }
  };

  formatDate = (item: any) => {
    if (item != null) {
      var date = this.convertUTCDateToLocalDate(item);
      let mdate = new Date(date);
      //let mdate = item;
      return new Intl.DateTimeFormat("en-GB", {
        year: "numeric",
        month: "long",
        day: "2-digit",
        hour: "2-digit",
        minute: "2-digit",
      }).format(mdate);
    }

    return item;
  };

  convertUTCDateToLocalDate = (date: string) => {
    var newDate = new Date(date);
    newDate.setMinutes(newDate.getMinutes() - newDate.getTimezoneOffset());
    return newDate;
  };

  restoreProduction = (productionStoreId: string, productionId: string) => {
    try {
      this.setState({ loadingRestoreResponse: true });
      restoreAPI(productionStoreId, productionId)
        .then((response: { data: { statusQueryGetUri: string } }) => {
          this.pollRestore(response.data.statusQueryGetUri);
        })
        .catch((err) => {
          this.setState({ setstatusMessage: err });
        });
    } catch (ex) {
      console.log(ex);
      this.setErrorState(CREATE_PRODUCTION.ERROR_WENTWRONG);
    }
  };

  makeProductionOffline = (productionStoreId: string, productionId: string) => {
    try {
      this.setState({ loadingMakeOfflineResponse: true });
      makeOfflineAPI(productionStoreId, productionId)
        .then((response: { data: { statusQueryGetUri: string } }) => {
          this.pollMakeOffline(response.data.statusQueryGetUri);
        })
        .catch((err) => {
          this.setState({ setstatusMessage: err });
        });
    } catch (ex) {
      this.setErrorState(CREATE_PRODUCTION.ERROR_WENTWRONG);
    }
  };
  pollMakeOffline = (url: string) => {
    try {
      pollURL(url)
        .then((res) => {
          if (res.data.runtimeStatus === "Completed") {
            this.setState({ loadingMakeOfflineResponse: false });
            var output = res.data.output;
            if (output.Success) {
              this.setState({ setStatus: "true" });
              this.setState({
                setstatusMessage: `${output.Data[0].Name} ${MakeOffline_Prod.Success}`,
              });
              this.componentDidMount();
            } else {
              if (output.Error.includes("with open files or folders")) {
                this.setState({ setStatus: "false" });
                this.setState({
                  setstatusMessage: MakeOffline_Prod.Error,
                });
                this.componentDidMount();
              } else {
                this.setState({ setStatus: "false" });
                this.setState({
                  setstatusMessage: MakeOffline_Prod.NotExists,
                });
                this.componentDidMount();
              }
            }
          } else {
            this.componentDidMount();
            this.pollMakeOffline(url);
          }
        })
        .catch((error) => {
          this.setState({ setStatus: "false" });
          this.setState({ setstatusMessage: error });
        });
    } catch (ex) {
      console.log(ex);
      this.setErrorState(CREATE_PRODUCTION.ERROR_WENTWRONG);
    }
  };
  pollRestore = (url: string) => {
    try {
      pollURL(url)
        .then((res) => {
          if (res.data.runtimeStatus === "Completed") {
            this.setState({ loadingRestoreResponse: false });
            var output = res.data.output;

            if (output.Success) {
              this.setState({ setStatus: "true" });
              this.setState({
                setstatusMessage: `${output.Data[0].Name} ${Restore_Prod.Success}`,
              });
              this.componentDidMount();
            } else {
              // var errorMsg = output.Error;

              if (output.Error.includes("does not exist in Archive")) {
                this.setState({ setStatus: "false" });
                this.setState({
                  setstatusMessage: Restore_Prod.NotExists,
                });
                this.componentDidMount();
              } else {
                this.setState({ setStatus: "false" });
                this.setState({ setstatusMessage: Restore_Prod.Error });
                this.componentDidMount();
              }
            }
          } else {
            this.componentDidMount();
            this.pollRestore(url);
          }
        })
        .catch((error) => {
          this.setState({ setStatus: "false" });
          this.setState({ setstatusMessage: error });
        });
    } catch (ex) {
      console.log(ex);
      this.setErrorState(CREATE_PRODUCTION.ERROR_WENTWRONG);
    }
  };

  deleteProduction = (productionStoreId: string, productionId: string) => {
    try {
      deleteAPI(productionStoreId, productionId)
        .then((response) => {
          if (response.data.success === true) {
            this.setState({ setStatus: "true" });
            this.setState({
              setstatusMessage: `${response.data.data[0].name} ${Delete_Prod.Success}`,
            });
            this.componentDidMount();
          } else {
            this.setState({ setStatus: "false" });
            this.setState({ setstatusMessage: Delete_Prod.Error });
          }
        })
        .catch((error) => {
          this.setState({ setStatus: "false" });
          this.setState({ setstatusMessage: Delete_Prod.ForbiddenError });
        });
    } catch (ex) {
      console.log(ex);
      this.setErrorState(CREATE_PRODUCTION.ERROR_WENTWRONG);
    }
  };

  private async refreshGrid() {
    this.setState({ setstatusMessage: "" });
    this.setState({loadingListProduction: true});
    await this.getProductions(
      this.state.selectedProductionStoreId,
      this.state.sortByColumnName,
      this.state.sortIsDescending
    );
    if (this.state.showHideFilter) {
      this._filterByText(this.state.filterText);
    }
  }

  private _onFilterTextChange = (
    ev: React.FormEvent<HTMLInputElement | HTMLTextAreaElement>,
    text?: string
  ): void => {
    this.setState({ filterText: text! });
    this._filterByText(text);
  };

  private _filterByText = (text?: string): void => {
    if (text != undefined && text.length >= 3) {
      this.setState({
        items: text
          ? this._allItems.filter(
              (i) => i.name.toLowerCase().indexOf(text.toLowerCase()) > -1
            )
          : this._allItems,
      });
    } else {
      this.setState({
        items: this._allItems,
      });
    }
  };

  private _onFilterBoxClose = (): void => {
    this.setState({
      items: this._allItems,
    });
    this.setState({ filterText: "" });
    this.setsearchVisibilty(false);
  };

  public render(): JSX.Element {
    const {
      items,
      selectionDetails,
      showHideFilter,
      showGridPanel,
      hasStoresAccess,
    } = this.state;

    return (
      <Layout>
        <Stack {...verticalStackProps}>
          <Stack>
            <Label className="LabelFont">
              {this.state.selectedProductionStoreName}
            </Label>
          </Stack>
          <Stack {...verticalStackProps}>
            {this.state.setstatusMessage !== "" && (
              <StatusMessage
                Message={this.state.setstatusMessage}
                Type={this.state.setStatus}
                SelectedItem={this.state.selectedItem}
                HandleWarning={this.handleWarning}
                MenuAction={this.state.contextMenuItem}
              />
            )}
          </Stack>
          {showGridPanel && (
            <Stack {...childVerticalStackProps}>
              <Announced message={selectionDetails} />
              {/*this.state.loadingRestoreResponse ? (
                <LoadingSpinner
                  spinnerLabel="Restoring..."
                  spinnerSize={SpinnerSize.small}
                />
              ) : (
                <span />
              )}
              {this.state.loadingMakeOfflineResponse ? (
                <LoadingSpinner
                  spinnerLabel="Making offline..."
                  spinnerSize={SpinnerSize.small}
                />
              ) : (
                <span />
              )*/}
              {showHideFilter}
              <Stack {...horizontalStackAddFilterProps}>
                <Stack {...horizontalStackAddProps} className="leftAlignClass">
                  <IconButton
                    iconProps={refreshIcon}
                    id="btn_Refresh"
                    onClick={() => this.refreshGrid()}
                  />
                  <Label>Refresh</Label>
                </Stack>
                <Stack {...horizontalStackAddProps}>
                  <IconButton
                    iconProps={addIcon}
                    id="btn_CreateProduction"
                    disabled={hasStoresAccess ? false : true}
                    onClick={() =>
                      (window.location.href = `create-production/${this.state.selectedProductionStoreId}`)
                    }
                  />
                  <Label>Create Production</Label>
                </Stack>
                <Stack {...horizontalStackAddProps}>
                  <IconButton
                    iconProps={filterIcon}
                    id="btn_FilterProduction"
                    onClick={() => this.setsearchVisibilty(true)}
                  />
                  <Label>Filters</Label>
                </Stack>
              </Stack>
              {showHideFilter && (
                <Stack {...horizontalStackSearchProps}>
                  <Stack
                    {...horizontalStackSeachChildProps}
                    className="leftAlignClass"
                  >
                    <TextField
                      className={textFieldClass}
                      placeholder="Filter by name"
                      styles={textFieldStyles}
                      id="txt_FilterName"
                      onChange={this._onFilterTextChange}
                    />
                  </Stack>
                  <Stack {...horizontalStackSeachChildProps}>
                    <IconButton
                      iconProps={cancelIcon}
                      id="btn_FilterCancel"
                      onClick={this._onFilterBoxClose}
                    />
                  </Stack>
                </Stack>
              )}

              {/* <Announced message={`Number of items after filter applied: ${items.length}.`}  /> */}
              <MarqueeSelection selection={this._selection}>
                <Stack>
                  <ShimmeredDetailsList
                    items={items}
                    columns={this._columns}
                    setKey="set"
                    detailsListStyles={
                      items.length == 0 ? emptyGridStyles : gridStyles
                    }
                    layoutMode={DetailsListLayoutMode.fixedColumns}
                    selection={this._selection}
                    selectionPreservedOnEmptyClick={true}
                    ariaLabelForSelectionColumn="Toggle selection"
                    ariaLabelForSelectAllCheckbox="Toggle selection for all items"
                    checkButtonAriaLabel="select row"
                    checkboxVisibility={CheckboxVisibility.hidden}
                    enableShimmer={this.state.loadingListProduction}
                    ariaLabelForShimmer="Fetching productions..."
                    ariaLabelForGrid="Item details"
                    removeFadingOverlay={true}
                    shimmerOverlayStyles={shimmerGridStyles}
                    shimmerLines={15}
                  />
                  {items.length == 0 ? <Stack>No productions</Stack> : <div />}
                </Stack>
              </MarqueeSelection>
            </Stack>
          )}
        </Stack>
      </Layout>
    );
  }

  private _getSelectionDetails(): string {
    const selectionCount = this._selection.getSelectedCount();

    switch (selectionCount) {
      case 0:
        return "No items selected";
      case 1:
        return (
          "1 item selected: " +
          (this._selection.getSelection()[0] as IListProductionItem).name
        );
      default:
        return `${selectionCount} items selected`;
    }
  }

  private setsearchVisibilty(isVisible: boolean) {
    this.setState({ showHideFilter: isVisible });
  }
}

function _copyAndSort<T>(
  items: T[],
  columnFieldName: string,
  isSortedDescending?: boolean
): T[] {
  const field = columnFieldName as keyof T;
  return items
    .slice(0)
    .sort((a: T, b: T) =>
      (isSortedDescending ? a[field] <= b[field] : a[field] >= b[field])
        ? 1
        : -1
    );
}
