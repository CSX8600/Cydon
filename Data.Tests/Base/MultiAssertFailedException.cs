using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Tests.Base
{
    public class MultiAssertFailedException : Exception
    {
        private List<Exception> exceptions;
        public MultiAssertFailedException(List<Exception> exceptions)
        {
            this.exceptions = exceptions;
        }

        public override string Message
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                foreach(Exception exception in exceptions)
                {
                    builder.AppendLine(exception.Message);
                    builder.AppendLine();
                }

                return builder.ToString();
            }
        }
    }
}
