using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PowerPlatform.UIAutomation.Api.Helpers {
    public interface ILogger {
        void Log(string message);
        void Log(Dictionary<string, string> message);
        void Log(string message, DateTime timestamp);
        void Log(Exception ex, bool throwEx);
    }
}
