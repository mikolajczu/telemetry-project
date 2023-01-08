using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telemetry.Entities.Models
{
    public class User : IdentityUser
    {
        public virtual ICollection<TelemetrySession> Sessions { get; set; }
        public virtual ICollection<UserPage> Pages { get; set; }
        public string? Description { get; set;}

    }
}
