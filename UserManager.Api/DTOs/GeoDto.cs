using System.ComponentModel.DataAnnotations;

namespace UserManager.Api.DTOs
{
    public class GeoDto
    {
        private const string _coordinateRegexPattern = @"^[-+]?([1-8]?\d(\.\d+)?|90(\.0+)?),\s*[-+]?(180(\.0+)?|((1[0-7]\d)|([1-9]?\d))(\.\d+)?)$";

        [RegularExpression(_coordinateRegexPattern)]
        public string? Lat { get; set; }

        [RegularExpression(_coordinateRegexPattern)]
        public string? Lng { get; set; }
    }
}
