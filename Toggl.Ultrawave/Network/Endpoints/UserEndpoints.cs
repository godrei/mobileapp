﻿using System;

namespace Toggl.Ultrawave.Network
{
    internal struct UserEndpoints
    {
        private readonly Uri baseUrl;

        public UserEndpoints(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Endpoint Get => Endpoint.Get(baseUrl, "me");

        public Endpoint GetWithGoogle => Endpoint.Get(baseUrl, "me?app_name=toggl_mobile");

        public Endpoint Post => Endpoint.Post(baseUrl, "signup");

        public Endpoint Put => Endpoint.Put(baseUrl, "me");

        public Endpoint ResetPassword => Endpoint.Post(baseUrl, "me/lost_passwords");
    }
}