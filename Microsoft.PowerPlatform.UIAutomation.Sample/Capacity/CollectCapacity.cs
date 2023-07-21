using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Security;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Dynamics365.UIAutomation.Browser;
using Microsoft.PowerPlatform.UIAutomation.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools;

namespace Microsoft.PowerPlatform.UIAutomation.Sample {
    [TestClass]
    public class PowerPlatformAdminCenterCapacity {
        private static string _username = "";
        private static string _password = "";
        private static BrowserType _browserType;
        private static Uri _xrmUri;
        private static string _azureKey = "";
        private static string _defaultDownloadDirectory = "";
        private static string _driversPath = "";
        private static string _environmentId = "";
        public TestContext TestContext { get; set; }

        private static TestContext _testContext;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            _testContext = context;

            _username = _testContext.Properties["OnlineUsername"].ToString();
            _password = _testContext.Properties["OnlinePassword"].ToString();
            _xrmUri = new Uri(_testContext.Properties["OnlineCrmUrl"].ToString());
            _browserType = (BrowserType)Enum.Parse(typeof(BrowserType), _testContext.Properties["BrowserType"].ToString());
            _azureKey = (!String.IsNullOrEmpty(_testContext.Properties["AzureKey"].ToString())) ? _testContext.Properties["AzureKey"].ToString() : "";
            _defaultDownloadDirectory = (!String.IsNullOrEmpty(_testContext.Properties["DefaultDownloadDirectory"].ToString())) ? _testContext.Properties["DefaultDownloadDirectory"].ToString() : "";
            _environmentId = _testContext.Properties["EnvironmentId"].ToString();
            _driversPath = _testContext.Properties["DriversPath"].ToString();
        }
        [TestMethod]
        public void CollectCapacityForAllEnvironments() {
            string sessionId = Guid.NewGuid().ToString();
            TestSettings.SharedOptions.DriversPath = _driversPath;
            TestSettings.SharedOptions.DownloadsPath = _defaultDownloadDirectory;
            TestSettings.Options.DownloadsPath = _defaultDownloadDirectory;
            using (var powerPlatformClient = new Microsoft.PowerPlatform.UIAutomation.Api.PowerPlatformAdminCenterBrowser(TestSettings.SharedOptions, new Helpers.AppInsightsLogger(_azureKey, sessionId), sessionId)) {
                powerPlatformClient.OnlineLogin.Login(new Uri("https://admin.powerplatform.microsoft.com"), _username.ToSecureString(), _password.ToSecureString());

                powerPlatformClient.Browser.Driver.Navigate().GoToUrl("https://admin.powerplatform.microsoft.com/resources/capacity#environments");
                //powerPlatformClient.Browser.Driver.
                powerPlatformClient.Browser.ThinkTime(1000);
                //powerPlatformClient.Browser.TakeWindowScreenShot(strFileName, ScreenshotImageFormat.Png);
                //powerPlatformClient.Capacity.ChangeTab("Storage capacity");
                if (String.IsNullOrEmpty(_environmentId))
                {
                    powerPlatformClient.Capacity.GetAllEnvironments();
                }
                else
                {
                    powerPlatformClient.Capacity.DrillIntoSpecificEnvironment(_environmentId);
                }
                
                powerPlatformClient.Browser.ThinkTime(5000);
            }
        }
        [TestMethod]
        public void TestExpectedToThrowException() {
            throw new Exception("This test is expected to fail to provide content for Test Runs in Azure DevOps Build results.");
        }

        public static DevToolsSession AddExtraHeader(WebClient client)
        {
            OpenQA.Selenium.DevTools.IDevTools devTools = client.Browser.Driver as OpenQA.Selenium.DevTools.IDevTools;
            //Create and Get DevTool Session
            var session = devTools.GetDevToolsSession();



            OpenQA.Selenium.DevTools.V114.DevToolsSessionDomains devToolsSession = session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V114.DevToolsSessionDomains>();
            session.Domains.Network.EnableNetwork();



            devToolsSession.Network.Enable(new OpenQA.Selenium.DevTools.V114.Network.EnableCommandSettings());



            var extraHeader = new OpenQA.Selenium.DevTools.V114.Network.Headers();
            extraHeader.Add("X-Info", String.Format("{0}_{1}_{2}", "", DateTime.Now.ToString("MM-dd-yyyy"), Guid.NewGuid().ToString()));
            devToolsSession.Network.SetExtraHTTPHeaders(new OpenQA.Selenium.DevTools.V114.Network.SetExtraHTTPHeadersCommandSettings
            {
                Headers = extraHeader
            });
            return session;
        }






    }
}
