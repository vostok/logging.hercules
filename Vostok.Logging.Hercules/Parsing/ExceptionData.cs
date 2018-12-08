namespace Vostok.Logging.Hercules.Parsing
{
    public class ExceptionData
    {
        public string Type { get; }
        public string Message { get; }
        public StackFrameData[] StackTrace { get; }
    }
}