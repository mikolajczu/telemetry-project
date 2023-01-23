using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Telemetry.Entities.Models
{
    public class TelemetrySession
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string UserId { get; set; } = null!;
        public virtual ICollection<string> PagesIds { get; set; } = new List<string>();
        public DateTime SessionDate { get; set; }
        public double Time { get; set; }
        public byte Status { get; set; } // 0 - OFF, 1 - ON
    }
}
