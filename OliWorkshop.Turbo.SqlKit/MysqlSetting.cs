using OliWorkshop.Turbo.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace OliWorkshop.Turbo.SqlKit
{
    public class MysqlSetting : DbSetupParameters
    {
        public bool UseMariaDb;

         public new object Extra => UseMariaDb;
    }
}
