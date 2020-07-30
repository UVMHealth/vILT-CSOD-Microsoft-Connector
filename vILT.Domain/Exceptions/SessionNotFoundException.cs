using System;

namespace vILT.Domain
{
    public class SessionNotFoundException : Exception
    {
        public SessionNotFoundException()
        {
        }

        public SessionNotFoundException(string message)
            : base(message)
        {
        }

        public SessionNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
