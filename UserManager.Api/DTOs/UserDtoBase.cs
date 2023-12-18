using System.ComponentModel.DataAnnotations;

namespace UserManager.Api.DTOs
{
    public abstract class UserDtoBase
    {
        public string? Name { get; set; }

        public string? UserName { get; set; }

        public AddressDto? Address { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public string? Website { get; set; }

        public CompanyDto? Company { get; set; }
    }
}
