using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telemetry.Entities.Models
{
    public class TelemetrySession
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<UserPage> Pages { get; set; }
        public double Time { get; set; }
        public byte Status { get; set; } // 0 - OFF, 1 - ON
    }
}
