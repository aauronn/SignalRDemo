using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRDemo.Entities
{
    public class Principal
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
