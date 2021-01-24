// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Dynamics365.UIAutomation.Browser;
using Microsoft.PowerPlatform.UIAutomation.Api;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Web;
//using System.Web.Script.Serialization;

namespace Microsoft.PowerPlatform.UIAutomation.Api
{
    public class WebClient : BrowserPage
    {
        public List<ICommandResult> CommandResults => Browser.CommandResults;
        public Guid ClientSessionId;
        public WebClient(BrowserOptions options)
        {
            Browser = new InteractiveBrowser(options);
            OnlineDomains = Constants.Xrm.XrmDomains;
            ClientSessionId = Guid.NewGuid();
        }

        internal BrowserCommandOptions GetOptions(string commandName)
        {
            return new BrowserCommandOptions(Constants.DefaultTraceSource,
                commandName,
                0,
                0,
                null,
                true,
                typeof(NoSuchElementException), typeof(StaleElementReferenceException));
        }

        internal BrowserCommandResult<bool> InitializeTestMode(bool onlineLoginPath = false)
        {
            return this.Execute(GetOptions("Initialize Unified Interface TestMode"), driver =>
            {
                var uri = driver.Url;
                var queryParams = "&flags=testmode=true,easyreproautomation=true";

                if (!uri.Contains(queryParams))
                {
                    var testModeUri = uri + queryParams;

                    driver.Navigate().GoToUrl(testModeUri);

                    driver.WaitForPageToLoad();

                    if (!onlineLoginPath)
                    {
                        driver.WaitForTransaction();
                    }
                }

                return true;
            });
        }

        internal BrowserCommandResult<bool> InitializeModes()
        {
            return this.Execute(GetOptions("Initialize Unified Interface Modes"), driver =>
            {
                driver.WaitForPageToLoad();

                var uri = driver.Url;
                var queryParams = "&flags=easyreproautomation=true";

                if (Browser.Options.UCITestMode) queryParams += ",testmode=true";
                if (Browser.Options.UCIPerformanceMode) queryParams += "&perf=true";

                if (!uri.Contains(queryParams) && !uri.Contains(HttpUtility.UrlEncode(queryParams)))
                {
                    var testModeUri = uri + queryParams;

                    driver.Navigate().GoToUrl(testModeUri);
                }

                //WaitForMainPage();

                return true;
            });
        }
        #region PageWaits
        //internal bool WaitForMainPage(TimeSpan timeout, string errorMessage)
        //    => WaitForMainPage(timeout, null, () => throw new InvalidOperationException(errorMessage));

        //internal bool WaitForMainPage(TimeSpan? timeout = null, Action<IWebElement> successCallback = null, Action failureCallback = null)
        //{
        //    IWebDriver driver = Browser.Driver;
        //    timeout = timeout ?? Constants.DefaultTimeout;
        //    successCallback = successCallback ?? (
        //                          _ =>
        //                          {
        //                              bool isUCI = driver.HasElement(By.XPath(Elements.Xpath[Reference.Login.CrmUCIMainPage]));
        //                              if (isUCI)
        //                                  driver.WaitForTransaction();
        //                          });

        //    var xpathToMainPage = By.XPath(Elements.Xpath[Reference.Login.CrmMainPage]);
        //    var element = driver.WaitUntilVisible(xpathToMainPage, timeout, successCallback, failureCallback);
        //    return element != null;
        //}

        #endregion
        public string[] OnlineDomains { get; set; }



        //public void LoginTimeOutExceptionRetry(Exception ex)
        //{
        //    if (ex.GetType() == typeof(mfaLoginException)) {
        //        //LoginWithUsernameAndPassword()
        //    }
        //}
        #region PageWaits
        internal bool WaitForMainPage(TimeSpan timeout, string errorMessage)
            => WaitForMainPage(timeout, null, () => throw new InvalidOperationException(errorMessage));

        internal bool WaitForMainPage(TimeSpan? timeout = null, Action<IWebElement> successCallback = null, Action failureCallback = null) {
            IWebDriver driver = Browser.Driver;
            timeout = timeout ?? Constants.DefaultTimeout;
            successCallback = successCallback ?? (
                                  _ =>
                                  {
                                      bool isUCI = driver.HasElement(By.XPath(Elements.Xpath[Reference.Login.CrmUCIMainPage]));
                                      if (isUCI)
                                          driver.WaitForTransaction();
                                  });

            var xpathToMainPage = By.XPath(Elements.Xpath[Reference.Login.CrmMainPage]);
            var element = driver.WaitUntilAvailable(xpathToMainPage, timeout, successCallback, failureCallback);
            return element != null;
        }

        #endregion
        #region Login

        internal BrowserCommandResult<LoginResult> Login(Uri uri) {
            var username = Browser.Options.Credentials.Username;
            if (username == null)
                return PassThroughLogin(uri);

            var password = Browser.Options.Credentials.Password;
            return Login(uri, username, password);
        }

        internal BrowserCommandResult<LoginResult> Login(Uri orgUri, SecureString username, SecureString password, SecureString mfaSecretKey = null, Action<LoginRedirectEventArgs> redirectAction = null) {
            return Execute(GetOptions("Login"), Login, orgUri, username, password, mfaSecretKey, redirectAction);
        }

        private LoginResult Login(IWebDriver driver, Uri uri, SecureString username, SecureString password, SecureString mfaSecretKey = null, Action<LoginRedirectEventArgs> redirectAction = null) {
            bool online = !(OnlineDomains != null && !OnlineDomains.Any(d => uri.Host.EndsWith(d)));
            driver.Navigate().GoToUrl(uri);

            if (!online)
                return LoginResult.Success;

            driver.ClickIfVisible(By.Id("use_another_account_link"));

            bool waitingForOtc = false;
            bool success = EnterUserName(driver, username);
            if (!success) {
                var isUserAlreadyLogged = IsUserAlreadyLogged();
                if (isUserAlreadyLogged) {
                    SwitchToDefaultContent(driver);
                    return LoginResult.Success;
                }

                ThinkTime(1000);
                waitingForOtc = GetOtcInput(driver) != null;

                if (!waitingForOtc)
                    throw new Exception($"Login page failed. {Reference.Login.UserId} not found.");
            }

            if (!waitingForOtc) {
                driver.ClickIfVisible(By.Id("aadTile"));
                ThinkTime(1000);

                //If expecting redirect then wait for redirect to trigger
                if (redirectAction != null) {
                    //Wait for redirect to occur.
                    ThinkTime(3000);

                    redirectAction.Invoke(new LoginRedirectEventArgs(username, password, driver));
                    return LoginResult.Redirect;
                }

                EnterPassword(driver, password);
                ThinkTime(1000);
            }

            int attempts = 0;
            bool entered;
            do {
                entered = EnterOneTimeCode(driver, mfaSecretKey);
                success = ClickStaySignedIn(driver) || IsUserAlreadyLogged();
                attempts++;
            }
            while (!success && attempts <= Constants.DefaultRetryAttempts); // retry to enter the otc-code, if its fail & it is requested again 

            if (entered && !success)
                throw new InvalidOperationException("Something went wrong entering the OTC. Please check the MFA-SecretKey in configuration.");

            return success ? LoginResult.Success : LoginResult.Failure;
        }

        private bool IsUserAlreadyLogged() => WaitForMainPage(10.Seconds());

        private static string GenerateOneTimeCode(SecureString mfaSecretKey) {
            // credits:
            // https://dev.to/j_sakamoto/selenium-testing---how-to-sign-in-to-two-factor-authentication-2joi
            // https://www.nuget.org/packages/Otp.NET/
            string key = mfaSecretKey?.ToUnsecureString(); // <- this 2FA secret key.

            byte[] base32Bytes = Base32Encoding.ToBytes(key);

            var totp = new Totp(base32Bytes);
            var result = totp.ComputeTotp(); // <- got 2FA coed at this time!
            return result;
        }

        private bool EnterUserName(IWebDriver driver, SecureString username) {
            var input = driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Login.UserId]), new TimeSpan(0, 0, 30));
            if (input == null)
                return false;

            input.SendKeys(username.ToUnsecureString());
            input.SendKeys(Keys.Enter);
            return true;
        }

        private static void EnterPassword(IWebDriver driver, SecureString password) {
            var input = driver.FindElement(By.XPath(Elements.Xpath[Reference.Login.LoginPassword]));
            input.SendKeys(password.ToUnsecureString());
            input.Submit();
        }

        private bool EnterOneTimeCode(IWebDriver driver, SecureString mfaSecretKey) {
            try {
                IWebElement input = GetOtcInput(driver); // wait for the dialog, even if key is null, to print the right error
                if (input == null)
                    return true;

                if (mfaSecretKey == null)
                    throw new InvalidOperationException("The application is wait for the OTC but your MFA-SecretKey is not set. Please check your configuration.");

                var oneTimeCode = GenerateOneTimeCode(mfaSecretKey);
                SetInputValue(driver, input, oneTimeCode, 1.Seconds());
                input.Submit();
                return true; // input found & code was entered
            }
            catch (Exception e) {
                var message = $"An Error occur entering OTC. Exception: {e.Message}";
                Trace.TraceInformation(message);
                throw new InvalidOperationException(message, e);
            }
        }

        private void SetInputValue(IWebDriver driver, IWebElement input, string value, TimeSpan? thinktime = null) {
            // Repeat set value if expected value is not set
            // Do this to ensure that the static placeholder '---' is removed 
            driver.RepeatUntil(() =>
            {
                input.Clear();
                input.Click();
                input.SendKeys(Keys.Control + "a");
                input.SendKeys(Keys.Control + "a");
                input.SendKeys(Keys.Backspace);
                input.SendKeys(value);
                driver.WaitForTransaction();
            },
                d => input.GetAttribute("value").IsValueEqualsTo(value),
                TimeSpan.FromSeconds(9), 3,
                failureCallback: () => throw new InvalidOperationException($"Timeout after 10 seconds. Expected: {value}. Actual: {input.GetAttribute("value")}")
            );

            driver.WaitForTransaction();
        }
        private static IWebElement GetOtcInput(IWebDriver driver)
            => driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Login.OneTimeCode]), TimeSpan.FromSeconds(2));

        private static bool ClickStaySignedIn(IWebDriver driver) {
            var xpath = By.XPath(Elements.Xpath[Reference.Login.StaySignedIn]);
            var element = driver.ClickIfVisible(xpath, 5.Seconds());
            return element != null;
        }

        private static void SwitchToDefaultContent(IWebDriver driver) {
            SwitchToMainFrame(driver);

            //Switch Back to Default Content for Navigation Steps
            driver.SwitchTo().DefaultContent();
        }

        private static void SwitchToMainFrame(IWebDriver driver) {
            driver.WaitForPageToLoad();
            driver.SwitchTo().Frame(0);
            driver.WaitForPageToLoad();
        }

        internal BrowserCommandResult<LoginResult> PassThroughLogin(Uri uri) {
            return this.Execute(GetOptions("Pass Through Login"), driver =>
            {
                driver.Navigate().GoToUrl(uri);

                WaitForMainPage(60.Seconds(),
                    _ =>
                    {
                        //determine if we landed on the Unified Client Main page
                        var isUCI = driver.HasElement(By.XPath(Elements.Xpath[Reference.Login.CrmUCIMainPage]));
                        if (isUCI) {
                            driver.WaitForPageToLoad();
                            driver.WaitForTransaction();
                        }
                        else
                            //else we landed on the Web Client main page or app picker page
                            SwitchToDefaultContent(driver);
                    },
                    () => new InvalidOperationException("Load Main Page Fail.")
                );

                return LoginResult.Success;
            });
        }


        public void ADFSLoginAction(LoginRedirectEventArgs args) {
            //Login Page details go here.  You will need to find out the id of the password field on the form as well as the submit button. 
            //You will also need to add a reference to the Selenium Webdriver to use the base driver. 
            //Example

            var driver = args.Driver;

            driver.FindElement(By.Id("passwordInput")).SendKeys(args.Password.ToUnsecureString());
            driver.ClickWhenAvailable(By.Id("submitButton"), TimeSpan.FromSeconds(2));

            //Insert any additional code as required for the SSO scenario

            //Wait for CRM Page to load
            WaitForMainPage(TimeSpan.FromSeconds(60), "Login page failed.");
            SwitchToMainFrame(driver);
        }

        public void MSFTLoginAction(LoginRedirectEventArgs args) {
            //Login Page details go here.  You will need to find out the id of the password field on the form as well as the submit button. 
            //You will also need to add a reference to the Selenium Webdriver to use the base driver. 
            //Example

            var driver = args.Driver;

            //d.FindElement(By.Id("passwordInput")).SendKeys(args.Password.ToUnsecureString());
            //d.ClickWhenAvailable(By.Id("submitButton"), TimeSpan.FromSeconds(2));

            //This method expects single sign-on

            ThinkTime(5000);

            driver.WaitUntilVisible(By.XPath("//div[@id=\"mfaGreetingDescription\"]"));

            var azureMFA = driver.FindElement(By.XPath("//a[@id=\"WindowsAzureMultiFactorAuthentication\"]"));
            azureMFA.Click(true);

            Thread.Sleep(20000);

            //Insert any additional code as required for the SSO scenario

            //Wait for CRM Page to load
            WaitForMainPage(TimeSpan.FromSeconds(60), "Login page failed.");
            SwitchToMainFrame(driver);
        }

        #endregion

        public BrowserCommandOptions OpenAppBrowserCommandOptions()
        {
            return new BrowserCommandOptions()
            {
                //CommandName = "Open SC App",
                //Ex

            };
        }



        internal void ThinkTime(int milliseconds)
        {
            Browser.ThinkTime(milliseconds);
        }
        internal void Dispose()
        {
            Browser.Dispose();
        }
    }
}
