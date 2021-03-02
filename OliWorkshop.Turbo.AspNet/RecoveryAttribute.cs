using System;
using System.Collections.Generic;
using System.Text;

namespace OliWorkshop.Turbo.AspNet
{
    public class FromRecordAttribute : Attribute
    {
        public FromRecordAttribute(string field = null)
        {
            Field = field;
        }

        public string Field { get; }
    }
}
