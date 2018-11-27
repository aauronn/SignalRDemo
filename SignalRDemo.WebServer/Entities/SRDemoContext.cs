using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRDemo.WebServer.Entities
{
    public class SRDemoContext : IdentityDbContext<Principal>
    {
        public SRDemoContext(DbContextOptions<SRDemoContext> options)
            : base(options)
        {
            //Database.EnsureCreated();
            Database.Migrate();

        }

        public DbSet<Principal> Principals { get; set; }
        public DbSet<ActiveUser> ActiveUsers { get; set; }
    }
}
