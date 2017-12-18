using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Json;
using Nancy.Owin;
using Nancy.TinyIoc;
using NancyUtilities;
using Owin;

[assembly: OwinStartup(typeof(MsLogDetail.Startup))]

namespace MsLogDetail
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

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            var hostname = context.Request.Url.HostName;
            var port = context.Request.Url.Port;
            var sqlSpCmd = "SHMS.dbo.CreateOnlyIfNewServiceInfo";
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SHMS"].ConnectionString;
            LogServerInstance.UpdateDbWithServerInfo(connectionString, sqlSpCmd, hostname, (port ?? 0).ToString(), "ServicesList");
        }
    }
}
