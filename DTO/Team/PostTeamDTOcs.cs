﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Team
{
    public class PostTeam
    {
		[Required]
		[RegularExpression(@"^[a-zA-Z0-9_-]*$")]
		public string name {  get; set; }

		[Required]
		public Guid clientid { get; set; }

        public List<Guid> userids { get; set; }
    }
}
