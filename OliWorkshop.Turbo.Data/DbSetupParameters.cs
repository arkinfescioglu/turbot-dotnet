using System;
using System.Collections.Generic;
using System.Text;

namespace OliWorkshop.Turbo.Data
{
    public class DbSetupParameters
    {
        public string TypeProvider;
        public string Connection;
        public int? Timeout;
        public int? BacthMax;
        public int? ChartSet;
        public bool ShowError;
        public string DbVersion;
        public object Extra;
    }
}
