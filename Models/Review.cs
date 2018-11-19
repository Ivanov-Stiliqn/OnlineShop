using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    public class Review
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Comment { get; set; }

        public int Stars { get; set; }

        public DateTime DateOfCreation { get; set; } = DateTime.Now;

        public string UserId { get; set; }

        public User User { get; set; }

        public Guid ProductId { get; set; }

        public Product Product { get; set; }
    }
}
