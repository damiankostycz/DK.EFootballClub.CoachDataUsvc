using DK.EFootballClub.CoachDataUsvc.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DK.EFootballClub.CoachDataUsvc;

public class MongoDbService
{
    private readonly IMongoCollection<Coach> _coaches;

    public MongoDbService(string? connectionString, string? databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _coaches = database.GetCollection<Coach>("Coaches");
    }

    public async Task<List<Coach>> GetAllCoachesAsync()
    {
        return await _coaches.Find(_ => true).ToListAsync();
    }

    private async Task<Coach?> GetCoachByIdAsync(string id)
    {
        var filter = Builders<Coach>.Filter.Eq("_id", ObjectId.Parse(id));
        return await _coaches.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Coach?> CreateCoachAsync(Coach coach)
    {
        await _coaches.InsertOneAsync(coach);
        return coach;
    }

    public async Task<Coach?> UpdateCoachAsync(string id, Coach updatedCoach)
    {
        var filter = Builders<Coach>.Filter.Eq("_id", ObjectId.Parse(id));
        var result = await _coaches.ReplaceOneAsync(filter, updatedCoach);

        if (result.ModifiedCount > 0)
        {
            return await GetCoachByIdAsync(id);
        }

        return null;
    }

    public async Task<bool> DeleteCoachAsync(string id)
    {
        var filter = Builders<Coach>.Filter.Eq("_id", ObjectId.Parse(id));
        var result = await _coaches.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }
}