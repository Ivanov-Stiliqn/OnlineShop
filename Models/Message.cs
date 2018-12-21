using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    public class Message
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public string ReciverId { get; set; }

        public User Receiver { get; set; }

        [Required]
        public string Text { get; set; }

        public bool IsRead { get; set; }

        public DateTime DateOfCreation { get; set; } = DateTime.Now;
    }
}
