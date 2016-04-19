// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.Net;

namespace GoogleCloudExtension.DataSources
{
    public class ExtendedWebClient : WebClient
    {
        public string HttpMethod { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var req = base.GetWebRequest(address);

            if (string.IsNullOrWhiteSpace(HttpMethod)) return req;
            if (req != null) req.Method = HttpMethod;

            return req;
        }
    }
}
