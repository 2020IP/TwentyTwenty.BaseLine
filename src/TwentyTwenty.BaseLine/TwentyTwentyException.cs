using System;

namespace TwentyTwenty.BaseLine
{
    public class TwentyTwentyException : Exception
    {
        public TwentyTwentyException() { }

        public TwentyTwentyException(string message) : base(message) { }

        public TwentyTwentyException(string message, Exception inner) : base(message, inner) { }

        public TwentyTwentyException(int errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }
        
        public TwentyTwentyException(int errorCode, string message, Exception inner)
            : base(message, inner)
        {
            ErrorCode = errorCode;
        }

        public int ErrorCode { get; } = -0;
    }
}