using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRDemo.WebServer.Entities
{
    public class SRDemoSeeder
    {
        private readonly SRDemoContext _context;
        private readonly IHostingEnvironment _hosting;
        private readonly UserManager<Principal> _userManager;

        public SRDemoSeeder(SRDemoContext context, IHostingEnvironment hosting, UserManager<Principal> userManager)
        {
            _context = context;
            _hosting = hosting;
            _userManager = userManager;
        }

        public async Task Seed()
        {
            _context.Database.EnsureCreated();

            var Principal = await _userManager.FindByEmailAsync("luis@mail.com");

            if (Principal == null)
            {
                Principal = new Principal
                {
                    FirstName = "Luis",
                    LastName = "Gutierrez",
                    UserName = "lgutierrez",
                    Email = "luis@mail.com"
                };

                var result = await _userManager.CreateAsync(Principal, "P@ssw0rd!");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Failed to create default user");
                }

            }

            ///////


            ///////

            var Principal3 = await _userManager.FindByEmailAsync("dgeorge@mail.com");

            if (Principal3 == null)
            {
                Principal3 = new Principal
                {
                    FirstName = "Durante",
                    LastName = "George",
                    UserName = "dgeorge",
                    Email = "csuser_test@mail.com"
                };

                var result3 = await _userManager.CreateAsync(Principal3, "P@ssw0rd!");

                if (result3 != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Failed to create default user");
                }

            }
        }
    }
}
