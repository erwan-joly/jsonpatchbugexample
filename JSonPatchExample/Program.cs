using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using ServiceStack.Text;
using JsonSerializer = ServiceStack.Text.JsonSerializer;

namespace JSonPatchExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public class Startup
    {
        private static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
        {
            var builder = new ServiceCollection()
                .AddLogging()
                .AddMvc()
                .AddNewtonsoftJson()
                .Services.BuildServiceProvider();

            return builder
                .GetRequiredService<IOptions<MvcOptions>>()
                .Value
                .InputFormatters
                .OfType<NewtonsoftJsonPatchInputFormatter>()
                .First();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //this works
            //services
            //    .AddControllers()
            //    .AddNewtonsoftJson()
            //    .AddControllersAsServices();

            //this will return null
            services.AddControllers(options =>
            {
                options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
            }).AddControllersAsServices();

            Task.Run(async () =>
            {
                await Task.Delay(1000);
                using var client = new HttpClient();
                var jsonPatch = new JsonPatchDocument<SmallObject>();
                jsonPatch.Replace(s => s.Property, 2);
                using var content = new StringContent(JsonConvert.SerializeObject(jsonPatch), Encoding.Default,
                    "application/json");

                var postResponse = await client
                    .PatchAsync(new Uri($"http://localhost:5000/api/patch"), content)
                    .ConfigureAwait(false);
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
