using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Models
{
    // Add profile data for application users by adding properties to the User class
    public class User : IdentityUser
    {
        public User()
        {
            this.MyProducts = new List<Product>();
            this.Reports = new List<Report>();
            this.ReportsGiven = new List<Report>();
            this.Reviews = new List<Review>();
            this.PurchaseOrders = new List<Order>();
            this.SellOrders = new List<Order>();
            this.MessagesSent = new List<Message>();
            this.MessagesReceived = new List<Message>();
        }

        public ICollection<Product> MyProducts { get; set; }

        public bool IsRestricted { get; set; }

        public string Whishlist { get; set; }

        public UserInfo UserInfo { get; set; }

        public ICollection<Report> Reports { get; set; }

        public ICollection<Report> ReportsGiven { get; set; }

        public ICollection<Review> Reviews { get; set; }

        public ICollection<Order> PurchaseOrders { get; set; }

        public ICollection<Order> SellOrders { get; set; }

        public ICollection<Message> MessagesSent { get; set; }

        public ICollection<Message> MessagesReceived { get; set; }
    }
}
