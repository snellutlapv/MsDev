using System;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Json;
using Nancy.Owin;
using Nancy.TinyIoc;
using NancyUtilities;
using Owin;

[assembly: OwinStartup(typeof(MsPractice.Startup))]

namespace MsPractice
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            app.UseNancy(options => options.PassThroughWhenStatusCodesAre(
                    HttpStatusCode.NotFound,
                    HttpStatusCode.InternalServerError))
                .UseStageMarker(PipelineStage.MapHandler)
               ;
        }
    }

    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            JsonSettings.MaxJsonLength = Int32.MaxValue;
        }
        public static void LoadInstanceInfoToDb(string url)
        {
            var urlStringParams = url.Split(':');
            var port = urlStringParams.Length > 2 ? urlStringParams[2] : "0";
            var sqlSpCmd = "SHMS.dbo.CreateOnlyIfNewServiceInfo";
            var host = urlStringParams.Length > 1 ? $"{urlStringParams[0]}:{urlStringParams[1]}" : $"Unknown-{Guid.NewGuid()}";
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SHMS"].ConnectionString;
            LogServerInstance.UpdateDbWithServerInfo(connectionString, sqlSpCmd, host, port, "ListManager");
        }
    }
}
