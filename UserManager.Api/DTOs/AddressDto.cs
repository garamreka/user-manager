namespace UserManager.Api.DTOs
{
    public class AddressDto
    {
        public string? Street { get; set; }

        public string? Suite { get; set; }

        public string? City { get; set; }

        public string? ZipCode { get; set; }

        public GeoDto? Geo { get; set; }
    }
}
