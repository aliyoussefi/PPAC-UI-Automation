using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyReproTests.Helper {
    class CustomBrowserOptions : BrowserOptions {
        public override ChromeOptions ToChrome() {
            var chromeOptions = base.ToChrome();
            chromeOptions.AddArgument("--no-sandbox");
            // chromeOptions.AddArgument("--remote-debugging-port=922");
            // chromeOptions.AddArgument("--disable-gpu");
            // chromeOptions.AddArgument("--dump-dom");
            //chromeOptions.AddArgument("--disable-dev-shm-usage");
            //chromeOptions.AddArgument("--headless");
            return chromeOptions;
        }
    }
}