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

            DateTime dtLiftoff = trackDateTimeLeftGround.GetOrAdd(callsign, DateTime.MinValue);

            if (!updated.OnGround && dtLiftoff == DateTime.MinValue)
            {
                trackDateTimeLeftGround.TryUpdate(callsign, DateTime.UtcNow, DateTime.MinValue);

                dtLiftoff = DateTime.UtcNow;
            }

            //updated.CorrectedAltitude;
            //updated.VerticalSpeed;
        }

        public CustomLabelItem GetCustomLabelItem(string itemType, Track track, FDP2.FDR flightDataRecord, RDP.RadarTrack radarTrack)
        {
            //if (itemType != LABEL_ITEM)
            //    return null;

            //DateTime dtLiftoff=DateTime.MinValue;
            //trackDateTimeLeftGround.TryGetValue(flightDataRecord.Callsign, out dtLiftoff);

            //if (dtLiftoff == DateTime.MinValue)
            //    return null;

            //string callsign = radarTrack.ActualAircraft.Callsign;

            //TimeSpan tsTimeSinceLiftoff = DateTime.UtcNow - dtLiftoff;

            //if (tsTimeSinceLiftoff.TotalMinutes <= 3.0)
            //{
            //    return new CustomLabelItem()
            //    {
            //        Type = itemType,
            //        ForeColourIdentity = Colours.Identities.StaticTools,
            //        Text = tsTimeSinceLiftoff.ToString("m:ss")
            //    };
            //}
            //else
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
