﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace Microsoft.PowerPlatform.UIAutomation.Api
{

    /// <summary>
    ///  The Home page.
    ///  </summary>
    public class Home
        : AppPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Home"/> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        public Home(InteractiveBrowser browser)
            : base(browser)
        {
        }

        /// <summary>
        /// Opens Items from the Home page
        /// </summary>
        /// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        public BrowserCommandResult<bool> MakeApp(string AppName, int thinkTime = Constants.DefaultThinkTime)
        {
            Browser.ThinkTime(thinkTime);

            return this.Execute(GetOptions("MakeApp: " + AppName), driver =>
            {

                //Find the apps on the home page
                var tiles = driver.FindElements(By.ClassName("sample-apps-list-tile-container"));

                //Navigate to the requested button
                foreach (var tile in tiles)
                {
                    var title = tile.FindElement(By.ClassName("content-element"));
                    if(title.Text.StartsWith(AppName))
                    {
                        //We found our App.  Move up one level to find the right button
                        var parent = tile.FindElement(By.XPath("../.."));

                        var button = parent.FindElement(By.TagName("button"));

                        //Hover over the tilcontent-elemente
                        tile.Hover(driver, true);
                        //Click the button
                        tile.Click();
                    }
                }

                //Wait for the main page to render
                driver.LastWindow();
                driver.WaitUntilVisible(By.XPath(Elements.Xpath[Reference.Canvas.CanvasMainPage]));

                return true;
            });
        }
    }
}
