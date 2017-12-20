using System;
using System.Timers;
using NancyUtilities;
using Nancy;
using Nancy.Responses;
using PV.App.Managers.Standard.Helpers;
using PV.Data.Standard;
using PV.Data.Standard.EntityClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace MsPractice
{
    public class PracticeModel
    {
        public PracticeModel(PracticeEntity entity)
        {
            Practice = entity.PracticeAbbr;
            Practice_ID = entity.PracticeId;
            Environment = entity.Environment;
            Address1 = entity.Address1;
            Address2 = entity.Address2;
            City = entity.City;
            State = entity.State;
            Zip = entity.Zip;
            Phone = entity.Phone;

            PracticePK = entity.PracticePk;
        }

        public string Practice;
        public int Practice_ID;
        public string Environment;
        public string Address1;
        public string Address2;
        public string City;
        public string State;
        public string Zip;
        public string Phone;
        Guid PracticePK;
    }

    public class HomeModule : NancyModule
    {
        protected static string MyNameSpace = "PV.Data.Standard.DataManagers";
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
                    "MsPractice",
                    _methodName,
                    _methodAction,
                    _requestor,
                    _query,
                    _timer.Interval.ToString()
                );
            };

            Get["/"] = _ => "Practice";
            Get["/GetEnvironmentFromPractice"] = _ => "Hello World";
            Get["/{practiceAbbreviation}"] = parameters =>
            {
                UpdateActionParams("", "Get");
                var results = GetPracticeInfo(parameters.practiceAbbreviation);
                return new JsonResponse(results, new DefaultJsonSerializer());

            };
            Get["GetPracticeInfo/{practiceAbbreviation}"] = parameters =>
            {
                UpdateActionParams("GetPracticeInfo", "Get");
                var results = GetPracticeInfo(parameters.practiceAbbreviation);
                return new JsonResponse(results, new DefaultJsonSerializer());

            };
        }

        private static PracticeModel GetPracticeInfo(string practiceAbbr)
        {
            var results = FetchPractice(practiceAbbr);

            return new PracticeModel(results);
        }


        private static PracticeEntity FetchPractice(string practiceAbbr)
        {
            var path = new PrefetchPath2((int)EntityType.PracticeEntity);
            path.Add(PracticeEntity.PrefetchPathClinics);

            var practiceEntity = new PracticeEntity();
            practiceEntity.PracticeAbbr = practiceAbbr;
            using (IDataAccessAdapter adapter = AdapterFactory.CreateAdapter(MyNameSpace))
            {
                bool fetchResult = adapter.FetchEntityUsingUniqueConstraint(practiceEntity,
                                                                            practiceEntity.ConstructFilterForUCPracticeAbbr(),
                                                                            path);
                if (!fetchResult)
                {
                    // Not found. Place handle code here.
                    return null;
                }
            }
            return practiceEntity;
        }
    }
}
