using System;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Hercules.Client.Abstractions.Events;
using Vostok.Logging.Hercules.Constants;

namespace Vostok.Logging.Hercules.Parsing
{
    /// <summary>
    /// A class that provides deserialization functionality for <see cref="Exception"/> data serialized to <see cref="HerculesEvent"/>.
    /// </summary>
    [PublicAPI]
    public class ExceptionData
    {
        private readonly HerculesTags tags;
        private ExceptionData[] innerExceptions;
        private StackFrameData[] stacktrace;
        
        /// <summary>
        /// The runtime type of exception.
        /// </summary>
        public string Type => tags[ExceptionTagNames.Type]?.AsString;
        
        /// <summary>
        /// The message that contains in this exception.
        /// </summary>
        public string Message => tags[ExceptionTagNames.Message]?.AsString;
        
        /// <summary>
        /// An array of <see cref="StackFrameData"/> that describes exception stacktrace. 
        /// </summary>
        public StackFrameData[] StackTrace => stacktrace ?? (stacktrace = ExtractStacktrace());
        
        /// <summary>
        /// An array of nested exceptions that contains in this exception.
        /// </summary>
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