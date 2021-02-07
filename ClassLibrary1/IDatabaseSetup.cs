using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace OliWorkshop.Turbo.Data
{
    public interface IDatabaseSetup
    {
        public void DbSetup(DbSetupParameters parameters, DbContextOptionsBuilder options);
    }
}
