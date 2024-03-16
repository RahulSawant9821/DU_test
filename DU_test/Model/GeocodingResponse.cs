namespace DU_test.Model
{
    public class GeocodingResponse
    {
        public LocationCoordinates StartLocation { get; set; }
        public LocationCoordinates EndLocation { get; set; }
    }

    public class LocationCoordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
