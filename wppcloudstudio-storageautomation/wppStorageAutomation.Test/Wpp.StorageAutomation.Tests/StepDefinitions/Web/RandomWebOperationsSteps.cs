// <copyright file="RandomWebOperationsSteps.cs" company="Microsoft">
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//    THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//    OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//    ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//    OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace Wpp.StorageAutomation.Tests.StepDefinitions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Bdd.Core;
    using global::Bdd.Core.Entities;
    using global::Bdd.Core.Utils;
    using global::Bdd.Core.Web.Executors;
    using global::Bdd.Core.Web.Executors.UI;
    using global::Bdd.Core.Web.StepDefinitions;
    using global::Bdd.Core.Web.Utils;
    using global::Wpp.StorageAutomation.Tests.Entities;
    using global::Wpp.StorageAutomation.Tests.Properties;

    using NUnit.Framework;

    using Ocaramba.Types;

    using OpenQA.Selenium;

    using TechTalk.SpecFlow;

    using Wpp.StorageAutomation.Tests.Core;
    using Wpp.StorageAutomation.Tests.Executors;

    [Binding]
    public class RandomWebOperationsSteps : WppWebStepDefinitionBase
    {
        private readonly FeatureContext featureContext;
        private readonly ScenarioContext scenarioContext;
        private IWebElement submit = null;
        private IWebElement input = null;
        private dynamic data;
        private Credentials userDetails;

        public RandomWebOperationsSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;
        }

        [Then(@"I have generated Access Token for ""(.*)""")]
        [StepDefinition(@"I have logged into Storage Automation portal using ""(.*)"" user")]
        public async Task GivenIHaveLaunchedSite(string role)
        {
            //// Getting credentials from Excel
            this.userDetails = this.ScenarioContext.GetCredential<Credentials>(this.FeatureContext, role, "input=Credentials.xlsx");
            Assert.IsNotNull(this.userDetails);
            //// Logging into portal
            await this.LoginAsync().ConfigureAwait(false);
        }

        public async Task LoginAsync()
        {
            this.Get<UrlPage>().NavigateToUrl(ConfigManager.AppSettings[Constants.WebAppUrl]);

            //// Entering credentials into Login page
            this.GivenIHaveEnteredIntoTheField(this.userDetails.User, nameof(Resources.IdpControl), Constants.Username.ToLower());
            this.Get<ElementPage>().Click(nameof(Resources.IdpControl), Constants.Submit.ToLower());
            this.GivenIHaveEnteredIntoTheField(this.userDetails.Password, nameof(Resources.OktaControl), Constants.Password.ToLower());
            this.Get<ElementPage>().Click(nameof(Resources.OktaControl), Constants.Submit.ToLower());

            if (this.scenarioContext.ScenarioInfo.Tags.Contains(Constants.Token))
            {
                await this.FetchToken().ConfigureAwait(false);
            }

            if (!this.userDetails.Role.EqualsIgnoreCase(Constants.Unauthorized))
            {
                this.Get<ElementPage>().WaitUntilElementIsVisible(nameof(Resources.HomePageLogo), 30);
            }
        }

        public async Task FetchToken()
        {
            await Task.Delay(1000).ConfigureAwait(true);
            var currentDriver = this.DriverContext.Driver;
            var jscript = currentDriver as IJavaScriptExecutor;
            string scriptToExecute = "var network = performance.getEntries() || {}; return network;";
            IList<object> netData = (IList<object>)jscript.ExecuteScript(scriptToExecute);
            var listOfRequests = netData.ToDictionaryList();
            string access_token = string.Empty;
            foreach (var listOfNetworkRequests in listOfRequests)
            {
                if (listOfNetworkRequests["name"].ToString().ContainsIgnoreCase("access_token"))
                {
                    access_token = listOfNetworkRequests["name"].ToString();
                    break;
                }
            }

            var regex = new Regex("access_token=.*?&");
            access_token = regex.Match(access_token).Value.Replace("access_token=", string.Empty).Trim('&');

            Assert.IsNotEmpty(access_token);

            await File.WriteAllTextAsync($@"accesstoken.txt".GetFullPath(), access_token).ConfigureAwait(false);
        }

        [Given(@"I have entered ""(.*)"" into the ""(.*)"" field")]
        public void GivenIHaveEnteredIntoTheField(string text, string textbox, object formatArgs)
        {
            this.Get<ElementPage>().WaitUntilPageLoad();
            this.submit = this.Get<ElementPage>().FindElementById(textbox, formatArgs);
            this.submit.SendKeys(text);
        }

        [StepDefinition(@"I enter ""(.*)"" into the ""(.*)"" text field")]
        public void GivenIEnterIntoTheTextField(string text, string field)
        {
            var productionName = this.WipStorageSettings.AllKeys.Contains(text) ? this.WipStorageSettings[text] : text;
            this.ScenarioContext[Constants.ProductionDirName] = productionName;
            this.Get<ElementPage>().WaitUntilPageLoad();
            var page = this.Get<ElementPage>();
            page.EnterText(nameof(Resources.TextBoxById), productionName, field.ToPascalCase());
        }

        [Then(@"I verify ""(.*)"" is disabled")]
        public void ThenIVerifyIsDisabled(string name)
        {
            this.Get<ElementPage>().CheckIfElementIsPresent(nameof(Resources.DisabledElement), 3, name.ToPascalCase());
        }

        [StepDefinition(@"I get a ""(.*)"" banner mesaage")]
        public void ThenIGetABannerMesaage(string message)
        {
            this.Get<ElementPage>().WaitUntilPageLoad();
            var page = this.Get<ElementPage>();
            message = message.Contains("<ProductionStoreName>") ? message.Replace("<ProductionStoreName>", this.ScenarioContext[Constants.ProductionStoreName].ToString()) : message;
            Assert.IsTrue(page.WaitUntilTextToBePresentInElement(page.GetElement(nameof(Resources.BannerMessage)), message, 10), $"Expected:{message}\nFound:{page.GetElementText(nameof(Resources.BannerMessage))}");
        }

        [Then(@"I get a field message ""(.*)"" for ""(.*)"" field")]
        public void ThenIGetAFieldMessageForField(string message, string field)
        {
            this.Get<ElementPage>().WaitUntilPageLoad();
            string uiMessage = this.Get<ElementPage>().FindElementByXPath(nameof(Resources.FieldMessage), field.ToPascalCase()).Text;
            Assert.IsTrue(uiMessage.Contains(message), $"Expected:{message}\nActual:{uiMessage}");
        }

        [Then(@"I get a empty field message for ""(.*)"" field")]
        public void ThenIGetAEmptyFieldMessageForField(string field)
        {
            var emptyElement = this.Get<ElementPage>().FindElementByXPath(nameof(Resources.EmptyMessage), field, field.ToLower());
            Assert.NotNull(emptyElement);
        }

        [When(@"I press submit")]
        public void WhenIPressSubmit()
        {
            this.submit.Submit();
        }

        [Then(@"the ""(.*)"" should be shown on the screen")]
        public void ThenTheSearchResultsShouldBeShownOnTheScreen(string searchResults)
        {
            var key = searchResults.Replace(" ", string.Empty);
            this.Get<ElementPage>().WaitUntilElementIsVisible(key);
            var results = this.Get<ElementPage>().FindElementByXPath(key);
            Assert.IsNotNull(results);
        }

        [When(@"I click ""(.*)"" on confirmation popup")]
        [StepDefinition(@"I click on ""(.*)"" button")]
        public void GivenIClickOnButton(string buttonName)
        {
            this.Get<ElementPage>().Click(nameof(Resources.ButtonById), buttonName.ToPascalCase());
            if (buttonName.EqualsIgnoreCase(Constants.Save))
            {
                this.ScenarioContext[Constants.ProductionCreationDateTime] = DateTime.Now.ToString("dd MMMM yyyy, HH:mm");
            }
        }

        [When(@"I click on ""(.*)"" button for production")]
        public void WhenIClickOnButtonForProduction(string buttonName)
        {
            var page = this.Get<ElementPage>();
            page.Click(nameof(Resources.ButtonById), formatArgs: Constants.Refresh);
            page.WaitUntilElementIsVisible(nameof(Resources.MoreOptionsButtonByProductionName), formatArgs: this.ScenarioContext[Constants.ProductionDirName].ToString());
            page.Click(nameof(Resources.MoreOptionsButtonByProductionName), formatArgs: this.ScenarioContext[Constants.ProductionDirName].ToString());
        }

        [Given(@"I have clicked ""(.*)"" button")]
        public async Task GivenIHaveClickedButton(string buttonName)
        {
            var filePath = @"TestData\Sample.xlsx".GetFullPath();
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            this.input = this.Get<ElementPage>().GetElement(buttonName);
            this.input.Click();

            // Wait for the File-save popup
            await Task.Delay(1000).ConfigureAwait(false);

            // TO DO: Test
            this.input.SendKeys(filePath);
            this.input.SendKeys("{ENTER}");

            // Wait for the file to download
            await Task.Delay(1000).ConfigureAwait(false);
        }

        [When(@"I enter ""(.*)""")]
        [When(@"I hit ""(.*)""")]
        public void WhenIEnter(string text)
        {
            this.input.SendKeys(text);
        }

        [Then(@"the value should be set")]
        public void ThenTheValueShouldBeSet()
        {
            var result = this.input.GetAttribute("value");
            this.VerifyThat(() => Assert.IsTrue(!string.IsNullOrWhiteSpace(result)));
        }

        [Then(@"I verify if data in the file is ""(.*)""")]
        public void ThenIVerifyIfDataInTheFileIs(string expectedData)
        {
            var dataInFile = File.ReadAllText(Constants.FilePath.GetFullPath());
            this.VerifyThat(() => Assert.AreEqual(expectedData, dataInFile));
        }

        [Then(@"the output should show ""(.*)""")]
        public void ThenTheOutputShouldShow(string text)
        {
            var textArea = this.Get<ElementPage>().GetElement(new ElementLocator(Ocaramba.Locator.TagName, "textarea"));
            var result = this.Get<ElementPage>().WaitUntil(() => textArea.GetAttribute("value").Contains(text));
            Assert.IsTrue(result);
        }

        [StepDefinition(@"I enter path of a file to be uploaded")]
        public void WhenIEnterPathOfAFileToBeUploaded()
        {
            this.input.SendKeys(Constants.FilePath.GetFullPath());
        }

        [When(@"I search the downloads tab for the ""(.*)"" file")]
        public async Task WhenISearchTheTabForTheFile(string fileName)
        {
            this.data = await this.Get<FilePage>().CheckDownloadedFileContent(fileName, this.ScenarioContext, this.FeatureContext).ConfigureAwait(false);
        }

        [When(@"I go to downloads tab and read file (.*)")]
        public Task WhenIGoToDownloadsTabAndReadFile(int index)
        {
            return Task.CompletedTask;
        }

        [Then(@"the content of the file should be valid")]
        public void ThenTheContentOfTheFileShouldBeValid()
        {
            Assert.IsTrue(this.data.Count > 0);
        }

        [StepDefinition(@"I am redirected to ""(.*)"" page")]
        public void GivenIAmRedirectedToPage(string heading)
        {
            this.Get<ElementPage>().CheckIfElementIsPresent(nameof(Resources.PageHeading), 3, heading);
        }

        [Given(@"I navigate to ""(.*)"" production store")]
        public void GivenINavigateToProductionStore(string productionStore)
        {
            FileShareConnector fs = new FileShareConnector();
            var productionStoreName = fs.WipStorageSettings.AllKeys.Contains(productionStore) ? fs.WipStorageSettings[productionStore] : productionStore;
            this.ScenarioContext[Constants.ProductionStoreName] = productionStoreName;
            var page = this.Get<ElementPage>();
            var details = ConfigManager.GetSection("wipStorage");
            var expanded = page.GetAttributeValue(nameof(Resources.ButtonByArialabel), "aria-expanded", formatArgs: details[Constants.Region1]);
            if (expanded.EqualsIgnoreCase("false"))
            {
                page.Click(nameof(Resources.ButtonByTitle), formatArgs: details[Constants.Region1]);
            }

            page.WaitUntilElementToBeClickable(nameof(Resources.ButtonByTitle), formatArgs: productionStoreName);
            page.Click(nameof(Resources.ButtonByTitle), formatArgs: productionStoreName);
        }

        [StepDefinition(@"I navigate to ""(.*)"" production store in ""(.*)"" region")]
        public void GivenINavigateToProductionStoreInRegion(string productionStore, string region)
        {
            FileShareConnector fs = new FileShareConnector();
            var ps = fs.WipStorageSettings.AllKeys.Contains(productionStore) ? fs.WipStorageSettings[productionStore] : productionStore;
            var details = ConfigManager.GetSection("wipStorage");
            var reg = details.AllKeys.Contains(region) ? details[region] : region;
            this.WhenIRegionWithTheName("expand", reg);
            var page = this.Get<ElementPage>();
            page.WaitUntilPageLoad();
            page.WaitUntilElementToBeClickable(nameof(Resources.ButtonByTitle), formatArgs: ps);
            page.Click(nameof(Resources.ButtonByTitle), ps);
        }

        [When(@"a ""(.*)"" in the ""(.*)"" is in ""(.*)"" status")]
        public void WhenAInTheIsInStatus(string directory, string ps, string status)
        {
            var page = this.Get<ElementPage>();
            this.ScenarioContext[Constants.ProductionDirName] = this.WipStorageSettings[directory];
            this.ScenarioContext[Constants.ProductionStoreName] = this.WipStorageSettings[ps];
            var arguments = new object[] { Constants.Status.ToLower(), 1 };
            var productionStatus = page.GetElementText(nameof(Resources.ProductionDetailsInGrid), formatArgs: arguments);
            if (!productionStatus.EqualsIgnoreCase(status))
            {
                page.WaitUntilElementIsVisible(nameof(Resources.MoreOptionsButtonByProductionName), formatArgs: this.ScenarioContext[Constants.ProductionDirName].ToString());
                page.Click(nameof(Resources.MoreOptionsButtonByProductionName), formatArgs: this.ScenarioContext[Constants.ProductionDirName].ToString());
                page.WaitUntilElementIsVisible(nameof(Resources.ButtonById), formatArgs: Constants.MakeOffline);
                page.Click(nameof(Resources.ButtonById), formatArgs: Constants.MakeOffline);
                page.WaitUntilElementIsVisible(nameof(Resources.ButtonById), formatArgs: Constants.Ok);
                page.Click(nameof(Resources.ButtonById), formatArgs: Constants.Ok);
            }

            page.Click(nameof(Resources.ButtonById), formatArgs: Constants.Refresh);
            page.WaitUntilPageLoad();
        }

        ////[When(@"a production in the ""(.*)"" is in ""(.*)"" status")]
        ////public async Task WhenAProductionInTheIsInStatusAsync(string productionStore, string status)
        ////{
        ////    FileShareConnector fs = new FileShareConnector();
        ////    productionStore = fs.WipStorageSettings.AllKeys.Contains(productionStore) ? fs.WipStorageSettings[productionStore] : productionStore;
        ////    var page = this.Get<ElementPage>();
        ////    var arguments = new object[] { Constants.Name.ToLower(), 1 };
        ////    var productionName = page.GetElementText(nameof(Resources.ProductionDetailsInGrid), formatArgs: arguments);
        ////    await this.ReadSqlAsync(SqlQueries.UpdateProductionStatus, args: new { status, productionName }).ConfigureAwait(false);
        ////    this.ScenarioContext[Constants.ProductionDirName] = productionName;
        ////    this.ScenarioContext[Constants.ProductionStoreName] = productionStore;
        ////    if (!status.EqualsIgnoreCase(Constants.Online) || !productionName.EqualsIgnoreCase("!!!_Production_!!!"))
        ////    {
        ////        var productionDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetProductionStoreDetails, args: new { name = productionStore }).ConfigureAwait(false);
        ////        FileShareConnector fileShare = new FileShareConnector(productionDetails[Constants.WIPKeyName]);
        ////        await fileShare.DeleteProduction(this.ScenarioContext.Get<string>(Constants.ProductionDirName), this.ScenarioContext.Get<string>(Constants.ProductionStoreName));
        ////    }

        ////    page.Click(nameof(Resources.ButtonById), formatArgs: Constants.Refresh);
        ////    page.WaitUntilPageLoad();
        ////}

        [When(@"a production in the ""(.*)"" is in ""(.*)"" status")]
        public async Task WhenAProductionInTheIsInStatusAsync(string productionStore, string status)
        {
            FileShareConnector fs = new FileShareConnector();
            productionStore = fs.WipStorageSettings.AllKeys.Contains(productionStore) ? fs.WipStorageSettings[productionStore] : productionStore;
            var page = this.Get<ElementPage>();
            await Task.Delay(3000).ConfigureAwait(false);
            this.Get<ElementPage>().WaitUntilPageLoad();
            var arguments = new object[] { Constants.Name.ToLower(), 1 };
            var productionName = page.GetElementText(nameof(Resources.ProductionDetailsInGrid), formatArgs: arguments);
            this.ScenarioContext[Constants.ProductionDirName] = productionName;
            this.ScenarioContext[Constants.ProductionStoreName] = productionStore;
            arguments = new object[] { Constants.Status.ToLower(), 1 };
            var productionStatus = page.GetElementText(nameof(Resources.ProductionDetailsInGrid), formatArgs: arguments);
            if (!productionStatus.EqualsIgnoreCase(status))
            {
                page.WaitUntilElementIsVisible(nameof(Resources.ButtonById), formatArgs: Constants.MoreOptions);
                page.Click(nameof(Resources.ButtonById), formatArgs: Constants.MoreOptions);
                page.WaitUntilElementIsVisible(nameof(Resources.ButtonById), formatArgs: Constants.MakeOffline);
                page.Click(nameof(Resources.ButtonById), formatArgs: Constants.MakeOffline);
                page.WaitUntilElementIsVisible(nameof(Resources.ButtonById), formatArgs: Constants.Ok);
                page.Click(nameof(Resources.ButtonById), formatArgs: Constants.Ok);
            }

            page.Click(nameof(Resources.ButtonById), formatArgs: Constants.Refresh);
            page.WaitUntilPageLoad();
        }

        [When(@"delete production from archive container")]
        public async Task WhenDeleteProductionFromArchiveContainer()
        {
            var productionDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetProductionStoreDetails, args: new { name = this.ScenarioContext[Constants.ProductionStoreName].ToString() }).ConfigureAwait(false);
            BlobConnector bs = new BlobConnector(productionDetails[Constants.ArchiveKeyName]);
            var productionId = await this.GetProdGUIDAsync(this.ScenarioContext[Constants.ProductionDirName].ToString());
            await bs.DeleteFolderInContainerAsync(this.ScenarioContext[Constants.ProductionStoreName].ToString(), productionId).ConfigureAwait(false);
        }

        [When(@"production in the ""(.*)"" is in ""(.*)"" status")]
        public async Task WhenProductionInTheIsInStatus(string productionStore, string status)
        {
            FileShareConnector fs = new FileShareConnector();
            productionStore = fs.WipStorageSettings.AllKeys.Contains(productionStore) ? fs.WipStorageSettings[productionStore] : productionStore;
            await Task.Delay(3000).ConfigureAwait(false);
            this.Get<ElementPage>().WaitUntilPageLoad();
            var page = this.Get<ElementPage>();
            var arguments = new object[] { Constants.Name.ToLower(), 1 };
            var productionName = page.GetElementText(nameof(Resources.ProductionDetailsInGrid), formatArgs: arguments);
            this.ScenarioContext[Constants.ProductionDirName] = productionName;
            this.ScenarioContext[Constants.ProductionStoreName] = productionStore;
            this.ScenarioContext[Constants.WIPStore] = productionStore;
            var productionDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetSpecificProductionDetails, args: new { name = productionName }).ConfigureAwait(false);
            Assert.AreEqual(status, productionDetails[Constants.Status]);
        }

        [Then(@"production is restored and status is updated to Online in UI for production")]
        public async Task ThenProductionIsRestoredAndStatusIsUpdatedToOnlineInUIForProduction()
        {
            await this.ProductionIsRestoredAndStatusIsUpdatedToOnlineInUIAsync(2).ConfigureAwait(false);
        }

        [Then(@"production is restored and status is updated to Online in UI")]
        public async Task ThenProductionIsRestoredAndStatusIsUpdatedToOnlineInUIAsync()
        {
            await this.ProductionIsRestoredAndStatusIsUpdatedToOnlineInUIAsync(1).ConfigureAwait(false);
        }

        [Then(@"production status is updated to ""(.*)"" in UI")]
        public async Task ThenProductionStatusIsUpdatedToInUI(string status)
        {
            var page = this.Get<ElementPage>();
            page.Click(nameof(Resources.ButtonById), formatArgs: Constants.Refresh);
            await Task.Delay(3000).ConfigureAwait(true);
            var arguments = new object[] { Constants.Status.ToLower(), 1 };
            Assert.IsTrue(page.WaitUntilTextToBePresentInElement(page.GetElement(nameof(Resources.ProductionDetailsInGrid), formatArgs: arguments), status, 10), $"Expected:{status}\nFound:{page.GetElementText(nameof(Resources.ProductionDetailsInGrid), formatArgs: arguments)}");
        }

        public async Task ProductionIsRestoredAndStatusIsUpdatedToOnlineInUIAsync(int number)
        {
            var page = this.Get<ElementPage>();
            this.ThenIGetABannerMesaage(string.Format(WebElementConstants.RestoreProductionSuccessMessage, this.ScenarioContext[Constants.ProductionDirName]).ToString());
            page.WaitUntilElementIsVisible(nameof(Resources.BannerMessage), 10);
            var restoreMessage = page.GetElementText(nameof(Resources.BannerMessage));
            Assert.AreEqual(this.ScenarioContext[Constants.ProductionDirName] + " restored successfully.", restoreMessage);
            //// sometimes it is taking 4-5 seconds to update status
            await Task.Delay(5000).ConfigureAwait(true);
            var arguments = new object[] { Constants.Status.ToLower(), number };
            var status = page.GetElementText(nameof(Resources.ProductionDetailsInGrid), formatArgs: arguments);
            Assert.AreEqual(Constants.Online, status);
        }

        [Then(@"user should see error message as ""(.*)""")]
        public async Task ThenUserShouldSeeErrorMessageAs(string message)
        {
            await Task.Delay(3000).ConfigureAwait(false);
            Assert.AreEqual(message, this.Get<ElementPage>().GetElementText(nameof(Resources.BannerMessage)));
        }

        [Then(@"I verify that ""(.*)"" message is displayed")]
        public void ThenIVerifyThatMessageIsDisplayed(string message)
        {
            Assert.AreEqual(message.Replace("<production>", this.GetValue(Constants.ProductionDirName)), this.Get<ElementPage>().GetElementText(nameof(Resources.BannerMessage)));
        }

        [Then(@"I should see ""(.*)"" option in the Contextual Menu")]
        public void ThenIShouldSeeOptionInTheContextualMenu(string button)
        {
            this.Get<ElementPage>().CheckIfElementIsPresent(nameof(Resources.ButtonById), formatArgs: button);
        }

        [Then(@"Verify ""(.*)"" is present in ""(.*)"" column")]
        public void ThenVerifyIsPresentInColumn(string value, string columnName)
        {
            var page = this.Get<ElementPage>();
            var arguments = new object[] { Constants.Name.ToLower(), 1 };
            value = this.WipStorageSettings.AllKeys.Contains(value) ? this.WipStorageSettings[value] : value;
            page.WaitUntilElementIsVisible(nameof(Resources.ProductionDetailsInGrid), 30, formatArgs: arguments);
            var columnList = page.GetListElementsText(nameof(Resources.ColumnListByName), formatArgs: columnName.ToLower());
            Assert.IsTrue(columnList.Contains(value), $"{value} not found in {columnName} column");
        }

        [Then(@"Verify ""(.*)"" is not present in ""(.*)"" column")]
        public void ThenVerifyIsNotPresentInColumn(string value, string columnName)
        {
            var page = this.Get<ElementPage>();
            var arguments = new object[] { Constants.Name.ToLower(), 1 };
            value = this.WipStorageSettings.AllKeys.Contains(value) ? this.WipStorageSettings[value] : value;
            page.WaitUntilElementIsVisible(nameof(Resources.ProductionDetailsInGrid), 30, formatArgs: arguments);
            var columnList = page.GetListElementsText(nameof(Resources.ColumnListByName), formatArgs: columnName.ToLower());
            Assert.IsFalse(columnList.Contains(value), $"{value} not found in {columnName} column");
        }

        [Then(@"Verify ""(.*)"" spinner is visible if action takes time")]
        public void ThenVerifySpinnerIsVisibleIfActionTakesTime(string text)
        {
            var page = this.Get<ElementPage>();
            if (page.CheckIfElementIsPresent(nameof(Resources.Spinner), 5, formatArgs: text))
            {
                page.WaitUntilInvisibilityOfElementLocated(nameof(Resources.Spinner), 30000, formatArgs: text);
            }
        }

        [StepDefinition(@"Verify Production ""(.*)"" is created in ""(.*)"" Production Store on UI")]
        public void ThenVerifyProductionIsCreatedInProductionStoreOnUI(string productionName, string productionStore)
        {
            this.ScenarioContext[Constants.ProductionDirName] = productionName;
            var page = this.Get<ElementPage>();
            page.WaitUntilInvisibilityOfElementLocated(nameof(Resources.Spinner), 5, formatArgs: "Creating");
            this.ThenIGetABannerMesaage(string.Format(WebElementConstants.CreateProductionSuccessMessage, productionName));
            this.GivenIClickOnButton(WebElementConstants.CreateProductionDiscardButton);
            this.ThenVerifyIsPresentInColumn(productionName, WebElementConstants.ProductionGridProductionName.ToLower());

            // Commented till dates logic is finalized
            // this.operation.ThenVerifyIsPresentInColumn("0", WebElementConstants.ProductionGridSize.ToLower());
            // this.operation.ThenVerifyIsPresentInColumn("0", WebElementConstants.ProductionGridSize.ToLower());
            this.ThenVerifyIsPresentInColumn("0 B", WebElementConstants.ProductionGridSize.ToLower());
            this.ThenVerifyIsPresentInColumn("Online", WebElementConstants.ProductionGridStatus.ToLower());
            this.ThenVerifyIsPresentInColumn(this.ScenarioContext[Constants.ProductionCreationDateTime].ToString(), WebElementConstants.Created.ToLower());
            this.ThenVerifyIsPresentInColumn(this.ScenarioContext[Constants.ProductionCreationDateTime].ToString(), WebElementConstants.Modified.ToLower());
        }

        [Then(@"Verify all productions are listed for ""(.*)"" production store in ""(.*)"" order of ""(.*)""")]
        public async Task ThenVerifyAllProductionsAreListedForProductionStoreInOrderOf(string productionStore, string order, string column)
        {
            BlobConnector blob = new BlobConnector();
            var psName = blob.ArchiveStorageSettings.AllKeys.Contains(productionStore) ? blob.ArchiveStorageSettings[productionStore] : productionStore;
            string psGUID = await this.ReadSqlScalarAsync<string>(SqlQueries.GetGuidOfPS, args: new { name = psName }).ConfigureAwait(false);
            var productionList = await this.ReadAllSqlAsDictionaryAsync(SqlQueries.GetProductionsNameInProductionStore, args: new { productionStoreId = psGUID }).ConfigureAwait(false);
            this.Get<ElementPage>().WaitUntilPageLoad();
            await Task.Delay(3000).ConfigureAwait(false);
            List<string> productionName = this.Get<ElementPage>().GetListElementsText(nameof(Resources.ColumnListByName), formatArgs: WebElementConstants.ProductionGridProductionName.ToLower());
            Assert.AreEqual(productionList.ToList().Count, productionName.Count, $"DB and UI Production count mismatch\nDB:{string.Join(",", productionList)}\nUI:{string.Join(",", productionName.ToArray())}");
            foreach (var productionItem in productionList)
            {
                Assert.IsTrue(productionName.Contains(productionItem[Constants.Name]), $"{productionItem[Constants.Name]} not present on UI");
            }

            this.IsSortedInOrder(column);
        }

        [Given(@"I click on column header and I verify the table is sorted")]
        public void GivenIClickOnColumnHeaderAndIVerifyTheTableIsSorted(Table table)
        {
            var page = this.Get<ElementPage>();
            var columns = table.Rows.ToDictionary(r => r[0]);
            foreach (var column in columns)
            {
                page.Click(nameof(Resources.ColumnName), column.Key.ToLower());
                this.IsSortedInOrder(column.Key);
                page.Click(nameof(Resources.ColumnName), column.Key.ToLower());
                this.IsSortedInOrder(column.Key, "desc");
            }
        }

        [StepDefinition(@"I ""(.*)"" region with the name ""(.*)""")]
        public void WhenIRegionWithTheName(string action, string regionName)
        {
            this.Get<ElementPage>().WaitUntilPageLoad();
            var page = this.Get<ElementPage>();
            var details = ConfigManager.GetSection("wipStorage");
            var region = details.AllKeys.Contains(regionName) ? details[regionName] : regionName;
            string currentState = page.GetElement(nameof(Resources.CollapseExpandButton), region).GetAttribute("aria-expanded");
            if ((action.EqualsIgnoreCase("collapse") && currentState.EqualsIgnoreCase("true")) || (action.EqualsIgnoreCase("expand") && currentState.EqualsIgnoreCase("false")))
            {
                Thread.Sleep(2000);
                page.Click(nameof(Resources.ButtonByTitle), region);
            }
        }

        [StepDefinition(@"Verify production stores ""(.*)"" listed for ""(.*)"" region")]
        public async Task ThenVerifyProductionStoresListedForRegion(string expected, string regionName)
        {
            var details = ConfigManager.GetSection("wipStorage");
            var region = details.AllKeys.Contains(regionName) ? details[regionName] : regionName;
            var prodStores = await this.ReadAllSqlAsDictionaryAsync(SqlQueries.GetPSForARegion, args: new { region = region }).ConfigureAwait(false);
            if (expected.EqualsIgnoreCase("are"))
            {
                foreach (var ps in prodStores)
                {
                    this.Get<ElementPage>().WaitUntilElementIsVisible(nameof(Resources.ButtonByTitle), 1, ps[Constants.Name]);
                    List<string> psUIList = new List<string>();
                    foreach (IWebElement element in this.Get<ElementPage>().FindElementsByXPath(nameof(Resources.ProductionStoresListByRegion), " "))
                    {
                        psUIList.Add(element.GetAttribute("name"));
                    }

                    this.IsSortedInOrder(psUIList, "asc");
                }
            }
            else if (expected.EqualsIgnoreCase("not"))
            {
                foreach (var ps in prodStores)
                {
                    this.Get<ElementPage>().WaitUntilInvisibilityOfElementLocated(nameof(Resources.ButtonByTitle), 1, ps[Constants.Name]);
                }
            }
        }

        [Then(@"UI should include production details with below columns")]
        public void ThenUIShouldIncludeProductionDetailsWithBelowColumns(Table table)
        {
            var columns = table.Rows.ToDictionary(r => r[0]);
            foreach (var column in columns)
            {
                Assert.IsTrue(this.Get<ElementPage>().CheckIfElementIsPresent(nameof(Resources.ColumnName), formatArgs: column.Key.ToLower()));
            }
        }

        [Then(@"I verify result includes ""(.*)"" in ""(.*)""")]
        public void ThenIVerifyResultIncludesIn(string p0, string p1)
        {
            // To be implemented when filter functionality is implemented
        }

        [Then(@"user can see error message as ""(.*)""")]
        public void ThenUserCanSeeErrorMessageAs(string message)
        {
            Assert.IsTrue(this.Get<ElementPage>().CheckIfElementIsPresent(nameof(Resources.HomePage), formatArgs: message));
        }

        [StepDefinition(@"user is navigated to home page")]
        public void ThenUserIsNavigatedToHomePage()
        {
            Assert.IsTrue(this.Get<ElementPage>().CheckIfElementIsPresent(nameof(Resources.HomePageLogo)));
        }

        [Then(@"user should not be logged into portal")]
        public void ThenUserShouldNotBeLoggedIntoPortal()
        {
            Assert.IsFalse(this.Get<ElementPage>().CheckIfElementIsPresent(nameof(Resources.HomePageLogo)));
        }

        [Then(@"user is logged out from cloud studio portal")]
        public void ThenUserIsLoggedOutFromCloudStudioPortal()
        {
            Assert.IsTrue(this.Get<ElementPage>().CheckIfElementIsPresent(nameof(Resources.IdpControl), formatArgs: Constants.Submit.ToLower()));
        }

        [Then(@"Verify regions are listed in ""(.*)"" order")]
        public async Task ThenVerifyRegionsAreListedInOrder(string order)
        {
            List<string> regionDBList = new List<string>();
            List<string> regionUIList = new List<string>();
            foreach (IWebElement element in this.Get<ElementPage>().FindElementsByXPath(nameof(Resources.RegionList)))
            {
                regionUIList.Add(element.GetAttribute("name"));
            }

            var distinctRegions = await this.ReadAllSqlAsDictionaryAsync(SqlQueries.GetDistinctRegions).ConfigureAwait(false);
            foreach (var productionItem in distinctRegions)
            {
                regionDBList.Add(productionItem["Region"].ToString());
            }

            this.IsSortedInOrder(regionUIList, order);
            Assert.IsTrue(regionUIList.All(regionDBList.Contains), $"Productions List mismatch: \n {string.Join(",", regionUIList.Except(regionDBList).ToArray())} \n {string.Join(",", regionDBList.Except(regionUIList).ToArray())}");
        }

        [Then(@"Verify production stores are listed in ""(.*)"" order")]
        public async Task ThenVerifyProductionStoresAreListedInOrder(string order)
        {
            List<string> psDBList = new List<string>();
            List<string> psUIList = new List<string>();
            foreach (IWebElement element in this.Get<ElementPage>().FindElementsByXPath(nameof(Resources.ProductionStoresListByRegion), " "))
            {
                psUIList.Add(element.GetAttribute("name"));
            }

            var prodstores = await this.ReadAllSqlAsDictionaryAsync(SqlQueries.GetPSForARegion).ConfigureAwait(false);
            foreach (var productionItem in prodstores)
            {
                psDBList.Add(productionItem["Name"].ToString());
            }

            this.IsSortedInOrder(psUIList, order);
            Assert.IsTrue(psUIList.All(psDBList.Contains), $"Productions List mismatch: \n {string.Join(",", psUIList.Except(psDBList).ToArray())} \n {string.Join(",", psDBList.Except(psUIList).ToArray())}");
        }

        [Then(@"Verify first production store is selected in the production store list")]
        public async Task ThenVerifyFirstProductionStoreIsSelectedInTheProductionStoreList()
        {
            string firstPs = await this.ReadSqlScalarAsync<string>(SqlQueries.GetFirstPSOnUI).ConfigureAwait(false);
            await this.ThenVerifyAllProductionsAreListedForProductionStoreInOrderOf(firstPs, "asc", "name");
        }

        [Then(@"Verify all production stores with which ""(.*)"" is registered")]
        public async Task ThenVerifyAllProductionStoresOfWhichIsAMemberOfAreListed(string groupName)
        {
            var psDetails = await this.ReadAllSqlAsDictionaryAsync(SqlQueries.GetPSForAGroup, args: new { groupName = "%" + groupName + "%" }).ConfigureAwait(false);
            List<string> prodStoreDBList = new List<string>();
            foreach (var productionItem in psDetails)
            {
                prodStoreDBList.Add(productionItem[Constants.Name].ToString());
            }

            List<string> psUIList = new List<string>();
            foreach (IWebElement element in this.Get<ElementPage>().FindElementsByXPath(nameof(Resources.ProductionStoresOnUI)))
            {
                psUIList.Add(element.GetAttribute("name"));
            }

            Assert.IsTrue(psUIList.All(prodStoreDBList.Contains), $"Productions List mismatch: \n {string.Join(",", psUIList.Except(prodStoreDBList).ToArray())} \n {string.Join(",", prodStoreDBList.Except(psUIList).ToArray())}");
        }

        [Then(@"Verify all production stores of which ""(.*)"" is not a member of are not listed")]
        public async Task ThenVerifyAllProductionStoresOfWhichIsNotAMemberOfAreNotListed(string groupName)
        {
            var psDetails = await this.ReadAllSqlAsDictionaryAsync(SqlQueries.GetPSForAGroup, args: new { groupName = groupName }).ConfigureAwait(false);
            List<string> prodStoreDBList = new List<string>();
            foreach (var productionItem in psDetails)
            {
                prodStoreDBList.Add(productionItem[Constants.Name].ToString());
            }

            List<string> psUIList = new List<string>();
            foreach (IWebElement element in this.Get<ElementPage>().FindElementsByXPath(nameof(Resources.ProductionStoresListByRegion), " "))
            {
                psUIList.Add(element.GetAttribute("name"));
            }

            Assert.IsFalse(psUIList.All(prodStoreDBList.Contains), $"Productions List mismatch: \n {string.Join(",", psUIList.Except(prodStoreDBList).ToArray())} \n {string.Join(",", prodStoreDBList.Except(psUIList).ToArray())}");
        }

        [Then(@"Verify ""(.*)"" production store is not visible")]
        public void ThenVerifyProductionStoreIsNotVisible(string productionStore)
        {
            var page = this.Get<ElementPage>();
            var psName = this.ArchiveStorageSettings.AllKeys.Contains(productionStore) ? this.ArchiveStorageSettings[productionStore] : productionStore;
            Assert.IsFalse(page.CheckIfElementIsPresent(nameof(Resources.ButtonByTitle), 5, formatArgs: psName));
        }

        [When(@"I naviagte to a an unauthorized production store ""(.*)""")]
        public async Task WhenINaviagteToAAnUnauthorizedProductionStore(string productionStore)
        {
            var psName = this.ArchiveStorageSettings.AllKeys.Contains(productionStore) ? this.ArchiveStorageSettings[productionStore] : productionStore;
            string psGUID = await this.ReadSqlScalarAsync<string>(SqlQueries.GetGuidOfPS, args: new { name = psName }).ConfigureAwait(false);
            this.Get<UrlPage>().NavigateToUrl(ConfigManager.AppSettings[Constants.WebAppUrl] + psGUID);
        }

        [When(@"production size is ""(.*)"" bytes")]
        public async Task WhenProductionSizeIsBytes(int numberOfBytes)
        {
            await this.ReadSqlAsync(SqlQueries.UpdateProductionSizeByProductionName, args: new { numberOfBytes, name = this.ScenarioContext[Constants.ProductionDirName] }).ConfigureAwait(false);
        }

        [Then(@"size column in grid should display as ""(.*)""")]
        public async Task ThenSizeColumnInGridShouldDisplayAs(string expected)
        {
            var page = this.Get<ElementPage>();
            page.Click(nameof(Resources.ButtonById), formatArgs: Constants.Refresh);
            await Task.Delay(3000).ConfigureAwait(true);
            var arguments = new object[] { Constants.Name.ToLower(), 1 };
            page.WaitUntilElementIsVisible(nameof(Resources.ProductionDetailsInGrid), 30, formatArgs: arguments);
            var actual = page.GetElementText(nameof(Resources.ColumnListByName), formatArgs: Constants.Size.ToLower());
            Assert.AreEqual(expected, actual);
        }

        //// ========================================================

        public void IsSortedInOrder(string column, string order = "asc")
        {
            List<string> list = this.Get<ElementPage>().GetListElementsText(nameof(Resources.ColumnListByName), formatArgs: column);
            if (order.ToLower() == "asc")
            {
                Assert.IsTrue(list.SequenceEqual(list.OrderBy(name => name)), $"{string.Join(", ", list)} is not sorted in {order} order");
            }
            else
            {
                Assert.IsTrue(list.SequenceEqual(list.OrderByDescending(name => name)), $"{string.Join(", ", list)} is not sorted in {order} order");
            }
        }

        public void IsSortedInOrder(List<string> list, string order = "asc")
        {
            if (order.ToLower() == "asc")
            {
                list.Reverse();
                Assert.IsTrue(list.IsInDescendingOrder(), $"{string.Join(", ", list)} is not sorted in {order} order");
            }
            else
            {
                Assert.IsTrue(list.IsInDescendingOrder(), $"{string.Join(", ", list)} is not sorted in {order} order");
            }
        }
    }
}