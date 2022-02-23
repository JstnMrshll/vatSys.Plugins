using RestSharp;
using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using vatsys;
using vatsys.Plugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace vatSys.Plugins
{
    [Export(typeof(IPlugin))]
    public class LiftoffTimer : IPlugin
    {
        /// The name of the custom label item we've added to Labels.xml in the Profile
        const string LABEL_ITEM = "TIME_SINCE_LIFTOFF";

        ConcurrentDictionary<string, TrackingAircraftStatus> trackDateTimeLeftGround = new ConcurrentDictionary<string, TrackingAircraftStatus>();

        /// Plugin Name
        public string Name { get => "Liftoff Timer"; }

        public void OnFDRUpdate(FDP2.FDR updated)
        {
        }

        public void OnRadarTrackUpdate(RDP.RadarTrack updated)
        {
            if (updated.ActualAircraft != null)
            {
                string callsign = updated.ActualAircraft.Callsign;

                if (callsign == "")
                    return;

                TrackingAircraftStatus acStatus = new TrackingAircraftStatus(updated.OnGround, updated.CorrectedAltitude);

                acStatus = trackDateTimeLeftGround.GetOrAdd(callsign, acStatus);

                if (acStatus != null && acStatus.OnGround && !updated.OnGround && (updated.CorrectedAltitude > (acStatus.Altitude + 50)))
                {
                    TrackingAircraftStatus newStatus = new TrackingAircraftStatus(updated.OnGround, DateTime.UtcNow);

                    trackDateTimeLeftGround.TryUpdate(callsign, newStatus, acStatus);
                }
            }
        }

        public CustomLabelItem GetCustomLabelItem(string itemType, Track track, FDP2.FDR flightDataRecord, RDP.RadarTrack radarTrack)
        {
            if (itemType != LABEL_ITEM || radarTrack == null || radarTrack.ActualAircraft == null)
                return null;

            string callsign = radarTrack.ActualAircraft.Callsign;

            if (callsign == "" || !trackDateTimeLeftGround.ContainsKey(callsign))
                return null;

            TrackingAircraftStatus acStatus = null;
            trackDateTimeLeftGround.TryGetValue(callsign, out acStatus);

            if (acStatus == null || acStatus.OnGround || acStatus.LiftoffTime == DateTime.MinValue)
                return null;

            TimeSpan tsTimeSinceLiftoff = DateTime.UtcNow - acStatus.LiftoffTime;

            double displayTime = 3.0;
            if (flightDataRecord != null)
            {
                switch (flightDataRecord.AircraftWake)
                {
                    case "H":
                    case "M":
                        displayTime = 2.0;
                        break;

                    case "L":
                        displayTime = 0.0;
                        break;

                    default:
                        break;
                }
            }

            //Colours.Identities color;
            //if (flightDataRecord.IsTrackedByMe)
            //    color = Colours.Identities.StaticTools;

            if (tsTimeSinceLiftoff.TotalMinutes <= displayTime)
            {
                return new CustomLabelItem()
                {
                    Type = itemType,
                    ForeColourIdentity = Colours.Identities.StaticTools,
                    Text = tsTimeSinceLiftoff.ToString("m\\:ss")
                };
            }
            else
            {
                if (trackDateTimeLeftGround.ContainsKey(callsign))
                    trackDateTimeLeftGround.TryRemove(callsign, out acStatus);

                return null;
            }
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
