namespace CarpetBG.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<Address> Addresses { get; set; } = [];
        public List<Order> Orders { get; set; } = [];
    }
}
