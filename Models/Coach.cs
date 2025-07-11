using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DK.EFootballClub.CoachDataUsvc.Models;

public class Coach
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string CoachId { get; set; } = string.Empty;

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("last_name")]
    public required string LastName { get; set; }

    [BsonElement("date_of_birth")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime DateOfBirth { get; set; }

    [BsonElement("begining_date")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime BeginningDate { get; set; }

    [BsonElement("specialization")]
    public string? Specialization { get; set; }

    [BsonElement("country")]
    public string? Country { get; set; }

    [BsonElement("phone_number")]
    public required string PhoneNumber { get; set; }

    [BsonElement("languages")]
    public string? Languages { get; set; }

    [BsonElement("sex")]
    public string? Sex { get; set; }

    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? TeamID { get; set; }
}
