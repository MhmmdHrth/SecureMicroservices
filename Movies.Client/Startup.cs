using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Movies.Client.ApiServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Net.Http.Headers;
using Movies.Client.HttpHandlers;
using IdentityModel.Client;
using Microsoft.IdentityModel.Tokens;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;

namespace Movies.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddScoped<IMovieApiService, MovieApiService>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = "https://localhost:5005";

                    options.ClientId = "movies_mvc_client";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code id_token"; //grant_type options

                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("address");
                    options.Scope.Add("email");
                    options.Scope.Add("roles");
                    options.Scope.Add("movieAPI");

                    options.ClaimActions.MapUniqueJsonKey("address", "address");
                    options.ClaimActions.MapUniqueJsonKey("role", "role");

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    //check if token have these claims
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = JwtClaimTypes.GivenName,
                        RoleClaimType = JwtClaimTypes.Role
                    };
                });

            //AUTHORIZATION FLOW
            //1.create httpclient used for accessing Movies.API
            //services.AddTransient<AuthenticationDelegatinHandler>();
            //services.AddHttpClient("MovieAPIClient", options =>
            //{
            //    options.BaseAddress = new Uri("https://localhost:5001/");
            //    options.DefaultRequestHeaders.Clear();
            //    options.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            //}).AddHttpMessageHandler<AuthenticationDelegatinHandler>();

            ////2.create httpclient used for accessing Identity Server Provider
            //services.AddHttpClient("IDPClient", options =>
            //{
            //    options.BaseAddress = new Uri("https://localhost:5005/");
            //    options.DefaultRequestHeaders.Clear();
            //    options.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            //});

            //services.AddSingleton(new ClientCredentialsTokenRequest
            //{
            //    Address = "https://localhost:5005/connect/token",
            //    ClientId = "movieClient",
            //    ClientSecret = "secret",
            //    Scope = "movieAPI"
            //});

            //HYBRID FLOW
            services.AddTransient<AuthenticationDelegatinHandler>();
            services.AddHttpClient("MovieAPIClient", options =>
            {
                options.BaseAddress = new Uri("https://localhost:5001/");
                options.DefaultRequestHeaders.Clear();
                options.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            }).AddHttpMessageHandler<AuthenticationDelegatinHandler>();

            services.AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
