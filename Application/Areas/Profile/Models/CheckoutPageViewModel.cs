using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Areas.Profile.Models
{
    public class CheckoutPageViewModel
    {
        public ICollection<CartViewModel> Products { get; set; }

        public UserInfoViewModel UserInfo { get; set; }

        public string UserInfoId { get; set; }

        public bool IsTermsAccepted { get; set; }
    }
}
