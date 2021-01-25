using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using System.Text;

namespace EasyReproTests.Helper {
    public class EasyRepoParameters {

        public static EasyRepoParameters Initialize() {
            string username = Environment.GetEnvironmentVariable(EnviromentParameters.CRM_ENV_USER) ?? "";
            string password = Environment.GetEnvironmentVariable(EnviromentParameters.CRM_ENV_PASS) ?? "";
            string crmUrl = Environment.GetEnvironmentVariable(EnviromentParameters.CRM_ENV_URL) ?? "";
            string appInsightsInstrumentationKey = Environment.GetEnvironmentVariable(EnviromentParameters.APP_INSIGHTS_INSTRUMENTATION_KEY) ?? ""; 
            return new EasyRepoParameters(username, password, crmUrl, appInsightsInstrumentationKey);
        }

        public SecureString UserName { get; private set; }
        public SecureString Password { get; private set; }
        public Uri XmUri { get; private set; }
        public string AppInsightsInstrumentationKey { get; private set; }

        private EasyRepoParameters(string username, string password, string crmUrl, string appInsightsInstrumentationKey) {
            if (string.IsNullOrWhiteSpace(username)) {
                throw new ArgumentNullException("username");
            }
            UserName = username.ToSecureString();

            if (string.IsNullOrWhiteSpace(password)) {
                throw new ArgumentNullException("password");
            }
            Password = password.ToSecureString();

            Uri uriResult = null;
            if (string.IsNullOrWhiteSpace(crmUrl) && !Uri.TryCreate(crmUrl, UriKind.RelativeOrAbsolute, out uriResult)) {
                throw new ArgumentNullException("crmUrl");
            }
            XmUri = new Uri(crmUrl);

            if (string.IsNullOrWhiteSpace(appInsightsInstrumentationKey)) {
                Debug.WriteLine("appInsightsInstrumentationKey is null, will not write to Application Insights. Please add a key to push telemetry to the cloud.");
            }
            AppInsightsInstrumentationKey = appInsightsInstrumentationKey;
        }
    }

    public static class StringExtensions {
        public static SecureString ToSecureString(this string @string) {

            var secureString = new SecureString();

            if (@string.Length > 0) {
                foreach (var c in @string.ToCharArray())
                    secureString.AppendChar(c);
            }

            return secureString;
        }
    }
}