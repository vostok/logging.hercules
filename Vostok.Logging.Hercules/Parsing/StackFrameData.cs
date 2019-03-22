using System;
using JetBrains.Annotations;
using Vostok.Hercules.Client.Abstractions.Events;
using Vostok.Logging.Hercules.Constants;

namespace Vostok.Logging.Hercules.Parsing
{
    /// <summary>
    /// A class that provides deserialization functionality for <see cref="Exception"/> stacktrace in <see cref="LogEventData"/>.
    /// </summary>
    [PublicAPI]
    public class StackFrameData
    {
        private readonly HerculesTags tags;

        internal StackFrameData(HerculesTags tags)
        {
            this.tags = tags;
        }

        /// <summary>
        /// the method in which the frame is executing.
        /// </summary>
        [CanBeNull]
        public string Function => tags[StackFrameTagNames.Function]?.AsString;

        /// <summary>
        /// The type where <see cref="Function"/> is declared.
        /// </summary>
        [CanBeNull]
        public string Type => tags[StackFrameTagNames.Type]?.AsString;

        /// <summary>
        /// The file name that contains the code that is executing in this frame.
        /// </summary>
        [CanBeNull]
        public string File => tags[StackFrameTagNames.File]?.AsString;

        /// <summary>
        /// The line number in the file that contains the code that is executing in this frame.
        /// </summary>
        [CanBeNull]
        public int? Line => tags[StackFrameTagNames.Line]?.AsInt;

        /// <summary>
        /// The column number in the file that contains the code that is executing in this frame.
        /// </summary>
        [CanBeNull]
        public int? Column => tags[StackFrameTagNames.Column]?.AsInt;
    }
}