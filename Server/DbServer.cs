using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class DbServer
    {
        private readonly DbContextOptions<ServerDbContext> options;

        public DbServer()
        {
            options = ServerDbContext.GenerateOptions();
            CreatingTest();
        }

        public void CreatingTest()
        {
            var db = new ServerDbContext();
        }

        //...
    }
}
