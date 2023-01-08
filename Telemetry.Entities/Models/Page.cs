﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telemetry.Entities.Models
{
    public class Page
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual ICollection<UserPage> Pages { get; set; }
    }
}