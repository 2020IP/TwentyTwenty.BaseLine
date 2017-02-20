using System;

namespace TwentyTwenty.BaseLine
{
    public class TwentyTwentyException : Exception
    {
        public TwentyTwentyException() { }

        public TwentyTwentyException(string message) : base(message) { }

        public TwentyTwentyException(string message, Exception inner) : base(message, inner) { }

        public TwentyTwentyException(int errorCode, string message, params object[] parameters)
            : base(message)
        {
            ErrorCode = errorCode;
            Parameters = parameters;
        }
        
        public TwentyTwentyException(int errorCode, string message, Exception inner, params object[] parameters)
            : base(message, inner)
        {
            ErrorCode = errorCode;
            Parameters = parameters;
        }

        public int ErrorCode { get; } = -0;
        public object[] Parameters { get; set; }
    }
}