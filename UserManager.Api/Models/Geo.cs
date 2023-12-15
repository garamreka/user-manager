using MongoDB.Bson.Serialization.Attributes;

namespace UserManager.Api.Models
{
    public class Geo
    {
        [BsonElement("lat")]
        public string Lat { get; set; }

        [BsonElement("lng")]
        public string Lng { get; set; }
    }
}
