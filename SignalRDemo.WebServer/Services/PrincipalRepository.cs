using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SignalRDemo.WebServer.Entities;

namespace SignalRDemo.WebServer.Services
{
    public class PrincipalRepository : IPrincipalRepository
    {
        private readonly SRDemoContext _context;
        private readonly ILogger<PrincipalRepository> _logger;

        public PrincipalRepository(SRDemoContext context, ILogger<PrincipalRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public Principal GetPrincipalByNTLogin(string ntLogin)
        {
            try
            {
                var model = _context.Principals.FirstOrDefault(x => x.UserName == ntLogin);

                return model;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error getting Principal");
                throw;
            }
        }
    }
}
