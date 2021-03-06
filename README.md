# Microsoft.PowerPlatform.UIAutomation
### Overview 
**Microsoft.PowerPlatform.UIAutomation** is a UI automation tool used for the Power Platform Admin Center.

### How Microsoft.PowerPlatform.UIAutomation works
**Microsoft.PowerPlatform.UIAutomation** is a UI automation project based on the Selenium UI Test Framework. The solution contains three projects, two of which are used as dependent assemblies for the unit test project.

The unit test project is designed to work within the Power Platform Admin Center to perform administrative tasks not currently automatable via APIs. The approach used here is not supported and is only needed as a stop gap until a proper API call is public.

### Examples

#### Collect Environment Capacity

The unit test, **CollectCapacityForAllEnvironments**, will open the Power Platform Admin Center, navigate to the Capacity area, take a screenshot of the Summary, switch to the Environment tab and proceed to collect the File, DB and Log numbers for each organization.

The unit test uses Application Insights by default but includes a **ILogger** interface to allow developers to bring their own logging solution.

### About Author
This project was created and is maintained by [Ali Youssefi](https://www.linkedin.com/in/aliyoussefi/). If you are interested in contributing, please follow the steps listed here. 

[LinkedIn Profile](https://www.linkedin.com/in/aliyoussefi/)

[YouTube channel](https://www.youtube.com/channel/UC2gUZlDx50UbOxNVTnIGz8w)

[Twitter Handle](https://twitter.com/alyousse2)