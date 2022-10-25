using AirportFlightInformation.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace AirportFlightInformation.Controllers
{
    [Route("api/[Controller]/[Action]")]
    [ApiController]
    public class AirportFlightInformationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly Dictionary<string, int> Airports = new Dictionary<string, int>()
        {
            { "Mashhad - HashemiNezhad", 102 },
            { "Tehran - Mehrabad", 2 },
            { "Shiraz", 1 },
            { "Isfahan", 114 },
            { "Tabriz", 103 },
            { "Ahvaz", 401 },
            { "Boshehr", 104 },
            { "Kerman", 201 },
            { "Sari", 106 },
            { "Yazd", 107 },
            { "kermanShah", 111 },
            { "Rasht", 203 },
            { "Zahedan", 109 },
            { "Abadan", 301 },
            { "Bandarabbas", 117 },
            { "Gorgan", 202 },
            { "Hamedan", 112 },
            { "Ardabil", 113 },
            { "Ilam", 105 },
            { "Oromie", 110 },
            { "Birjand", 204 },
            { "Sanandaj", 402 },
            { "Shahrkord", 108 },
            { "Bojnourd", 901 },
            { "Larestan", 601 },
            { "KhoramAbad", 701 },
            { "Parsabad Moghan", 702 },
            { "Semnan", 801 },
            { "Shahroud", 802 },
            { "Noshahr", 1201 },
            { "Yasouj", 1001 },
            { "Zanjan", 501 },

        };

        public AirportFlightInformationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [ActionName("GetAirportsList")]
        public IActionResult Get()
        {
            return Ok(Airports);
        }

        [HttpGet]
        [ActionName("GetAirpotInformation")]
        public IActionResult Get(int AirportID)
        {
            if (AirportID <= 0 || !Airports.Where(w => w.Value == AirportID).Any())
            {
                return NotFound(new { Error = 404, Message = "کد فرودگاه وارد شده معتبر نیست" });
            }

            try
            {
                HtmlWeb scraper = new HtmlWeb();

                var Url = $"{_configuration["FidsData:Url"].ToString()}/{AirportID}/";

                var webpage = scraper.Load(Url);

                var rows = webpage.DocumentNode.SelectNodes(
                    "/html/body/div[2]/div/div/div/div/div/div[1]/div/div/div/div/div[2]/div[3]/div[2]/table/tbody/tr");

                if (rows != null)
                {
                    List<AirportItem> airportItems = new();
                    int rownumber = 1;
                    foreach (var row in rows)
                    {
                        AirportItem airportItem = new()
                        {
                            DateTime = row
                                .SelectSingleNode(
                                    $"{_configuration["FidsData:SampleString"]}/tr[{rownumber}]/td[{1}]/p").InnerText,
                            Airline = row
                                .SelectSingleNode(
                                    $"{_configuration["FidsData:SampleString"]}/tr[{rownumber}]/td[{2}]/p").InnerText,
                            FlightNumber =
                                row.SelectSingleNode(
                                    $"{_configuration["FidsData:SampleString"]}/tr[{rownumber}]/td[{3}]/p").InnerText,
                            Origin = row
                                .SelectSingleNode(
                                    $"{_configuration["FidsData:SampleString"]}/tr[{rownumber}]/td[{4}]/p").InnerText,
                            FlightStatus =
                                row.SelectSingleNode(
                                    $"{_configuration["FidsData:SampleString"]}/tr[{rownumber}]/td[{5}]/p").InnerText,
                            RealTime = row
                                .SelectSingleNode(
                                    $"{_configuration["FidsData:SampleString"]}/tr[{rownumber}]/td[{7}]/p").InnerText,
                            AirplaneType =
                                row.SelectSingleNode(
                                    $"{_configuration["FidsData:SampleString"]}/tr[{rownumber}]/td[{8}]/p").InnerText,
                        };

                        airportItems.Add(airportItem);

                        rownumber++;
                    }

                    AirportResult airportResult = new()
                    {
                        Status = 200,
                        Message = "لیست پرواز با موفقیت استخراج گردید :)",
                        Count = airportItems.Count,
                        AirportName = Airports.Where(w => w.Value == AirportID).SingleOrDefault().Key,
                        airportItems = airportItems
                    };

                    return Ok(new { AirportResults = airportResult });
                }
                else
                {
                    return NotFound(new AirportResult()
                    { Status = 404, Message = "E1 - عملیات استخراج داده با خطا مواجه گردید" });
                }
            }
            catch (Exception)
            {
                return NotFound(new AirportResult()
                { Status = 404, Message = "E2 - عملیات استخراج داده با خطا مواجه گردید" });
            }
        }
    }
}