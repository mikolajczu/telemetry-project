using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mongo.AspNetCore.Identity;

namespace Telemetry.Entities.Models
{
    public class User : MongoIdentityUser
    {
        public virtual ICollection<string> SessionsIds { get; set; }
        public virtual ICollection<string> PagesIds { get; set; }
        public string? Description { get; set; }
    }
}