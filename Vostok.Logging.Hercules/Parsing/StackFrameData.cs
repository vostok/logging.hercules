using JetBrains.Annotations;
using Vostok.Hercules.Client.Abstractions.Events;
using Vostok.Logging.Hercules.Constants;

namespace Vostok.Logging.Hercules.Parsing
{
    [PublicAPI]
    public class StackFrameData
    {
        private readonly HerculesTags tags;

        internal StackFrameData(HerculesTags tags)
        {
            this.tags = tags;
        }

        public string Function => tags[StackFrameTagNames.Function]?.AsString;
        public string Type => tags[StackFrameTagNames.Type]?.AsString;
        public string File => tags[StackFrameTagNames.File]?.AsString;
        public int? Line => tags[StackFrameTagNames.Line]?.AsInt;
        public int? Column => tags[StackFrameTagNames.Column]?.AsInt;
    }
}