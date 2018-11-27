using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SignalRDemo.WebServer.Entities;

namespace SignalRDemo.WebServer.Services
{
    public class ActiveUserRepository : IActiveUserRepository
    {
        private readonly SRDemoContext _context;
        private readonly ILogger<ActiveUserRepository> _logger;

        public ActiveUserRepository(SRDemoContext context, ILogger<ActiveUserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void AddActiveUser(ActiveUser activeUser)
        {
            try
            {
                var currentUser = _context.ActiveUsers.FirstOrDefault(x => x.NTLogin == activeUser.NTLogin);

                if(currentUser != null)
                {
                    currentUser.IsActive = true;
                    currentUser.LastConnected = DateTime.Now;
                }
                else
                {
                    activeUser.IsActive = true;
                    activeUser.LastConnected = DateTime.Now;
                    _context.ActiveUsers.Add(activeUser);
                }

                
            }
            catch (Exception e)
            {
                _logger.LogError($"Error adding active user");
                _logger.LogDebug($"Error adding active user: {activeUser.NTLogin}. Error Message: {e.Message}");
                throw;
            }
        }

        public IEnumerable<ActiveUser> GetActiveUsers()
        {
            try
            {
                var ActiveUsers = _context.ActiveUsers.ToList();

                return ActiveUsers;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error getting active users");
                throw;
            }
        }

        public void RemoveActiveUser(string ntlogin)
        {
            try
            {
                var activeUser = _context.ActiveUsers.FirstOrDefault(x => x.NTLogin == ntlogin);

                if (activeUser != null)
                {
                    //_context.ActiveUsers.Remove(activeUser);
                    activeUser.LastDisconnected = DateTime.Now;
                    activeUser.IsActive = false;
                }   
            }
            catch (Exception e)
            {
                _logger.LogError($"Error removing active user");
                _logger.LogDebug($"Error Message: {e.Message}");
                throw;
            }
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
