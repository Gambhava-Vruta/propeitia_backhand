using Microsoft.AspNetCore.Mvc;
using Propertia.Models;
using System.Net;
using System.Text.Json;

[ApiController]
[Route("api/location")]
public class LocationController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly HttpClient _http;

    public LocationController(IConfiguration config)
    {
        _config = config;
        _http = new HttpClient();
    }

    [HttpPost("nearby")]
    public async Task<IActionResult> GetNearby([FromBody] LocationRequest req)
    {
        var rawAddress = $"{req.Area}, {req.City}, {req.State}, {req.Country}";
        var address = WebUtility.UrlEncode(rawAddress);
        var apiKey = _config["GoogleApiKey"];

        // 1️⃣ Geocoding
        var geoUrl =
     $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key={apiKey}";

        var geoJson = await _http.GetStringAsync(geoUrl);
        using var geoDoc = JsonDocument.Parse(geoJson);

        var results = geoDoc.RootElement.GetProperty("results");

        if (results.GetArrayLength() == 0)
        {
            return BadRequest(new
            {
                message = "Location not found by Google Geocoding API",
                address = rawAddress
            });
        }

        var location = results[0]
            .GetProperty("geometry")
            .GetProperty("location");

        var lat = location.GetProperty("lat").GetDouble();
        var lng = location.GetProperty("lng").GetDouble();


        string[] types = {
            "hospital",
            "park",
            "police",
            "fire_station",
            "train_station",
            "subway_station",
            "stationery_store"
        };

        var result = new Dictionary<string, object>();

        foreach (var type in types)
        {
            var url =
                $"https://maps.googleapis.com/maps/api/place/nearbysearch/json" +
                $"?location={lat},{lng}&radius=3000&type={type}&key={apiKey}";

            var json = await _http.GetStringAsync(url);
            using var doc = JsonDocument.Parse(json);

            var places = doc.RootElement
                .GetProperty("results")
                .EnumerateArray()
                .Select(p => new
                {
                    name = p.GetProperty("name").GetString(),
                    rating = p.TryGetProperty("rating", out var r) ? r.GetDouble() : (double?)null,
                    address = p.GetProperty("vicinity").GetString()
                });

            result[type] = places;
        }

        return Ok(new { nearby = result });
    }
}
