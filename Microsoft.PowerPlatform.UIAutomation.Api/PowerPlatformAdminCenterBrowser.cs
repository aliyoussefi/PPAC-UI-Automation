using Microsoft.Dynamics365.UIAutomation.Browser;
using Microsoft.PowerPlatform.UIAutomation.Api.Helpers;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PowerPlatform.UIAutomation.Api
{
    public class PowerPlatformAdminCenterBrowser : BrowserPage, IDisposable
    {
        public List<ICommandResult> CommandResults => Browser.CommandResults;
        public Guid ClientSessionId;
        public ILogger _logger;
        public string[] OnlineDomains {
            get; set;
        }
        #region Constructor(s)

        public PowerPlatformAdminCenterBrowser(BrowserOptions options, ILogger logger, string sessionId)
        {
            Browser = new InteractiveBrowser(options);
            OnlineDomains = Constants.Xrm.XrmDomains;
            ClientSessionId = new Guid(sessionId);
            _logger = logger;
        }

        #endregion Constructor(s)
        public T GetPage<T>(PowerPlatformAdminCenterBrowser client)
        where T : BrowserPage {
            return (T)Activator.CreateInstance(typeof(T), new object[] { client });
        }

        public void Dispose() {
            throw new NotImplementedException();
        }

        public OnlineLogin OnlineLogin => this.GetPage<OnlineLogin>(this);
        public Analytics Analytics => this.GetPage<Analytics>(this);

        public Capacity Capacity => this.GetPage<Capacity>(this);
    }
}
