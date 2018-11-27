using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRDemo.WebServer.Entities
{
    public class Principal : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
