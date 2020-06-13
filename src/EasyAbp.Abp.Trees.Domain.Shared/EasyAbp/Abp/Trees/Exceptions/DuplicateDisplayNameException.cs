using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;

namespace EasyAbp.Abp.Trees.Exceptions
{
    public class DuplicateDisplayNameException : BusinessException
    {
        public DuplicateDisplayNameException(Exception exception = null)
            : base(TreesErrorCodes.DuplicateDisplayName, "DuplicateDisplayNameWarning", innerException: exception)
        {

        }

    }
}
