namespace DragonAsia.SalesMan.Models
{
    public class Customer
    {
        #region properties
		public string Guid { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        #endregion

        public string Group { get { return string.IsNullOrWhiteSpace(Name) ? "-" : Name.Substring(0, 1).ToUpper(); } }
        public bool ShouldSerializeGroup() { return false; }
    }
}   
