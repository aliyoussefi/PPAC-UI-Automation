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
//using Extensions = Microsoft.Dynamics365.UIAutomation.Api.UCI.Extensions;


namespace Microsoft.PowerPlatform.UIAutomation.Sample {
    [TestClass]
    public class PowerPlatformAdminCenterCapacity {
        private static string _username = "";
        private static string _password = "";
        private static BrowserType _browserType;
        private static Uri _xrmUri;
        public TestContext TestContext { get; set; }

        private static TestContext _testContext;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            _testContext = context;

            _username = _testContext.Properties["OnlineUsername"].ToString();
            _password = _testContext.Properties["OnlinePassword"].ToString();
            _xrmUri = new Uri(_testContext.Properties["OnlineCrmUrl"].ToString());
            _browserType = (BrowserType)Enum.Parse(typeof(BrowserType), _testContext.Properties["BrowserType"].ToString());
        }
        [TestMethod]
        public void TestTakeScreenshot() {
            string sessionId = Guid.NewGuid().ToString();
            using (var powerPlatformClient = new Microsoft.PowerPlatform.UIAutomation.Api.PowerPlatformAdminCenterBrowser(TestSettings.Options, new Helpers.AppInsightsLogger("2002f686-da1b-4894-974c-056bf5bb9b34", sessionId), sessionId)) {
                powerPlatformClient.OnlineLogin.Login(new Uri("https://admin.powerplatform.microsoft.com"), _username.ToSecureString(), _password.ToSecureString());
                powerPlatformClient.Capacity.OpenCapacity();
                powerPlatformClient.Capacity.ChangeTab("Storage capacity");
                powerPlatformClient.Capacity.GetAllEnvironments();
                powerPlatformClient.Capacity.ChangeEnvironment("[9.0] Ali test instance");
            }

            var client = new WebClient(TestSettings.Options);
            //using (var xrmApp1 = new PowerPlatformAdminCenterBrowser(powerPlatformClient)) {
            //    powerPlatformClient.OnlineLogin.Login(new Uri("https://admin.powerplatform.microsoft.com"), _username.ToSecureString(), _password.ToSecureString());

            //}


            //using (var xrmApp = new XrmApp(client)) {
            //    xrmApp.OnlineLogin.Login(_xrmUri, _username.ToSecureString(), _password.ToSecureString());
            //    //var powerPlatformClient = new Microsoft.PowerPlatform.UIAutomation.Api.PowerPlatformAdminCenterBrowser(TestSettings.Options);
            //    //powerPlatformClient.OnlineLogin.Login(new Uri("https://admin.powerplatform.microsoft.com"), _username.ToSecureString(), _password.ToSecureString());

            //    xrmApp.Navigation.OpenApp(UCIAppName.alyousseUci);

            //    ScreenshotImageFormat fileFormat = ScreenshotImageFormat.Tiff;  // Image Format -> Png, Jpeg, Gif, Bmp and Tiff.
            //    string strFileName = String.Format("Screenshot_{0}.{1}", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"), fileFormat);
            //    client.Browser.TakeWindowScreenShot(strFileName, fileFormat);
            //}
        }
        [TestMethod]
        public void TestExpectedToThrowException() {
            throw new Exception("This test is expected to fail to provide content for Test Runs in Azure DevOps Build results.");
        }






    }
}
