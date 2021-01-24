// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PowerPlatform.UIAutomation.Api
{
    public static class Elements
    {
        public static Dictionary<string, string> Xpath = new Dictionary<string, string>()
        {
            //Capacity
            {"Capacity_OpenResources","//button[@aria-label=\"Resources\"]" },
            {"Capacity_OpenCapacity","//a[@title=\"Capacity\"]" },
            //Capacity
            {"Capacity_ChangeTab","//div[@class=\"orginsights-sub-nav\"]" },
            //{"Capacity_Capacity","//a[@title=\"Capacity\"]" },
            //
            //Analytics
            {"Analytics_ChangeFilters","//span[@class=\"orginsights-change-filters\"]" },
            {"Analytics_ChangeTab","//div[@class=\"orginsights-sub-nav\"]" },
            {"Analytics_OrgInsightsChart","//div[contains(@aria-label=\"[NAME]\")]" },
            {"Analytics_CommandBar","//button[contains(@name=\"[NAME]\")]" },
            {"Analytics_OpenAnalytics","//button[@aria-label=\"Analytics\"]" },
            {"Analytics_CommonDataService","//a[@title=\"Dataverse\"]" },
            {"Analytics_PowerAutomate","//a[@title=\"Power Automate\"]" },
            {"Analytics_PowerApps","//a[@title=\"Power Apps\"]" },

            {"Analytics_ChangeEnvironment","//div[@aria-label=\"Select an Environment\"]" },
            {"Analytics_EnvironmentName", "//button[@title=\"[NAME]\"]" },

            {"Analytics_FiltersFromDate", "//input[@aria-label='From']" },
            {"Analytics_FiltersDateValue", "//button[@aria-label=\"[NAME]\"]" },
            {"Analytics_FiltersFromTime", "//div[@data-testid='fromTimeDropdown']" },
            {"Analytics_FiltersToDate", "//input[@aria-label='To']" },
            {"Analytics_FiltersToTime", "//div[@data-testid='toTimeDropdown']" },

            //Dropdown402-list
            {"Analytics_FiltersToTimeTimeValue", "//button[@title=\"[NAME]\"]" },
            {"Analytics_FiltersApplyButton", "//button[@data-testid=\"btnApply\"]" },

            //Download
            {"Analytics_DownloadButton", "//button[@name=\"Download\"]" },
            {"Analytics_DownloadReportName", "//button[@aria-label=\"[NAME]\"]" },

            //OrgInsights Body
            {"OrgInsights_TabNav", "//div[@class=\"orginsights-sub-nav\"]" },
            {"OrgInsights_ActiveTab", "//a[@class=\"orginsights-sub-nav-item orginsights-sub-nav-item-active\"]" },
            {"OrgInsights_SelectedTab", "//a[text()=\"[NAME]\"]" },


            //OrgInsights Home
            {"OrgInsights_Body", "//div[@class=\"orginsights-body\"]" },
            {"OrgInsights_Home_ActiveUsers", "//div[@aria-label=\"Active users\"]" },
            {"OrgInsights_Home_ApiCalls", "//div[@aria-label=\"Total API calls\"]" },
            {"OrgInsights_Home_ApiSuccess", "//div[@aria-label=\"API success rate\"]" },
            {"OrgInsights_Home_PluginExecutions", "//div[@aria-label=\"Plug-in executions\"]" },

            #region unused
            //Login           
            { "Login_UserId", "//input[@type='email']"},
            { "Login_Password", "//input[@type='password']"},
            { "Login_SignIn", "id(\"cred_sign_in_button\")"},
            { "Login_MainPage", "//div[contains(@class,\"home-page-component\")]"},
            { "Login_SharePointPage", "id(\"O365_NavHeader\")"},
                        { "Login_CrmMainPage", "//*[contains(@id,'crmTopBar') or contains(@data-id,'topBar')]"},
            { "Login_CrmUCIMainPage", "//*[contains(@data-id,'topBar')]"},
            { "Login_StaySignedIn", "//input[@id=\"idSIButton9\"]"},
            { "Login_OneTimeCode", "//input[@name='otc']"},

            //Canvas
            { "Canvas_Shell", "//*[@id=\"shell-root\"]" },
            { "Canvas_AppManagementPage", "//section[contains(@class, 'app-management-page')]" },
            { "Canvas_Skip", "//*[@id=\"rootBody\"]/div[1]/div[2]/div[2]/div/button" },
            { "Canvas_ColorWhite", "//button[@title=\"RGBA(255, 255, 255, 1)\"]" },
            { "Canvas_ColorBlue", "//button[@title=\"RGBA(0, 120, 215, 1)\"]" },
            { "Canvas_ExistingColor", "//div[@class=\"appmagic-borderfill-container\"]" },
            { "Canvas_ClickButton", "//button[@aria-label=\"[NAME]\"]" },

            //Navigation
            { "Navigation_HomePage", "//*[@class=\"home-page\"]" },
            { "Navigation_ChangeEnvironmentButton", "//button[contains(@class, 'groups-menu-toggle')]" },
            { "Navigation_ChangeEnvironmentList", "//div[contains(@class, 'action-menu-panel')]" },

            //TestFramework
            { "TestFramework_ToastMessage", "//*[@class=\"toast-message\"]" },

            //Backstage
            { "Backstage_MenuContainer","//div[contains(@class, 'backstage-nav')]" },
            { "Backstage_OpenFileMenu","//button[contains(@id, 'appmagic-file-tab')]" },
            { "Backstage_Description", "//textarea[@aria-label='Description']" },

            //Dialog
            {"Permission_Dialog", "//span[@id=\"app-permissions-dialog-message\"]" },
            {"Allow_Dialog", "//button[contains(@class, 'dialog-button')]" },
            {"Override_Dialog", "//button[contains(@class, 'dialog pa__dialog overlay')]" },
            {"Override_DialogButton", "//button[contains(text(),'Override')]"},

            //Sidebar
            { "Navigation_AppLink", "//a[contains(text(),'[NAME]')]"},
            { "Navigation_AppCommandBarButton", "//span[contains(@title, '[NAME]')]"},
            { "Navigation_CanvasPageLoaded", "//div[contains(@class,'ribbon-tab-bar-parent-container')]"},
            { "Navigation_SaveButton", "//button[contains(text(), 'Save')]"},
            { "Navigation_SaveButtonSuccess", "//div[contains(text(),'All changes are saved')]"},
            { "Navigation_PublishButton", "//button[contains(@aria-label,'Publish')]"},
            { "Navigation_PublishToSharePointButton", "//button[contains(@aria-label,'Publish to SharePoint')]"},
            { "Navigation_PublishToSharePointDialogButton", "//button[contains(text(),'Publish to SharePoint')]"},
            { "Navigation_PublishButtonSuccess", "//div[contains(text(),'All changes are saved')]"},
            { "Navigation_PublishVerifyButton", "//button[contains(text(), 'Publish this version')]"},
            { "Navigation_PublishVerifyButtonSuccess", "//div[contains(text(),'All changes are saved and published')]"},
            { "Navigation_SwitchDesignMode", "//div[contains(@class, 'react-sidebar-mode-switcher')]"},
            { "Navigation_DesignModeButton", "//button[contains(@title, '[NAME]')]"},
            { "Navigation_Sidebar", "//div[contains(@class, 'react-sidebar-app')]"},
            { "Navigation_ShellSidebar", "//div[contains(@class, 'ba-Sidebar')]"},


            //SharePoint
            { "Close_App", "//button[contains(@aria-label,'Close')]/i"},
            { "Click_Button", "//button[@name=\"[NAME]\"]" },
            { "Click_SubButton", "//button[@name=\"[NAME]\"]" },

            //CommandBar
            { "CommandBar_Container","//div[contains(@class,'ba-CommandBar')]"},
            { "CommandBar_CancelSolutionCheckerButton","//button[contains(@class,'listItemCancelButton')]"},
            { "CommandBar_CancelSolutionCheckerSolutionList","//div[@class=\"ms-List\" and @role=\"list\"]"},
            { "CommandBar_SubButtonContainer", "//ul[contains(@class,'ms-ContextualMenu')]" },
            { "CommandBar_OverflowContainer", "//div[contains(@class,'ms-OverflowSet-overflowButton')]" },
            { "CommandBar_ContextualMenuList", "//ul[contains(@class,'ms-ContextualMenu-list')]" },
            { "CommandBar_GridSolutionNameColumn", "//div[@data-automation-key='name']"},
            { "CommandBar_GridSolutionStatusColumn", "//div[contains(@data-automation-key,'solutionChecker')]"},


            //ModelDrivenApps
            { "ModelDrivenApps_CellsContainer", "//div[contains(@data-automationid,'DetailsRowCell')]"},
            { "ModelDrivenApps_NotificationContainer", "//div[@class=\"notification-container\"]"},
            { "ModelDrivenApps_MoreCommandsButton", "../div[contains(@data-automation-key, 'contextualMenu')]"},
            { "ModelDrivenApps_MoreCommandsContainer", "//ul[contains(@class,'ms-ContextualMenu-list')]"},
            { "ModelDrivenApps_SubButtonContainer", "//ul[contains(@class,'ms-ContextualMenu-list')]"},
            #endregion
        };

        public static Dictionary<string, string> ElementId = new Dictionary<string, string>()
        {

        };

        public static Dictionary<string, string> CssClass = new Dictionary<string, string>()
        {

        };

        public static Dictionary<string, string> Name = new Dictionary<string, string>()
        {

        };
    }

    public static class Reference
    {
        public static class OrgInsights_Home
        {
            public static string ActiveUsers = "OrgInsights_Home_ActiveUsers";
            public static string ApiCalls = "OrgInsights_Home_ApiCalls";
            public static string ApiSuccess = "OrgInsights_Home_ApiSuccess";
            public static string PluginExecutions = "OrgInsights_Home_PluginExecutions";
        }

        public static class OrgInsights
        {
            public static string ActiveTab = "OrgInsights_ActiveTab";
            public static string SelectedTab = "OrgInsights_SelectedTab";
            public static string TabNav = "OrgInsights_TabNav";
        }
        public static class Capacity {
            public static string OpenResources = "Capacity_OpenResources";
            public static string OpenCapacity = "Capacity_OpenCapacity";
            public static string ChangeTab = "Capacity_ChangeTab";
        }
        public static class Analytics
        {

            public static string DownloadButton = "Analytics_DownloadButton";
            public static string DownloadReportName = "Analytics_DownloadReportName";
            public static string FilterApply = "Analytics_FiltersApplyButton";
            public static string FilterFromDate = "Analytics_FiltersFromDate";
            public static string FilterDateValue = "Analytics_FiltersDateValue";
            public static string FilterFromTime = "Analytics_FiltersFromTime";
            public static string FilterToDate = "Analytics_FiltersToDate";
            public static string FilterToTime = "Analytics_FiltersToTime";
            public static string FilterToTimePicklist = "Analytics_FiltersToTimeTime";
            public static string FilterToTimePicklistValue = "Analytics_FiltersToTimeTimeValue";
            //public static string ChangeFilters = "Analytics_ChangeFilters";

            public static string ChangeFilters = "Analytics_ChangeFilters";
            public static string ChangeEnvironment = "Analytics_ChangeEnvironment";
            public static string EnvironmentName = "Analytics_EnvironmentName";
            public static string ChangeTab = "Analytics_ChangeTab";
            public static string OrgInsightsChart = "Analytics_OrgInsightsChart";
            public static string CommandBar = "Analytics_CommandBar";
            public static string OpenAnalytics = "Analytics_OpenAnalytics";
            public static string CommonDataService = "Analytics_CommonDataService";
            public static string PowerAutomate = "Analytics_PowerAutomate";
            public static string PowerApps = "Analytics_PowerApps";

            
        }
        public static class Login
        {
            public static string UserId = "Login_UserId";
            public static string LoginPassword = "Login_Password";
            public static string SignIn = "Login_SignIn";
            public static string MainPage = "Login_MainPage";
            public static string CrmMainPage = "Login_CrmMainPage";
            public static string CrmUCIMainPage = "Login_CrmUCIMainPage";
            public static string StaySignedIn = "Login_StaySignedIn";
            public static string OneTimeCode = "Login_OneTimeCode";
            public static string SharePointPage = "Login_SharePointPage";
            public static int SignInAttempts = 3;
        }
        public static class Canvas
        {
            public static string CanvasMainPage = "Canvas_Shell";
            public static string AppManagementPage = "Canvas_AppManagementPage";
            public static string CanvasSkipButton = "Canvas_Skip";
            public static string ColorWhite = "Canvas_ColorWhite";
            public static string ColorBlue = "Canvas_ColorBlue";
            public static string ExistingColor = "Canvas_ExistingColor";
            public static string ClickButton = "Canvas_ClickButton";
        }
        public static class Navigation
        {
            public static string AppLink = "Navigation_AppLink";
            public static string AppCommandBarButton = "Navigation_AppCommandBarButton";
            public static string CanvasPageLoaded = "Navigation_CanvasPageLoaded";
            public static string SaveButton = "Navigation_SaveButton";
            public static string SaveButtonSuccess = "Navigation_SaveButtonSuccess";
            public static string PublishButton = "Navigation_PublishButton";
            public static string PublishToSharePointButton = "Navigation_PublishToSharePointButton";
            public static string PublishToSharePointDialogButton = "Navigation_PublishToSharePointDialogButton";
            public static string PublishVerifyButton = "Navigation_PublishVerifyButton";
            public static string PublishVerifyButtonSuccess = "Navigation_PublishVerifyButtonSuccess";
            public static string SwitchDesignMode = "Navigation_SwitchDesignMode";
            public static string DesignModeButton = "Navigation_DesignModeButton";
            public static string Sidebar = "Navigation_Sidebar";
            public static string ShellSidebar = "Navigation_ShellSidebar";
            public static string HomePage = "Navigation_HomePage";
            public static string ChangeEnvironmentButton = "Navigation_ChangeEnvironmentButton";
            public static string ChangeEnvironmentList = "Navigation_ChangeEnvironmentList";
            public static string CreateAnApp = "Create an app";
            public static string CreateBlankAppPhoneButtonId = "blank-app-icon-container-id-phone";
            public static string CreateAppFromTemplateButtonId = "app-from-template-icon-container-id-phone";
            public static string CreateAppFromTemplateServiceDeskButtonId = "/providers/Microsoft.PowerApps/galleries/public/items/Microsoft.ServiceDeskNew.0.2.0";
            public static string CreateAppFromTemplateChooseButtonId = "templates-choose-button-id";
            public static string CreateAppFromSPOListButtonId = "shared_sharepointonline-phone";
            public static string CreateAppFromSPOListSPURLClassName = "new-site-text-input";
            public static string CreateAppFromDataNewSiteButtonClassName = "new-site-button";
            public static string CreateAppFromDataSPConnectButtonClassName = "data-sources-pane-connect-button";
            
        }

        public static class TestFramework
        {
            public static string ToastMessage = "TestFramework_ToastMessage";
        }

        public static class BackStage
        {
            public static string MenuContainer = "Backstage_MenuContainer";
            public static string OpenFileMenu = "Backstage_OpenFileMenu";
            public static string Description = "Backstage_Description";
        }

        public static class Dialog
        {
            public static string PermissionsDialog = "Permission_Dialog";
            public static string AllowDialog = "Allow_Dialog";
            public static string OverrideDialog = "Override_Dialog";
            public static string OverrideDialogButton = "Override_DialogButton";
        }

        public static class SharePoint
        {
            public static string CloseApp = "Close_App";
            public static string AllowDialog = "Allow_Dialog";
            public static string ClickButton = "Click_Button";
            public static string ClickSubButton = "Click_SubButton";
        }

        public static class CommandBar
        {
            public static string Container = "CommandBar_Container";
            public static string CancelSolutionCheckerButton = "CommandBar_CancelSolutionCheckerButton";
            public static string CancelSolutionCheckerSolutionList = "CommandBar_CancelSolutionCheckerSolutionList";
            public static string SubButtonContainer = "CommandBar_SubButtonContainer";
            public static string OverflowContainer = "CommandBar_OverflowContainer";
            public static string ContextualMenuList = "CommandBar_ContextualMenuList";
            public static string GridSolutionNameColumn = "CommandBar_GridSolutionNameColumn";
            public static string GridSolutionStatusColumn = "CommandBar_GridSolutionStatusColumn";
        }

        public static class ModelDrivenApps
        {
            public static string CellsContainer = "ModelDrivenApps_CellsContainer";
            public static string NotificationContainer = "ModelDrivenApps_NotificationContainer";
            public static string MoreCommandsButton = "ModelDrivenApps_MoreCommandsButton";
            public static string MoreCommandsContainer = "ModelDrivenApps_MoreCommandsContainer";
            public static string SubButtonContainer = "ModelDrivenApps_SubButtonContainer";
        }

    }
}

