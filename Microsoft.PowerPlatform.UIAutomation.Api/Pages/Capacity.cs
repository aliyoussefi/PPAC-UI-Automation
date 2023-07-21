// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.PowerPlatform.UIAutomation.Api
{

    /// <summary>
    ///  The Sidebar page.
    ///  </summary>
    public class Capacity
        : BrowserPage
    {
        private readonly PowerPlatformAdminCenterBrowser _client;
        public Capacity(PowerPlatformAdminCenterBrowser browser)
            : base() {
            _client = browser;
        }
        internal BrowserCommandOptions GetOptions(string commandName) {
            return new BrowserCommandOptions(Constants.DefaultTraceSource,
                commandName,
                0,
                0,
                null,
                true,
                typeof(NoSuchElementException), typeof(StaleElementReferenceException));
        }
        //public BrowserCommandResult<bool> Homepage()
        //{
        //    return this.Execute(GetOptions("Home Page"), driver =>
        //    {
        //        var uri = new Uri(this.Browser.Driver.Url);
        //        var link = $"{uri.Scheme}://{uri.Authority}/home";

        //        driver.Navigate().GoToUrl(link);

        //        driver.WaitUntilVisible(By.XPath(Elements.Xpath[Reference.Navigation.HomePage])
        //            , new TimeSpan(0, 2, 0),
        //            e => { e.WaitForPageToLoad(); },
        //            f => { throw new Exception("Home page load failed."); });

        //        return true;
        //    });
        //}
        public void HandleAnalyticsPaneExceptionsGracefully(Exception ex)
        {
            //"CDS" "Common Data Service" "DataFlex"
        }
        public class OpenAnalyticsPaneNotAvailableException : Exception { };
        public BrowserCommandResult<string> OpenCapacity()
        {
            BrowserCommandOptions openAnalyticsCommandOptions = new BrowserCommandOptions();
            openAnalyticsCommandOptions.CommandName = "Open Analytics Pane";
            openAnalyticsCommandOptions.RetryAttempts = 3;
            openAnalyticsCommandOptions.RetryDelay = 1000;
            openAnalyticsCommandOptions.ThrowExceptions = true;
            openAnalyticsCommandOptions.ExceptionTypes = new List<Type>();
            openAnalyticsCommandOptions.ExceptionTypes.Add(typeof(OpenAnalyticsPaneNotAvailableException));
            openAnalyticsCommandOptions.ExceptionAction = HandleAnalyticsPaneExceptionsGracefully;
            return _client.Execute(openAnalyticsCommandOptions, driver =>
            {
                var chosenEnvironment = "";
                var ResourcesButton = driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Capacity.OpenResources]));
                //Expand Analytics
                ResourcesButton.Click();

                var capacityButton = driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Capacity.OpenCapacity]));
                try
                {
                    capacityButton.Click();
                }
                catch (Exception ex)
                {

                    //throw new OpenAnalyticsPaneNotAvailableException(ex, "CDS Option not available to click");
                }
                
                return chosenEnvironment;
                
            });
        }

        public BrowserCommandResult<Dictionary<string, string>> GetHomeAnalytics()
        {
            return _client.Execute(GetOptions($"GetHomeAnalytics"), driver =>
            {
                Dictionary<string, string> homeAnalytics = new Dictionary<string, string>();
                var AnalyticsButton = driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.OrgInsights_Home.ActiveUsers]));
                //Expand Analytics
                var activeUserCount = AnalyticsButton.FindElement(By.TagName("div")).Text;
                var apiCalls = driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.OrgInsights_Home.ApiCalls])).FindElement(By.TagName("div")).Text;
                var apiSuccess = driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.OrgInsights_Home.ApiSuccess])).FindElement(By.TagName("div")).Text;
                var pluginExecutions = driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.OrgInsights_Home.PluginExecutions])).FindElement(By.TagName("div")).Text;

                homeAnalytics.Add("Active User Count", activeUserCount);
                homeAnalytics.Add("API Calls", apiCalls);
                homeAnalytics.Add("API Pass Rate", apiSuccess);
                homeAnalytics.Add("Executions", pluginExecutions);
                return homeAnalytics;

            });
        }

        public BrowserCommandResult<string> ChangeFilters(string environment, string fromDate, string toDate, string fromTime, string toTime)
        {
            return _client.Execute(GetOptions($"ChangeFilters"), driver =>
            {
                var chosenEnvironment = "";
                var AnalyticsButton = driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Analytics.ChangeFilters]));
                //Expand Analytics
                AnalyticsButton.Click();

                var cdsButton = driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Analytics.ChangeEnvironment]));
                cdsButton.Click();

                driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Analytics.ChangeEnvironment]));
                var environments = driver.FindElement(By.XPath(Elements.Xpath[Reference.Analytics.ChangeEnvironment]));
                var environmentPickerId = environments.GetAttribute("id");
                var environmentPicker = driver.FindElement(By.Id(environmentPickerId + "-list"));
                bool environmentExist = environmentPicker.HasElement(By.XPath(Elements.Xpath[Reference.Analytics.EnvironmentName].Replace("[NAME]", environment.ToLower())));
                if (environmentExist)
                {
                    //choose env
                    environmentPicker.FindElement(By.XPath(Elements.Xpath[Reference.Analytics.EnvironmentName].Replace("[NAME]", environment.ToLower()))).Click();

                    var FromButton = driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Analytics.FilterFromDate]));
                    FromButton.Click();
                    //button aria-label June 30, 2020
                    FromButton.FindElement(By.XPath(Elements.Xpath[Reference.Analytics.FilterDateValue].Replace("[NAME]", fromDate))).Click();


                    var ToButton = driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Analytics.FilterToDate]));
                    ToButton.Click();
                    //button aria-label June 30, 2020
                    FromButton.FindElement(By.XPath(Elements.Xpath[Reference.Analytics.FilterDateValue].Replace("[NAME]", toDate))).Click();


                    //FromButton.FindElement(By.XPath("//button[@aria-label=\"June 30, 2020\"]")).Click();
                    //From Time
                    var FromTimeDropDown = driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Analytics.FilterFromTime]));
                    FromTimeDropDown.Click();
                    var FromTimeDropDownPicklist = driver.WaitUntilAvailable(By.Id(FromTimeDropDown.GetAttribute("id") + "-list"));
                    FromTimeDropDownPicklist.FindElement(By.XPath(Elements.Xpath[Reference.Analytics.FilterToTimePicklistValue].Replace("[NAME]", fromTime.ToLower()))).Click();

                    //To Time
                    var ToTimeDropDown = driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Analytics.FilterToTime]));
                    ToTimeDropDown.Click();
                    var ToTimeDropDownPicklist = driver.WaitUntilAvailable(By.Id(ToTimeDropDown.GetAttribute("id") + "-list"));
                    ToTimeDropDownPicklist.FindElement(By.XPath(Elements.Xpath[Reference.Analytics.FilterToTimePicklistValue].Replace("[NAME]", toTime.ToLower()))).Click();

                    driver.FindElement(By.XPath(Elements.Xpath[Reference.Analytics.FilterApply])).Click();
                }
                //var environments = cdsButton.FindElements(By.XPath("//button[@title=/" + environment + "/]"));

                

                return chosenEnvironment;

            });
        }

        //ms-List-page
        public BrowserCommandResult<string> GetAllEnvironments() {
            return _client.Execute(GetOptions($"Get All Environments"), driver =>
            {
                var chosenEnvironment = "";
                string containerClass = LocateVisibleEnvironments(driver);
                //var returnObject = driver.ExecuteScript("return window.document.getElementsByClassName('" + containerClass + "')[0].scrollTop += 1000");
                //LocateVisibleEnvironments(driver);
                //var returnObject1 = driver.ExecuteScript("return window.document.getElementsByClassName('" + containerClass + "')[0].scrollTop += 1000");
                //while (Convert.ToInt32(returnObject1) > Convert.ToInt32(returnObject)) {
                //    returnObject = returnObject1;
                //    LocateVisibleEnvironments(driver);
                //    returnObject1 = driver.ExecuteScript("return window.document.getElementsByClassName('" + containerClass + "')[0].scrollTop += 1000");
                //}
                //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                //js.ExecuteScript("window.scrollBy(0,1000)");
                //DrillIntoSpecificEnvironment(driver, environmentRow);
                return chosenEnvironment;

            });
        }

        private string LocateVisibleEnvironments(IWebDriver driver) {
            string rtnObjectClassName = String.Empty;
            var allEnvironmentContainer = driver.WaitUntilAvailable(By.XPath("//div[@class=\"ms-List-page\"]"));
            var allEnvironments = driver.FindElements(By.XPath("//div[@class=\"ms-List-page\"]"));
            //var environmentContainer = driver.FindElement(By.ClassName("capacityReportPageContainer"));
            //var environmentScrollableContainer = environmentContainer.FindElement(By.XPath(".//div[contains(@class, \"ms-ScrollablePane--contentContainer\")]"));
            //rtnObjectClassName = environmentScrollableContainer.GetAttribute("class");
            foreach (var listpage in allEnvironments) {
                var environmentList = listpage.FindElements(By.XPath(".//div[@data-automationid=\"DetailsRowFields\"]"));
                foreach (IWebElement environmentRow in environmentList) {
                    string[] stringSeparators = new string[] { "\r\n" };
                    string[] lines = environmentRow.Text.Split(stringSeparators, StringSplitOptions.None);
                    Dictionary<string, string> capacityNumbers = new Dictionary<string, string>();

                    capacityNumbers.Add("Org", lines[0]);
                    capacityNumbers.Add("DB", lines[3]);
                    capacityNumbers.Add("File", lines[4]);
                    capacityNumbers.Add("Log", lines[5]);
                    _client._logger.Log(capacityNumbers);

                    DrillIntoSpecificEnvironment(driver, environmentRow);
                }
            }

            return rtnObjectClassName;
        }

        public BrowserCommandResult<string> ChangeEnvironment(string environmentName)
        {
            return _client.Execute(GetOptions($"Change Environment to: {environmentName}"), driver =>
            {
                var chosenEnvironment = "";
                var environment = driver.WaitUntilAvailable(By.XPath("//span[text()=\"" + environmentName + "\"]"));
                //Get Container
                //environment.
                var environmentRow = environment.FindElement(By.XPath("./..")).FindElement(By.XPath("./.."));
                string[] stringSeparators = new string[] { "\r\n" };
                string[] lines = environmentRow.Text.Split(stringSeparators, StringSplitOptions.None);
                Dictionary<string, string> capacityNumbers = new Dictionary<string, string>();

                capacityNumbers.Add("Org", lines[0]);
                capacityNumbers.Add("DB", lines[3]);
                capacityNumbers.Add("File", lines[4]);
                capacityNumbers.Add("Log", lines[5]);
                _client._logger.Log(capacityNumbers);
                //DrillIntoSpecificEnvironment(driver, environmentRow);

                return chosenEnvironment;

            });
        }

        public static void DrillIntoSpecificEnvironment(IWebDriver driver, IWebElement environmentRow) {
            //Drill into Environment
            environmentRow.FindElement(By.TagName("button")).Click();
            Thread.Sleep(10000);
            IWebElement svgObject = driver.FindElement(By.XPath("//*[local-name()='svg']//*[local-name()='rect' and @class='highcharts-button-box']"));
            Actions builder = new Actions(driver);
            builder.Click(svgObject).Build().Perform();
            IWebElement downloadButton = driver.FindElement(By.XPath("//*[text()='Download all tables']"));
            downloadButton.Click();
        }

        public void DrillIntoSpecificEnvironment(string environmentId)
        {
            //Drill into Environment
            _client.Browser.Driver.Navigate().GoToUrl("https://admin.powerplatform.microsoft.com/resources/capacity/environment/" + environmentId);
            Thread.Sleep(10000);
            IWebElement svgObject = _client.Browser.Driver.FindElement(By.XPath("//*[local-name()='svg']//*[local-name()='rect' and @class='highcharts-button-box']"));
            Actions builder = new Actions(_client.Browser.Driver);
            builder.Click(svgObject).Build().Perform();
            IWebElement downloadButton = _client.Browser.Driver.FindElement(By.XPath("//*[text()='Download all tables']"));
            downloadButton.Click();
        }

        public BrowserCommandResult<bool> ChangeTab(string tabName)
        {
            return _client.Execute(GetOptions($"Change Tab to: {tabName}"), driver =>
            {

                var tabButton = driver.WaitUntilAvailable(By.XPath("//button[@name=\"" + tabName + "\"]"));

                tabButton.Click();

                return true;
                
                 
            });
        }

        public BrowserCommandResult<bool> DownloadReport(string reportName)
        {
            return _client.Execute(GetOptions($"DownloadReport {reportName}"), driver =>
            {

                driver.FindElement(By.XPath(Elements.Xpath[Reference.Analytics.DownloadButton])).Click();

                driver.FindElement(By.XPath(Elements.Xpath[Reference.Analytics.DownloadReportName].Replace("[NAME]", reportName))).Click();

                Thread.Sleep(1000);

                return true;


            });
        }

    }
}
