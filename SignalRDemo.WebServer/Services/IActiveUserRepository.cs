using SignalRDemo.WebServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRDemo.WebServer.Services
{
    public interface IActiveUserRepository
    {
        IEnumerable<ActiveUser> GetActiveUsers();

        void AddActiveUser(ActiveUser activeUser);

        void RemoveActiveUser(string ntlogin);

        bool Save();
    }
}
