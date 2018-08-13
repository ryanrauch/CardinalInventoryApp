using System;
using System.Collections.Generic;
using System.Text;

namespace CardinalInventoryApp.Contracts
{
    public class SmartWatchSession
    {
        public Guid SmartWatchSessionId { get; set; }
        public String Description { get; set; }
        public DateTime Timestamp { get; set; }
        public Decimal IntervalDuration { get; set; } // Seconds 00.000 (5,3)
        public int IntervalStart { get; set; }
        public int IntervalStop { get; set; }
        public double AttitudeRollOffset { get; set; } //Radians
        public Guid PourSpoutId { get; set; }
        public PourSpout PourSpout { get; set; }
    }
}
