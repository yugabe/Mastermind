using Mastermind.Api.Data;
using Mastermind.Api.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mastermind.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) => 
            services.AddDbContext<MastermindDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString(nameof(MastermindDbContext)) ?? "Server=(localdb)\\mssqllocaldb;Database=Mastermind;Trusted_Connection=True;MultipleActiveResultSets=true"))
                .AddIdentityCore<IdentityUser>()
                    .AddEntityFrameworkStores<MastermindDbContext>().Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(ConfigureCookieOptions).Services
                .AddControllers()
                    .AddJsonOptions(ConfigureJsonOptions).Services
                .AddSwaggerGen(o => o.SwaggerDoc("v1", new OpenApiInfo { Title = "Mastermind API", Version = "v1" }))
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<GameRepository>()
                .AddScoped<GameService>()
                .AddScoped<ScoreRepository>()
                .AddScoped<UserService>();

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) =>
            app.UseWhen(_ => env.IsDevelopment(), dev => dev.UseDeveloperExceptionPage().UseDatabaseErrorPage())
                .UseWhen(_ => !env.IsDevelopment(), prod => prod.UseHsts())
                .Use(HandleUserException)
                .UseHttpsRedirection()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .Use(HandleInvalidCookie)
                .UseSwagger()
                .UseSwaggerUI(ConfigureSwaggerUI)
                .UseEndpoints(e => e.MapControllers());

        private static async Task HandleUserException(HttpContext context, Func<Task> nextRequestDelegate)
        {
            try
            {
                await nextRequestDelegate();
            }
            catch (UserException uex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";

                await JsonSerializer.SerializeAsync(context.Response.Body, new ProblemDetails
                {
                    Detail = uex.Message,
                    Status = context.Response.StatusCode,
                    Title = "The was an error in the provided request.",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                }, context.RequestServices.GetRequiredService<IOptions<JsonSerializerOptions>>().Value);
            }
        }

        private static async Task HandleInvalidCookie(HttpContext context, Func<Task> nextRequestDelegate)
        {
            var userService = context.RequestServices.GetRequiredService<UserService>();
            var userId = userService.GetCurrentUserId();
            if (context.User.Identity.IsAuthenticated && await context.RequestServices.GetRequiredService<MastermindDbContext>().Users.AllAsync(u => u.Id != userId))
                await userService.HttpCookieSignOutAsync();
            await nextRequestDelegate();
        }

        private static void ConfigureJsonOptions(JsonOptions options)
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        }


        private static void ConfigureCookieOptions(CookieAuthenticationOptions options) =>
            options.Events = new CookieAuthenticationEvents
            {
                OnRedirectToLogin = c =>
                {
                    c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return Task.CompletedTask;
                },
                OnRedirectToAccessDenied = c =>
                {
                    c.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return Task.CompletedTask;
                }
            };

        private static void ConfigureSwaggerUI(SwaggerUIOptions options)
        {
            options.DefaultModelRendering(ModelRendering.Model);
            options.DisplayRequestDuration();
            options.DocExpansion(DocExpansion.List);
            options.EnableDeepLinking();
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Mastermind API V1");
        }
    }
}
