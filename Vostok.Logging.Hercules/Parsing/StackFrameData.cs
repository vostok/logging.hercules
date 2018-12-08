namespace Vostok.Logging.Hercules.Parsing
{
    public class StackFrameData
    {
        public string Function { get; }
        public string Type { get; }
        public string File { get; }
        public int? Line { get; }
        public int? Column { get; }
    }
}