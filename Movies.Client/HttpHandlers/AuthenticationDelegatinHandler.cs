using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Movies.Client.HttpHandlers
{
    public class AuthenticationDelegatinHandler : DelegatingHandler
    {

        //AUTHORIZATION FLOW
        //private readonly HttpClient httpClient;
        //private readonly ClientCredentialsTokenRequest tokenRequest;

        //public AuthenticationDelegatinHandler(IHttpClientFactory httpClientFactory, ClientCredentialsTokenRequest clientCredentialsTokenRequest)
        //{
        //    IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        //    this.httpClient = _httpClientFactory.CreateClient("IDPClient");

        //    this.tokenRequest = clientCredentialsTokenRequest ?? throw new ArgumentNullException(nameof(clientCredentialsTokenRequest));
        //}

        //protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        //{
        //    var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(tokenRequest);
        //    if (tokenResponse.IsError)
        //    {
        //        throw new HttpRequestException("Something went wrong while requesting the access token");
        //    }

        //    request.SetBearerToken(tokenResponse.AccessToken);
        //    return await base.SendAsync(request, cancellationToken);
        //}


        //HYBRID FLOW
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthenticationDelegatinHandler(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.SetBearerToken(accessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
