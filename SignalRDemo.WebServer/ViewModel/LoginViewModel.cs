using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRDemo.WebServer.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string UserIPAddress { get; set; }
        [Required]
        public string UserMachineName { get; set; }
        public bool RememberMe { get; set; }

    }
}
