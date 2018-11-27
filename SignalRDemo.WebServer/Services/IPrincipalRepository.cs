using SignalRDemo.WebServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRDemo.WebServer.Services
{
    public interface IPrincipalRepository
    {
        Principal GetPrincipalByNTLogin(string ntLogin);
    }
}
