using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRDemo.WebServer.Models
{
    public class ActiveUserDto
    {
        public string ConnectionID { get; set; }

        [Required]
        public string NTLogin { get; set; }

        [Required]
        public string UserIp { get; set; }

        [Required]
        public string UserIPAddress { get; set; }

        [Required]
        public string UserMachineName { get; set; }
    }
}
