using MongoDB.Bson.Serialization.Attributes;

namespace UserManager.Api.Models
{
    public class Company
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("catchPhrase")]
        public string CatchPhrase { get; set; }

        [BsonElement("bs")]
        public string Bs { get; set; }
    }
}
