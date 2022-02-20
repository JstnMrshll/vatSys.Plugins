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
    public class LiftoffTimer : IPlugin
    {
        /// The name of the custom label item we've added to Labels.xml in the Profile
        const string LABEL_ITEM = "TIME_SINCE_LIFTOFF";

        ConcurrentDictionary<string, DateTime> trackDateTimeLeftGround = new ConcurrentDictionary<string, DateTime>();

        /// Plugin Name
        public string Name { get => "Liftoff Timer"; }

        public void OnFDRUpdate(FDP2.FDR updated)
        {
        }

        public void OnRadarTrackUpdate(RDP.RadarTrack updated)
        {
            string callsign = updated.ActualAircraft.Callsign;

            if (callsign == "")
                return;

            DateTime dtLiftoff; // = trackDateTimeLeftGround.GetOrAdd(callsign, DateTime.MinValue);

            if (updated.OnGround && trackDateTimeLeftGround.ContainsKey(callsign))
            {
                trackDateTimeLeftGround.TryRemove(callsign, out dtLiftoff);
            }
            else if (!updated.OnGround && updated.CorrectedAltitude < 5000 && !trackDateTimeLeftGround.ContainsKey(callsign))
            {
                dtLiftoff = DateTime.UtcNow;

                trackDateTimeLeftGround.TryAdd(callsign, dtLiftoff);
            }

            //updated.CorrectedAltitude;
            //updated.VerticalSpeed;
        }

        public CustomLabelItem GetCustomLabelItem(string itemType, Track track, FDP2.FDR flightDataRecord, RDP.RadarTrack radarTrack)
        {
            if (itemType != LABEL_ITEM)
                return null;

            string callsign = radarTrack.ActualAircraft.Callsign;

            if (callsign == "" || !trackDateTimeLeftGround.ContainsKey(callsign))
                return null;

            DateTime dtLiftoff = DateTime.MinValue;
            trackDateTimeLeftGround.TryGetValue(callsign, out dtLiftoff);

            if (dtLiftoff == DateTime.MinValue)
                return null;

            TimeSpan tsTimeSinceLiftoff = DateTime.UtcNow - dtLiftoff;

            double displayTime = 3.0;
            if (flightDataRecord != null)
            {
                switch (flightDataRecord.AircraftWake)
                {
                    case "H":
                    case "M":
                        displayTime = 2.0;
                        break;

                    default:
                        break;
                }
            }

            if (tsTimeSinceLiftoff.TotalMinutes <= displayTime)
            {
                return new CustomLabelItem()
                {
                    Type = itemType,
                    ForeColourIdentity = Colours.Identities.StaticTools,
                    Text = tsTimeSinceLiftoff.ToString(" m\\:ss")
                };
            }
            else
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

    }

}
