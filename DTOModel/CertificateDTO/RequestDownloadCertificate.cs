﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel
{
	public class RequestDownloadCertificate
	{
		public Guid certificateId {  get; set; }
		public Guid teamId { get; set; }
	}
}
