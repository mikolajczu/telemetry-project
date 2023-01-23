using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Telemetry.Entities.Models
{
    public class Page
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Title { get; set; } = null!;
    }
}
