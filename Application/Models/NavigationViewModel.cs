using System.Collections.Generic;

namespace Application.Models
{
    public class NavigationViewModel
    {
        public ICollection<MenuItemViewModel> Categories { get; set; }

        public bool UnSeenSellOrders { get; set; }

        public bool UnSeenPurchaseOrders { get; set; }
    }
}
