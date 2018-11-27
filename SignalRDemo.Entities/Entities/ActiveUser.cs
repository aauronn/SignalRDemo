using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;

namespace SignalRDemo.Entities
{
    [Table("ActiveUsers")]
    public class ActiveUser
    {
        [Key]
        [Column(Order = 0)]
        public string NTLogin { get; set; }

        [Column(Order = 1)]
        public string ConnectionID { get; set; }

        [Required]
        [Column(Order = 2)]
        public string UserIPAddress { get; set; }

        [Required]
        [Column(Order = 3)]
        public string UserMachineName { get; set; }

        [DefaultValue(0)]
        [Column(Order = 4)]
        public bool IsActive { get; set; }

        [Required]
        [Column(Order = 5)]
        public DateTime LastConnected { get; set; }

        [Column(Order = 6)]
        public DateTime? LastDisconnected { get; set; }
    }
}
