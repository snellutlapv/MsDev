using System;
using System.Timers;
using Nancy;
using Nancy.Responses;
using NancyUtilities;
using PV.App.Managers.Standard.Helpers;
using PV.Data.Standard;
using PV.Data.Standard.EntityClasses;
using PV.Data.Standard.HelperClasses;
using PV.Data.Standard.TypedViewClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace MsLogDetail
{
    public class LogDetailModel
    {
        public LogDetailModel(LogDetailEntity entity)
        {
            LogNum = entity.LogNum;
            Practice = entity.Practice;
            Clinic = entity.Clinic;
            PatNum = entity.PatNum;
            SvcDate = entity.SvcDate;
            VisitType = entity.VisitType;
            CmpNum = entity.CmpNum;
            Protocol = entity.Protocol;
            PhyNum = entity.PhyNum;
            Status = entity.Status;
            Notes = entity.Notes;
            TimeIn = entity.TimeIn;
            TimeOut = entity.TimeOut;
        }

        public int LogNum;
        public string Practice;
        public string Clinic;
        public int PatNum;
        private DateTime SvcDate;
        public string VisitType;
        public int CmpNum;
        public string Protocol;
        public int PhyNum;
        public string Status;
        public string Notes;
        public DateTime TimeIn;
        public DateTime? TimeOut;
    }

    public class HomeModule : NancyModule
    {
        private string _methodName;
        private string _methodAction;
        private string _query;
        private string _requestor;
        private Timer _timer = new Timer();
        private string _serviceDomainUrl;

        private void UpdateActionParams(string methodname, string action)
        {
            _methodAction = action;
            _methodName = methodname;
        }

        private string conn = System.Configuration.ConfigurationManager.ConnectionStrings["SHMS"].ConnectionString;
        protected static string MyNameSpace = "PV.Data.Standard.DataManagers";

        public HomeModule()
        {
            
            Before += (ctx) =>
            {
                _serviceDomainUrl = ctx.Request.Url.HostName;
                _query = ctx.Request.Path;
                _requestor = ctx.Request.Headers.Referrer;
                _timer.Start();
                return null;
            };

            After += _ =>
            {
                _timer.Stop();

                LogServerInstance.UpdateDbWithServerLog(
                    conn,
                    _serviceDomainUrl,
                    "MsLogDetail",
                    _methodName,
                    _methodAction,
                    _requestor,
                    _query,
                    _timer.Interval.ToString()
                );
            };

            Get["/"] = _ => "Log Detail";
            Get["GetLogDetail/{logDetailPk}&{environment}"] = parameters =>
            {
                UpdateActionParams("GetLogDetail", "Get");
                var results = GetLogDetail(parameters.logDetailPk, parameters.environment);
                return new JsonResponse(results, new DefaultJsonSerializer());

            };
        }

        private LogDetailModel GetLogDetail(string logDetailPk, string environment)
        {
            Guid logDetailPkGuid;
            if (!Guid.TryParse(logDetailPk, out logDetailPkGuid))
            {
                return null;
            }

            using (IDataAccessAdapter adapter = AdapterFactory.CreateAdapter(MyNameSpace, environment))
            {

                var toFetch = new LogDetailEntity(logDetailPkGuid);
                var prefetch = new PrefetchPath2(EntityType.LogDetailEntity) { LogDetailEntity.PrefetchPathPhysician };
                var fetchResult = adapter.FetchEntity(toFetch, prefetch);
                return !fetchResult ? null : new LogDetailModel(toFetch);

            }
        }
    }
}
