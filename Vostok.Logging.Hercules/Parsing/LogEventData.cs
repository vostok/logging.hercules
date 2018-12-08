using System.Collections.Generic;

namespace Vostok.Logging.Hercules.Parsing
{
    public class LogEventData
    {
        public string MessageTemplate { get; }
        public string RenderedMessage { get; }
        public ExceptionData Exception { get; }
        public IDictionary<string, object> Properties { get; }
        public IDictionary<string, object> AdditionalFields { get; }
    }
}