// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using OtpNet;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Threading;

namespace Microsoft.PowerPlatform.UIAutomation.Api
{
    public enum LoginResult
    {
        Success,
        Failure,
        Redirect
    }

    /// <summary>
    /// Login Page
    /// </summary>
    public class OnlineLogin
        : BrowserPage
    {
        public string[] OnlineDomains { get; set; }
        private readonly PowerPlatformAdminCenterBrowser _client;
        public OnlineLogin(PowerPlatformAdminCenterBrowser browser)
            : base()
        {
            this.OnlineDomains = Constants.Xrm.XrmDomains;
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
        //public OnlineLogin(InteractiveBrowser browser, params string[] onlineDomains)
        //    : this()
        //{
        //    this.OnlineDomains = onlineDomains;
        //}

        //public BrowserCommandResult<LoginResult> Login()
        //{
        //    return this.Login(new Uri(Constants.DefaultLoginUri));
        //}

        //public BrowserCommandResult<LoginResult> Login(SecureString username, SecureString password)
        //{
        //    return this.Execute(GetOptions("Login"), this.Login, new Uri(Constants.DefaultLoginUri), username, password, default(Action<LoginRedirectEventArgs>));
        //}

        //public BrowserCommandResult<LoginResult> Login(SecureString username, SecureString password, Action<LoginRedirectEventArgs> redirectAction)
        //{
        //    return this.Execute(GetOptions("Login"), this.Login, new Uri(Constants.DefaultLoginUri), username, password, redirectAction);
        //}

        //public BrowserCommandResult<LoginResult> Login(Uri uri)
        //{
        //    if (this.Browser.Options.Credentials.IsDefault)
        //        throw new InvalidOperationException("The default login method cannot be invoked without first setting credentials on the Browser object.");

        //    return this.Execute(GetOptions("Login"), this.Login, uri, this.Browser.Options.Credentials.Username, this.Browser.Options.Credentials.Password, default(Action<LoginRedirectEventArgs>));
        //}
        /// <summary>
        /// Login Page
        /// </summary>
        /// <param name="uri">The Uri</param>
        /// <param name="username">The Username to login to CRM application</param>
        /// <param name="password">The Password to login to CRM application</param>
        /// <example>xrmBrowser.OnlineLogin.Login(_xrmUri, _username, _password);</example>
        public BrowserCommandResult<LoginResult> Login(Uri uri, SecureString username, SecureString password, SecureString mfaSecretKey = null, Action<LoginRedirectEventArgs> redirectAction = null)
        {
            //this.Login(uri, username, password, mfaSecretKey, default(Action<LoginRedirectEventArgs>));
            return _client.Execute(GetOptions("Login"), this.Login, uri, username, password,mfaSecretKey, default(Action<LoginRedirectEventArgs>));
        }



        /// <summary>
        /// Login Page
        /// </summary>
        /// <param name="uri">The Uri</param>
        /// <param name="username">The Username to login to CRM application</param>
        /// <param name="password">The Password to login to CRM application</param>
        /// <param name="redirectAction">The RedirectAction</param>
        /// <example>xrmBrowser.OnlineLogin.Login(_xrmUri, _username, _password, ADFSLogin);</example>
        //public BrowserCommandResult<LoginResult> Login(Uri uri, SecureString username, SecureString password, SecureString mfaSecretKey = null, Action<LoginRedirectEventArgs> redirectAction = null)
        //{
        //    return this.Execute(GetOptions("Login"), this.Login, uri, username, password, mfaSecretKey, redirectAction);
        //}

        internal void ThinkTime(int milliseconds)
        {
            _client.Browser.ThinkTime(milliseconds);
        }

        private LoginResult Login(IWebDriver driver, Uri uri, SecureString username, SecureString password, SecureString mfaSecretKey = null, Action<LoginRedirectEventArgs> redirectAction = null)
        {
            //bool online = !(OnlineDomains != null && !OnlineDomains.Any(d => uri.Host.EndsWith(d)));
            driver.Navigate().GoToUrl(uri);

            //if (!online)
            //    return LoginResult.Success;

            if (driver.IsVisible(By.Id("use_another_account_link")))
                driver.ClickWhenAvailable(By.Id("use_another_account_link"));


            bool waitingForOtc = false;
            bool success = EnterUserName(driver, username);
            if (!success)
            {
                SwitchToDefaultContent(driver);
                //var isUserAlreadyLogged = IsUserAlreadyLogged();
                //if (isUserAlreadyLogged)
                //{
                //    SwitchToDefaultContent(driver);
                //    return LoginResult.Success;
                //}

                ThinkTime(1000);
                waitingForOtc = GetOtcInput(driver) != null;

                if (!waitingForOtc)
                    throw new Exception($"Login page failed. {Reference.Login.UserId} not found.");
            }

            if (!waitingForOtc)
            {
                if (driver.IsVisible(By.Id("aadTile"))) {
                    driver.FindElement(By.Id("aadTile")).Click(true);
                }
                ThinkTime(1000);

                //If expecting redirect then wait for redirect to trigger
                if (redirectAction != null)
                {
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
            do
            {
                entered = EnterOneTimeCode(driver, mfaSecretKey);
                success = ClickStaySignedIn(driver); //|| IsUserAlreadyLogged();
                attempts++;
            }
            while (!success && attempts <= Constants.DefaultRetryAttempts); // retry to enter the otc-code, if its fail & it is requested again 

            if (entered && !success)
                throw new InvalidOperationException("Something went wrong entering the OTC. Please check the MFA-SecretKey in configuration.");

            return success ? LoginResult.Success : LoginResult.Failure;
        }

      

        //private bool IsUserAlreadyLogged() => WaitForMainPage(10.Seconds());

        private static string GenerateOneTimeCode(SecureString mfaSecretKey)
        {
            // credits:
            // https://dev.to/j_sakamoto/selenium-testing---how-to-sign-in-to-two-factor-authentication-2joi
            // https://www.nuget.org/packages/Otp.NET/
            string key = mfaSecretKey?.ToUnsecureString(); // <- this 2FA secret key.

            byte[] base32Bytes = Base32Encoding.ToBytes(key);

            var totp = new Totp(base32Bytes);
            var result = totp.ComputeTotp(); // <- got 2FA coed at this time!
            return result;
        }

        private bool EnterUserName(IWebDriver driver, SecureString username)
        {
            var input = driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Login.UserId]), new TimeSpan(0, 0, 30));
            if (input == null)
                return false;

            input.SendKeys(username.ToUnsecureString());
            input.SendKeys(Keys.Enter);
            return true;
        }

        private static void EnterPassword(IWebDriver driver, SecureString password)
        {
            var input = driver.FindElement(By.XPath(Elements.Xpath[Reference.Login.LoginPassword]));
            input.SendKeys(password.ToUnsecureString());
            input.Submit();
        }

        private bool EnterOneTimeCode(IWebDriver driver, SecureString mfaSecretKey)
        {
            try
            {
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
            catch (Exception e)
            {
                var message = $"An Error occur entering OTC. Exception: {e.Message}";
                Trace.TraceInformation(message);
                throw new InvalidOperationException(message, e);
            }
        }


        private static IWebElement GetOtcInput(IWebDriver driver)
            => driver.WaitUntilAvailable(By.XPath(Elements.Xpath[Reference.Login.OneTimeCode]), TimeSpan.FromSeconds(2));

        private static bool ClickStaySignedIn(IWebDriver driver)
        {
            driver.WaitUntilVisible(By.XPath(Elements.Xpath[Reference.Login.StaySignedIn]), new TimeSpan(0, 0, 5));

            if (driver.IsVisible(By.XPath(Elements.Xpath[Reference.Login.StaySignedIn]))) {
                driver.ClickWhenAvailable(By.XPath(Elements.Xpath[Reference.Login.StaySignedIn]));

            }
            return true;
        }

        //internal bool WaitForMainPage(TimeSpan timeout, string errorMessage)
        //   => WaitForMainPage(timeout, null, () => throw new InvalidOperationException(errorMessage));

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
        //    var element = driver.WaitUntilAvailable(xpathToMainPage, timeout, successCallback, failureCallback);
        //    return element != null;
        //}

        private static void SwitchToDefaultContent(IWebDriver driver)
        {
            SwitchToMainFrame(driver);

            //Switch Back to Default Content for Navigation Steps
            driver.SwitchTo().DefaultContent();
        }

        private static void SwitchToMainFrame(IWebDriver driver)
        {
            driver.WaitForPageToLoad();
            driver.SwitchTo().Frame(0);
            driver.WaitForPageToLoad();
        }

        private void SetInputValue(IWebDriver driver, IWebElement input, string value, TimeSpan? thinktime = null)
        {
            input.SendKeys(Keys.Control + "a");
            input.SendKeys(Keys.Backspace);
            driver.WaitForTransaction();

            if (string.IsNullOrWhiteSpace(value))
            {
                input.Click(true);
                return;
            }

            if (string.IsNullOrEmpty(value))
            {
                input.Click(true);
                return;
            }

            input.SendKeys(value, true);
            driver.WaitForTransaction();
        }
    }
}