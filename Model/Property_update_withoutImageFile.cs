namespace WebApplication1.Model
{
    public class Property_update_withoutImageFile
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public decimal RentAmount { get; set; }
        public int Bedrooms { get; set; }
        public string IsFurnished { get; set; }
        public string PropertyType { get; set; }
        public double AreaInSqFt { get; set; }
        public DateTime AvailableFrom { get; set; }
        public string OwnerName { get; set; }
        public string ContactNumber { get; set; }
        public string listingType { get; set; }
        public string ImageFile { get; set; }
    }
}
