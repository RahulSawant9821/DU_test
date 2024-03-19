using DU_test.Data;
using DU_test.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;


namespace DU_test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DUController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<JobOffer>> GetOffers()
        {
            return Ok(OffersStore.Offers);
        }

        [HttpGet("Offer/{JobId}", Name = "GetOffer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<JobOffer> GetOffer(string JobId)
        {
            if (string.IsNullOrEmpty(JobId))
            {
                return BadRequest();
            }
            var offer = OffersStore.Offers.FirstOrDefault(u => u.JobOfferId == JobId);
            if (offer == null)
            {
                return NotFound();
            }

            return Ok(offer);
        }

        [HttpPost("authenticateAndRequestSimilarOffers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<JobOffer>> AuthenticateAndRequestSimilarOffers([FromBody] Driver driver)
        {
            if (driver == null)
            {
                return BadRequest(driver);
            }

            if (driver.username == null || driver.password == null || driver.jobOffers == null)
            {
                return BadRequest(driver);
            }

            var authenticatedDriver = DriverStore.DriverDetail.FirstOrDefault(d => d.username == driver.username && d.password == driver.password);
            if (authenticatedDriver == null)
            {
                return StatusCode(401);
            }

            var selectedOffer = OffersStore.Offers.FirstOrDefault(o => o.JobOfferId == driver.jobOffers.JobOfferId && o.JobStatus == "Open");

            if (selectedOffer == null)
            {
                return StatusCode(500);
            }
            var similarOffers = OffersStore.Offers.Where(o => o.JobId == selectedOffer.JobId && o.JobOfferId != selectedOffer.JobOfferId && o.JobStatus == "Open").ToList();

            similarOffers.Insert(0, selectedOffer);

            return Ok(similarOffers);
        }


        [HttpGet("SubmitSelected")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public  ActionResult<JobOffer> SubmitSelected(string JobOfferId)
        {
            if (JobOfferId == null)
            {
                return BadRequest(JobOfferId);
            }

            var Job = OffersStore.Offers.FirstOrDefault(u => u.JobOfferId == JobOfferId);
            if (Job == null)
            {
                return NotFound();
            }
            Job.JobStatus = "In-Process";
           // var result = await requestRouteAndEstimates(Job.Pickup.City,Job.Dropoff.City);
            return Ok(Job);
        }


       
        private readonly IHttpClientFactory _httpClientFactory;

        public DUController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        [HttpPost("requestRouteAndEstimates")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GeocodingResponse>> requestRouteAndEstimates([FromBody] ServiceRoute serviceRoute)
        {
            if (serviceRoute == null || string.IsNullOrEmpty(serviceRoute.StartLocation) || string.IsNullOrEmpty(serviceRoute.EndLocation) || string.IsNullOrEmpty(serviceRoute.transportMeans ))
            {
                return BadRequest("Missing required parameters.");
            }

            if(serviceRoute.StartLocation.ToLower() == serviceRoute.EndLocation.ToLower())
            {
                return BadRequest("Pickup and Drop are the same location");
            }
        

            try
            {

                var accessToken = "Your access Token over here";

                var startUrl = $"https://api.mapbox.com/geocoding/v5/mapbox.places/{serviceRoute.StartLocation}.json?access_token={accessToken}";
                var targetUrl = $"https://api.mapbox.com/geocoding/v5/mapbox.places/{serviceRoute.EndLocation}.json?access_token={accessToken}";

                var startResponse = await MakeGeocodingRequest(startUrl);
                if (!startResponse.IsSuccessStatusCode)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Error during geocoding start location.");
                }

                var startContent = await startResponse.Content.ReadAsStringAsync();
                var startCoordinates = ExtractCoordinates(startContent);

                var targetResponse = await MakeGeocodingRequest(targetUrl);
                if (!targetResponse.IsSuccessStatusCode)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Error during geocoding end location.");
                }

                var targetContent = await targetResponse.Content.ReadAsStringAsync();
                var targetCoordinates = ExtractCoordinates(targetContent);
                var transport = "driving";

                if (!string.IsNullOrEmpty(serviceRoute.transportMeans))
                {
                    transport = serviceRoute.transportMeans.ToLower() switch
                    {
                        "car" => "driving",
                        "cycle" => "cycling",
                        "walking" => "walking",
                        _ => throw new ArgumentException("Invalid transport means."), 
                    };
                }
                else
                {
                    return BadRequest("Transport means is required.");
                }

                var url = $"https://api.mapbox.com/directions/v5/mapbox/{transport}/{startCoordinates.Item2},{startCoordinates.Item1};{targetCoordinates.Item2},{targetCoordinates.Item1}?access_token={accessToken}&geometries=geojson";

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var directionsResponse = JsonConvert.DeserializeObject<JObject>(content);

                        var totalDistance = directionsResponse["routes"]?.First?["distance"]?.Value<double>() ?? 0;
                        var totalTime = directionsResponse["routes"]?.First?["duration"]?.Value<double>() ?? 0;
                       

                        var routeInfo = new
                        {
                            Distance = totalDistance,
                            Time = totalTime,
                            Route = content
                        };

                        return Ok(routeInfo);
                        
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, "Error fetching route details.");
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during geocoding request: {ex.Message}");
                return StatusCode(StatusCodes.Status400BadRequest, "An error occurred while processing your request.");
            }
        }
        private (double, double) ExtractCoordinates(string json)
        {
            var jsonObject = JObject.Parse(json);
            var features = jsonObject["features"];
            if (features != null && features.HasValues)
            {
                var firstFeature = features.First;
                var geometry = firstFeature["geometry"];
                if (geometry != null)
                {
                    var coordinates = geometry["coordinates"];
                    if (coordinates != null && coordinates.HasValues)
                    {
                        var longitude = (double)coordinates[0];
                        var latitude = (double)coordinates[1];
                        return (latitude, longitude);
                    }
                }
            }
            return (0, 0);
        }
        private async Task<HttpResponseMessage> MakeGeocodingRequest(string url)
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                return await client.GetAsync(url);
            }
        }
    }
}
