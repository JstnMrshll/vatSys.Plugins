using RestSharp;
using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using vatsys;
using vatsys.Plugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VatSys.Plugins
{
    [Export(typeof(IPlugin))]
    public class FdrEnhancer : IPlugin
    {
        const string stdMsgPrefix = ".";
        //const string airportMsgPrefix = "*";

        const string RAPIDAPI_KEY = "";

        ConcurrentDictionary<string, string> airlineCallsigns = new ConcurrentDictionary<string, string>();
        ConcurrentDictionary<string, string> airportNames = new ConcurrentDictionary<string, string>();

        /// Plugin Name
        public string Name { get => "FDR Enhancer"; }

        public void OnFDRUpdate(FDP2.FDR updated)
        {
            if (updated.LocalOpData == "" || updated.LocalOpData.StartsWith("."))
            {
                string[] routeTextSections = updated.RouteNoParse.Split(' ');

                string waypoint = "";
                string sep = "";
                string airportName = LookupAirportName(updated.DesAirport);
                //airportName = vatsys.Airspace2.GetAirport(updated.DesAirport).FullName;

                if (routeTextSections.Length > 1)
                {
                    waypoint = routeTextSections[1];
                }

                string airlineCallsign = LookupAirlineCallsign(updated.Callsign);

                if (updated.State == FDP2.FDR.FDRStates.STATE_PREACTIVE && updated.RunwayString == "" && airlineCallsign != "")
                {
                    updated.LocalOpData = stdMsgPrefix + airlineCallsign;
                }
                else if (updated.State == FDP2.FDR.FDRStates.STATE_PREACTIVE && updated.RunwayString == "" && airlineCallsign == "")
                {
                    updated.LocalOpData = stdMsgPrefix + airportName;
                }
                else if (updated.State == FDP2.FDR.FDRStates.STATE_PREACTIVE && updated.RunwayString != "")
                {
                    if (airportName.Length + waypoint.Length > 12) airportName = airportName.Substring(0, (13 - waypoint.Length));
                    if (waypoint != "") sep = "*";

                    updated.LocalOpData = stdMsgPrefix + airportName + sep + waypoint;

                    //if (updated.IsTrackedByMe && !updated.OtherInfoString.Contains("/FDRENH/"))
                    //updated.Remarks += $"/FDRENH/ AIRPORT/{airportName} AIRLINE/{airlineCallsign}";

                }
            }

            // Modify Global tags only if it is managed by "Me"
            if (updated.IsTrackedByMe)
            {
                if (updated.TextOnly && !updated.GlobalOpData.Contains("^TXT"))
                {
                    updated.GlobalOpData = "^TXT";
                }
                else if (updated.ReceiveOnly && !updated.GlobalOpData.Contains("^RX"))
                {
                    updated.GlobalOpData = "^RX";
                }

                Coordinate source = vatsys.Airspace2.GetAirport(updated.DepAirport).LatLong;
                Coordinate destination = vatsys.Airspace2.GetAirport(updated.DesAirport).LatLong;

                double bearingBetweenAirports = CalculateBearingFromRads(source.LatitudeRads, source.LongitudeRads, destination.LatitudeRads, destination.LongitudeRads);

                bool needsEven = (bearingBetweenAirports >= 180 && bearingBetweenAirports < 360);
                bool evenRFL = (updated.RFL % 20 == 0);
                bool evenCFL = (updated.CFLUpper % 20 == 0);

                //if (updated.LocalOpData == "")
                //    updated.LocalOpData += bearingBetweenAirports.ToString("##0");

                if (updated.RFL > 0 && needsEven && !evenRFL && !updated.GlobalOpData.Contains(@"@RFL"))
                    updated.GlobalOpData += @"@RFL";
                if (updated.CFLUpper > 0 && needsEven && !evenCFL && !updated.GlobalOpData.Contains(@"@CFL"))
                    updated.GlobalOpData += @"@CFL";
            }
        }

        public void OnRadarTrackUpdate(RDP.RadarTrack updated)
        {
        }

        public CustomLabelItem GetCustomLabelItem(string itemType, Track track, FDP2.FDR flightDataRecord, RDP.RadarTrack radarTrack)
        {
            return null;
        }

        //Here we can set a custom colour for the track and label. Otherwise return null.
        public CustomColour SelectASDTrackColour(Track track)
        {
            return null;
        }

        public CustomColour SelectGroundTrackColour(Track track)
        {
            return null;
        }

        private string LookupAirlineCallsign(string callsign)
        {
            string callsignTextOnly = "";
            string airlineCallsign = "";

            int numPos = callsign.IndexOfAny("1234567890".ToCharArray());

            if (numPos >= 2)
            {
                callsignTextOnly = callsign.Substring(0, numPos);

                if (airlineCallsigns.ContainsKey(callsignTextOnly))
                {
                    airlineCallsign = airlineCallsigns.GetOrAdd(callsignTextOnly, airlineCallsign);
                }
                else
                {
                    var client = new RestClient("https://aviation-reference-data.p.rapidapi.com/airline/" + callsignTextOnly);
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("x-rapidapi-host", "aviation-reference-data.p.rapidapi.com");
                    request.AddHeader("x-rapidapi-key", RAPIDAPI_KEY);
                    IRestResponse response = client.Execute(request);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        JObject jsonResponse = JObject.Parse(response.Content);
                        if (jsonResponse.ContainsKey("callSign"))
                        {
                            airlineCallsign = jsonResponse["callSign"].ToString();

                            airlineCallsigns.TryAdd(callsignTextOnly, airlineCallsign);
                        }
                    }
                }
            }

            return airlineCallsign;
        }

        private string LookupAirportName(string airportCode)
        {
            string airportName = "";

            if (airportNames.ContainsKey(airportCode))
            {
                airportName = airportNames.GetOrAdd(airportCode, airportName);
            }
            else
            {
                var client = new RestClient("https://aviation-reference-data.p.rapidapi.com/airports/" + airportCode);
                var request = new RestRequest(Method.GET);
                request.AddHeader("x-rapidapi-host", "aviation-reference-data.p.rapidapi.com");
                request.AddHeader("x-rapidapi-key", "f15025cd2cmsh3af33e491c76174p1c5381jsn03476d3f5108");
                IRestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jsonResponse = JObject.Parse(response.Content);
                    if (jsonResponse.ContainsKey("name"))
                    {
                        airportName = jsonResponse["name"].ToString();

                        airportNames.TryAdd(airportCode, airportName);
                    }
                }
            }

            return airportName;
        }

        double DEG_PER_RAD = (180.0 / Math.PI);
        public double CalculateBearingFromRads(double lat1, double lon1, double lat2, double lon2)
        {
            var dLon = lon2 - lon1;
            var y = Math.Sin(dLon) * Math.Cos(lat2);
            var x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            double brg = DEG_PER_RAD * Math.Atan2(y, x);

            if (brg < 0) brg += 360;
            return brg;
        }

        public static double DegreesToRadians(double angle)
        {
            return angle * Math.PI / 180.0d;
        }
    }

}
