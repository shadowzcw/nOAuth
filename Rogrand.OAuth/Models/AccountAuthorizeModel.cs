﻿using System.Collections.Generic;
using DotNetOpenAuth.OAuth2.Messages;

namespace nOAuth.AuthorizationServer.Models {
    public class AccountAuthorizeModel {
		public string ClientApp { get; set; }

		public HashSet<string> Scope { get; set; }

		public EndUserAuthorizationRequest AuthorizationRequest { get; set; }
	}
}
