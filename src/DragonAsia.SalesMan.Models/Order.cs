using System;
using System.Collections.Generic;
using System.Linq;

namespace DragonAsia.SalesMan.Models
{
    public class Order
    {
		#region properties

		public string Guid { get; set; }
		public double DeliveryFee { get; set; }
        public bool TaxRefIncluded { get; set; }
        public DateTime Timestamp { get; set; }

        public Customer Customer { get; set; }
        public List<Item> Items { get; set; }
        
        #endregion

        public double TotalPrice { get { return DeliveryFee + Items.Sum(i => i.Price * i.Quantity); } }
        public DateTime Group { get { return new DateTime(Timestamp.Year, Timestamp.Month, Timestamp.Day); } }
        public bool ShouldSerializeTotalPrice() { return false; }
        public bool ShouldSerializeGroup() { return false; }
    }
}
