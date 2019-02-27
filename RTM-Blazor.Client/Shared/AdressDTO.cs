namespace RTM_Blazor.Client.Shared {
    public class AddressDTO {
        public int AddressId { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public int[] JobHistory { get; set; }
    }
}