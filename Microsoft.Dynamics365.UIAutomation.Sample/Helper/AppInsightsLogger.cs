using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.PowerPlatform.UIAutomation.Api.Helpers;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PowerPlatform.UIAutomation.Sample.Helpers {
    public class AppInsightsLogger : ILogger {
        private ApplicationInsights.TelemetryClient _client;
        private string _sessionId;
        public AppInsightsLogger(string instrumentationKey, string sessionId) {
            _client = new ApplicationInsights.TelemetryClient()
            {
                InstrumentationKey = instrumentationKey
            };
            _client.Context.Session.Id = _sessionId;
            _client.Context.Cloud.RoleName = "Capacity-Reports";
            _sessionId = sessionId;
        }

        public void Log(string message) {
            throw new NotImplementedException();
        }

        public void Log(string message, DateTime timestamp) {
            throw new NotImplementedException();
        }

        public void Log(Exception ex, bool throwEx) {
            throw new NotImplementedException();
        }

        public void Log(Dictionary<string, string> message) {
            EventTelemetry customEvent = new EventTelemetry();
            customEvent.Name = "Capacity Report for Enviroment " + message["Org"];
            foreach (var x in message) {
                customEvent.Properties.Add(x);
            }
            _client.TrackEvent(customEvent);
        }
    }
}
