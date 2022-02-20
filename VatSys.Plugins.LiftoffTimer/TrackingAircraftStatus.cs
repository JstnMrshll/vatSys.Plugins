using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VatSys.Plugins
{
    class TrackingAircraftStatus
    {
        public bool OnGround;
        public DateTime LiftoffTime;

        public TrackingAircraftStatus()
        {
            this.OnGround = true;
            this.LiftoffTime = DateTime.MinValue;
        }

        public TrackingAircraftStatus(bool OnGround)
        {
            this.OnGround = OnGround;
            this.LiftoffTime = DateTime.MinValue;
        }

        public TrackingAircraftStatus(bool OnGround, DateTime LiftoffTime)
        {
            this.OnGround = OnGround;
            this.LiftoffTime = LiftoffTime;
        }

    }
}
