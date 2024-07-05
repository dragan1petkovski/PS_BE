﻿using DTOModel.TeamDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel.CredentialDTO
{
    public class CredentialDTO
    {
        public Guid id {  get; set; }
        public string domain { get; set; }
        public string username {  get; set; }
        public string email { get; set; }
        public string remote { get; set; }
        public string password { get; set; }

        public string teamname { get; set; }
        public Guid teamid { get; set; }
        public string note { get; set; }
    }
}
