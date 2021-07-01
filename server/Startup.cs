using System.Security.Claims;
using Kcsara.Respond.Data;
using Kcsara.Respond.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Kcsara.Respond
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      if (string.IsNullOrWhiteSpace(Configuration["USE_SQLITE"]) && string.IsNullOrWhiteSpace(Configuration.GetConnectionString(SqlServerRespondDbContext.CONNECTION_CONFIG)))
      {
        services.AddDbContext<RespondDbContext, SqlServerRespondDbContext>();
      }
      else
      {
        services.AddDbContext<RespondDbContext, SqliteRespondDbContext>();
      }

      services.AddSignalR();
      services.AddControllersWithViews();
      // In production, the React files will be served from this directory
      services.AddSpaStaticFiles(configuration =>
      {
        configuration.RootPath = "ClientApp/build";
      });

      SetupAuthentication(services);
    }


    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RespondDbContext db)
    {
      db.Database.Migrate();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      //app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseSpaStaticFiles();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller}/{action=Index}/{id?}");
        endpoints.MapHub<RespondHub>("/hub");
      });

      app.UseSpa(spa =>
      {
        spa.Options.SourcePath = "ClientApp";

        if (env.IsDevelopment())
        {
          spa.UseReactDevelopmentServer(npmScript: "start");
        }
      });
    }

    private void SetupAuthentication(IServiceCollection services)
    {
      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
      })
      .AddCookie()
      .AddOpenIdConnect("kcsara", options =>
      {
        // Set the authority to your Auth0 domain
        options.Authority = $"https://login.kingcountysar.org";

        // Configure the Auth0 Client ID and Client Secret
        options.ClientId = Configuration["auth:ClientId"];
        options.ClientSecret = Configuration["auth:ClientSecret"];

        // Set response type to code
        options.ResponseType = OpenIdConnectResponseType.Code;

        // Configure the scope
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("email");
        options.Scope.Add("profile");
        options.Scope.Add("kcsara-profile");

        options.ClaimActions.Add(new UniqueJsonKeyClaimAction("d4hId", ClaimValueTypes.String, "d4hId"));

        // Set the callback path, so Auth0 will call back to http://localhost:3000/callback
        // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard
        options.CallbackPath = new PathString("/finish-login");

        // Configure the Claims Issuer to be Auth0
        options.ClaimsIssuer = "kcsara";
        options.GetClaimsFromUserInfoEndpoint = true;

        options.SaveTokens = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
          NameClaimType = "name"
        };
      });
    }
  }
}
