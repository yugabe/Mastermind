using System;

namespace Mastermind.Api
{
    [Serializable]
    public class UserException : Exception
    {
        public UserException() { }
        public UserException(string message) : base(message) { }
        public UserException(string message, Exception inner) : base(message, inner) { }
        protected UserException(System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
