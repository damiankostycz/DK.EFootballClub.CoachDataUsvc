using System.Net;
using System.Text.Json;
using DK.EFootballClub.CoachDataUsvc.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace DK.EFootballClub.CoachDataUsvc;

public class CoachDataHttpTrigger(ILoggerFactory loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<CoachDataHttpTrigger>();
    private readonly string? _dbConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
    private readonly string? _dbName = Environment.GetEnvironmentVariable("DATABASE_NAME");

   [Function("GetAllCoaches")]
    public async Task<HttpResponseData> GetAllCoaches(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "coaches")] HttpRequestData req)
    {
        var response = req.CreateResponse();
        try
        {
            var db = new MongoDbService(_dbConnectionString, _dbName);
            var coaches  = await db.GetAllCoachesAsync();
            await response.WriteAsJsonAsync(coaches);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching coaches");
            response.StatusCode = HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("Internal server error");
            return response;
        }
    }

    [Function("CreateCoach")]
    public async Task<HttpResponseData> CreateCoach(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "coaches")] HttpRequestData req)
    {
        var response = req.CreateResponse();

        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var coach = JsonSerializer.Deserialize<Coach>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (coach == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid coach data.");
                return response;
            }

            var db = new MongoDbService(_dbConnectionString, _dbName);
            Coach? createdCoach = await db.CreateCoachAsync(coach);

            response.StatusCode = HttpStatusCode.Created;
            response.Headers.Add("Location", $"/api/Coach/{createdCoach!.CoachId}");
            await response.WriteAsJsonAsync(createdCoach);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating coach");
            response.StatusCode = HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("Internal server error");
            return response;
        }
    }

    [Function("UpdateCoach")]
    public async Task<HttpResponseData> UpdateCoachr(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "coaches/{id}")] HttpRequestData req,
        string id)
    {
        var response = req.CreateResponse();

        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedCoachData = JsonSerializer.Deserialize<Coach>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (updatedCoachData == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteStringAsync("Invalid coach data.");
                return response;
            }

            var db = new MongoDbService(_dbConnectionString, _dbName);
            var updatedCoach = await db.UpdateCoachAsync(id, updatedCoachData);

            if (updatedCoach == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                await response.WriteStringAsync($"Coach with ID {id} not found.");
                return response;
            }

            response.StatusCode = HttpStatusCode.OK;
            await response.WriteAsJsonAsync(updatedCoach);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating coach with ID {id}");
            response.StatusCode = HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("Internal server error");
            return response;
        }
    }

    [Function("DeleteCoach")]
    public async Task<HttpResponseData> DeleteCoach(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "coaches/{id}")] HttpRequestData req,
        string id)
    {
        var response = req.CreateResponse();

        try
        {
            var db = new MongoDbService(_dbConnectionString, _dbName);
            var success = await db.DeleteCoachAsync(id);

            if (!success)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                await response.WriteStringAsync($"Coach with ID {id} not found.");
                return response;
            }

            response.StatusCode = HttpStatusCode.NoContent;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting coach with ID {id}");
            response.StatusCode = HttpStatusCode.InternalServerError;
            await response.WriteStringAsync("Internal server error");
            return response;
        }
    }
    
}