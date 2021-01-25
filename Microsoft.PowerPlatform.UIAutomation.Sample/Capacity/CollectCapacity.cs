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


namespace Microsoft.PowerPlatform.UIAutomation.Sample {
    [TestClass]
    public class PowerPlatformAdminCenterCapacity {
        private static string _username = "";
        private static string _password = "";
        private static BrowserType _browserType;
        private static Uri _xrmUri;
        private static string _azureKey = "";
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
        }
        [TestMethod]
        public void CollectCapacityForAllEnvironments() {
            string sessionId = Guid.NewGuid().ToString();
            using (var powerPlatformClient = new Microsoft.PowerPlatform.UIAutomation.Api.PowerPlatformAdminCenterBrowser(TestSettings.Options, new Helpers.AppInsightsLogger(_azureKey, sessionId), sessionId)) {
                powerPlatformClient.OnlineLogin.Login(new Uri("https://admin.powerplatform.microsoft.com"), _username.ToSecureString(), _password.ToSecureString());
                powerPlatformClient.Capacity.OpenCapacity();
                string strFileName = String.Format("CapacitySummaryOn_{0}.{1}", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"), ScreenshotImageFormat.Png);
                powerPlatformClient.Browser.ThinkTime(1000);
                powerPlatformClient.Browser.TakeWindowScreenShot(strFileName, ScreenshotImageFormat.Png);
                powerPlatformClient.Capacity.ChangeTab("Storage capacity");
                powerPlatformClient.Capacity.GetAllEnvironments();
   
            }
        }
        [TestMethod]
        public void TestExpectedToThrowException() {
            throw new Exception("This test is expected to fail to provide content for Test Runs in Azure DevOps Build results.");
        }






    }
}
