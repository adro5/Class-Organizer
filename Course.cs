﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace College_Organizer
{
    [JsonObject(MemberSerialization.Fields)]
    public class Course
    {
        public string courseName { get; set; }

        public string noteName { get; set; }

        public string notes { get; set; }
    }
}