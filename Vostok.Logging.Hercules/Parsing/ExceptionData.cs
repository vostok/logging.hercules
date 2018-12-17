using System.Linq;
using JetBrains.Annotations;
using Vostok.Hercules.Client.Abstractions.Events;
using Vostok.Logging.Hercules.Constants;

namespace Vostok.Logging.Hercules.Parsing
{
    [PublicAPI]
    public class ExceptionData
    {
        private readonly HerculesTags tags;
        private ExceptionData[] innerExceptions;
        private StackFrameData[] stacktrace;
        
        public string Type => tags[ExceptionTagNames.Type]?.AsString;
        public string Message => tags[ExceptionTagNames.Message]?.AsString;
        public StackFrameData[] StackTrace => stacktrace ?? (stacktrace = ExtractStacktrace());
        public ExceptionData[] InnerExceptions => innerExceptions ?? (innerExceptions = ExtractInnerExceptions());

        private ExceptionData(HerculesTags tags)
        {
            this.tags = tags;
        }

        internal static ExceptionData FromTags(HerculesTags tags)
        {
            return tags == null
                ? null
                : new ExceptionData(tags);
        }

        private ExceptionData[] ExtractInnerExceptions()
        {
            var vector = tags[ExceptionTagNames.InnerExceptions]?.AsVector;

            return vector?.AsContainerList.Select(FromTags).ToArray();
        }

        private StackFrameData[] ExtractStacktrace()
        {
            var vector = tags[ExceptionTagNames.StackTrace]?.AsVector;

            return vector?.AsContainerList.Select(x => new StackFrameData(x)).ToArray();
        }
    }
}