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
        ConcurrentDictionary<string, string> airlineCallsigns = new ConcurrentDictionary<string, string>();
        ConcurrentDictionary<string, string> airportNames = new ConcurrentDictionary<string, string>();

        /// Plugin Name
        public string Name { get => "FDR Enhancer"; }

        public void OnFDRUpdate(FDP2.FDR updated)
        {
            if (updated.State == FDP2.FDR.FDRStates.STATE_PREACTIVE && updated.RunwayString != null && updated.RunwayString.Length > 0)
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

                if (airportName.Length + waypoint.Length > 13) airportName = airportName.Substring(0, (13 - waypoint.Length));
                if (waypoint != "") sep = "*";

                updated.LocalOpData = airportName + sep + waypoint;

                //if (updated.IsTrackedByMe)
                //updated.Remarks += $"/FDRE/ AIRPORT/{airportName} AIRLINE/{airlineCallsign}";

                //updated.LabelOpData
            }
            else if (updated.State == FDP2.FDR.FDRStates.STATE_PREACTIVE && (updated.RunwayString==null || updated.RunwayString==""))
            {
                string airlineCallsign = LookupAirlineCallsign(updated.Callsign);
                updated.LocalOpData = airlineCallsign;
            }

            //if (updated.IsTrackedByMe && updated.TextOnly && (updated.GlobalOpData == null || updated.GlobalOpData == "" || !updated.GlobalOpData.Contains("TXT")))
            //{
            //    updated.GlobalOpData += (updated.GlobalOpData.Length > 0 ? " " : "") + "TXT";
            //}

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
                    request.AddHeader("x-rapidapi-key", "zzzzz");
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
                request.AddHeader("x-rapidapi-key", "zzzzz");
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

    }

}
