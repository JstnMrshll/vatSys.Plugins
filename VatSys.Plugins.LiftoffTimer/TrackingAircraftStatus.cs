using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vatSys.Plugins
{
    class TrackingAircraftStatus
    {
        public bool OnGround;
        public DateTime LiftoffTime;
        public int Altitude;

        public TrackingAircraftStatus()
        {
            this.OnGround = true;
            this.LiftoffTime = DateTime.MinValue;
            this.Altitude = 0;
        }

        public TrackingAircraftStatus(bool OnGround)
        {
            this.OnGround = OnGround;
            this.LiftoffTime = DateTime.MinValue;
            this.Altitude = 0;
        }

        public TrackingAircraftStatus(bool OnGround, int Altitude)
        {
            this.OnGround = OnGround;
            this.Altitude = Altitude;
        }

        public TrackingAircraftStatus(bool OnGround, DateTime LiftoffTime)
        {
            this.OnGround = OnGround;
            this.LiftoffTime = LiftoffTime;
            this.Altitude = 0;
        }

        public TrackingAircraftStatus(bool OnGround, DateTime LiftoffTime, int Altitude)
        {
            this.OnGround = OnGround;
            this.LiftoffTime = LiftoffTime;
            this.Altitude = Altitude;
        }

    }
}
