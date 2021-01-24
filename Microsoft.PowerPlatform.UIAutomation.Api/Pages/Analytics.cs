// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.PowerPlatform.UIAutomation.Api
{

    /// <summary>
    ///  The Sidebar page.
    ///  </summary>
    public class Analytics
        : BrowserPage
    {
        private readonly PowerPlatformAdminCenterBrowser _client;
        public Analytics(PowerPlatformAdminCenterBrowser browser)
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
        public BrowserCommandResult<string> OpenAnalytics()
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
                var AnalyticsButton = driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Analytics.OpenAnalytics]));
                //Expand Analytics
                AnalyticsButton.Click();

                var cdsButton = driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Analytics.CommonDataService]));
                try
                {
                    cdsButton.Click();
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

        public BrowserCommandResult<string> ChangeEnvironment(string environmentName)
        {
            return _client.Execute(GetOptions($"Change Environment to: {environmentName}"), driver =>
            {
                var chosenEnvironment = "";
                var environmentButton = driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Navigation.ChangeEnvironmentButton]));
                var environmentButtonName = environmentButton.FindElements(By.TagName("span"));
                chosenEnvironment = environmentButtonName[1].Text;

                if (chosenEnvironment == environmentName)
                {
                    return chosenEnvironment;
                }
                else
                {
                    environmentButton.Click(true);

                    var environments = driver.FindElement(By.XPath(Elements.Xpath[Reference.Navigation.ChangeEnvironmentList]));
                    var environmentsList = environments.FindElements(By.TagName("li"));


                    if (environmentsList != null)
                    {
                        var environmentFound = false;

                        foreach (var environmentListItem in environmentsList)
                        {
                            var titleLinks = environmentListItem.FindElements(By.XPath(".//div/div"));

                            if (titleLinks != null && titleLinks.Count > 0)
                            {
                                var title = titleLinks[0].GetAttribute("innerText");

                                if (title.ToLower().Contains(environmentName.ToLower()))
                                {
                                    environmentListItem.Click(true);
                                    environmentFound = true;                                    
                                }
                            }
                        }

                        if (!environmentFound)
                            throw new InvalidOperationException($"Environment {environmentName} does not exist in the list of environments. Please verify the environment exists, or that the provided name is correct and try again.");
                    }
                    else
                    {
                        throw new InvalidOperationException($"Environment List contains no values. Please create an environment via the PowerApps Admin Center.");
                    }

                    environmentButtonName = environmentButton.FindElements(By.TagName("span"));
                    chosenEnvironment = environmentButtonName[1].Text;

                    return chosenEnvironment;
                }
            });
        }

        public BrowserCommandResult<bool> ChangeTab(string tabName)
        {
            return _client.Execute(GetOptions($"Change Tab to: {tabName}"), driver =>
            {
                
                    var AnalyticsButton = driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.OrgInsights.ActiveTab]));

                    driver.FindElement(By.XPath(Elements.Xpath[Reference.OrgInsights.TabNav])).FindElement(
                        By.XPath(Elements.Xpath[Reference.OrgInsights.SelectedTab].Replace("[NAME]", tabName))).Click();

                    Thread.Sleep(1000);

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
