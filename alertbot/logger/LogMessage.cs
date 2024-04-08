namespace alertbot.logger
{
    public enum LogMessageType
    {
        dbg,
        err,
        inf,
        inf_urgent
    }
    public class LogMessage
    {        
        public LogMessageType Type
        {
            get; set;
        }     

        public string TAG {
            get; set;
        }
        
        public string Text {
            get; set;
        }
                
        public string Date {
            get; set;
        }

        public LogMessage(LogMessageType type, string tag, string text) { 
            TAG = tag;
            Type = type;
            Text = text;
            Date = DateTime.Now.ToString();
        }

        public override string ToString()
        {
            return $"{Date} {Type} {TAG} > {Text}";
        }

        public string ToFiltered()
        {
            return $"{Type}{TAG}{Text}".ToLower();
        }
    }

}
