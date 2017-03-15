namespace DragonAsia.SalesMan.Models
{
    public class Item
    {
		#region properties

		public string Guid { get; set; }
		public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

		#endregion
	}
}