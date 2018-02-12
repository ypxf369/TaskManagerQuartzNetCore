using System;
using System.Collections.Generic;
using System.Text;
using QuartzNetCore.EFModel_SqlServer.Models;

namespace QuartzNetCore.Repository
{
    internal class DbContextFactory
    {
        public static QuartzManagerContext DbContext;

        static DbContextFactory()
        {
            DbContext = new QuartzManagerContext();
        }
    }
}
