using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telemetry.Entities.Models
{
    public class UserPage
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public int PageId { get; set; }
        public virtual Page Page { get; set; }
        public int TelemetrySessionId { get; set; }
        public virtual TelemetrySession TelemetrySession { get; set; }
        public double Time { get; set; }
    }
}
