using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    public class Report
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string Details { get; set; }

        public string ReporterId { get; set; }

        public User Reporter { get; set; }

        public string ReportedUserId { get; set; }

        public User ReportedUser { get; set; }

        public DateTime DateOfCreation { get; set; } = DateTime.Now;
    }
}
