namespace AirportFlightInformation.Models
{
    public class AirportResult
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string AirportName { get; set; }
        public int Count { get; set; }
        public List<AirportItem> airportItems { get; set; }
    }
    public class AirportItem
    {
        public string DateTime { get; set; }
        public string Airline { get; set; }
        public string FlightNumber { get; set; }
        public string Origin { get; set; }
        public string FlightStatus { get; set; }
        public string RealTime { get; set; }
        public string AirplaneType { get; set; }
    }
}
