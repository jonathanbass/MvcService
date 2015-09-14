using System;
using System.Linq;
using System.ServiceProcess;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Hosting.Internal;
using Microsoft.AspNet.StaticFiles;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Runtime;

namespace MvcService
{
    public class Program : ServiceBase
    {
        private readonly IServiceProvider _serviceProvider;
        private IHostingEngine _hostingEngine;
        private IDisposable _shutdownServerDisposable;
        private readonly IApplicationEnvironment _applicationEnvironment;

        public Program(IServiceProvider serviceProvider, IApplicationEnvironment applicationEnvironment)
        {
            _serviceProvider = serviceProvider;
            _applicationEnvironment = applicationEnvironment;
        }

        public void Main(string[] args)
        {
            if (args.Contains("--windows-service"))
            {
                Run(this);
                return;
            }

            OnStart(null);
            Console.ReadLine();
            OnStop();
        }

        protected override void OnStart(string[] args)
        {
            var config = new ConfigurationBuilder(new JsonConfigurationSource($@"{_applicationEnvironment.ApplicationBasePath}\config.json")).Build();
     
            var builder = new WebHostBuilder(_serviceProvider, config);
            builder.UseServer("Microsoft.AspNet.Server.WebListener");
            builder.UseServices(services => services.AddMvc());
            builder.UseStartup(appBuilder =>
            {
                appBuilder.UseDefaultFiles();
                appBuilder.UseStaticFiles();
                appBuilder.UseMvc(routes =>
                {
                    routes.MapRoute(
                        null,
                        "{controller}/{action}",
                        new { controller = "Home", action = "Index" });
                });
            });

            _hostingEngine = builder.Build();
            _shutdownServerDisposable = _hostingEngine.Start();
        }

        protected override void OnStop()
        {
            _shutdownServerDisposable?.Dispose();
        }
    }
}
