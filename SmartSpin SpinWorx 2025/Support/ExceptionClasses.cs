using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSpin.Support
{
    public class SpinningException : Exception
    {
        public SpinningException() { }
        public SpinningException(string message) : base(message) { }
        public SpinningException(string message, Exception inner) : base(message, inner) { }
    }
}