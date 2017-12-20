using System;
using System.Diagnostics;
using System.Linq;
using Nancy;
using NancyUtilities;
using Nancy.Responses;
using PV.App.Managers.Standard;
using PV.App.Managers.Standard.Helpers;
using PV.Data.Standard.EntityClasses;
using PV.Data.Standard.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace MsBilling
{
    public class BillingModel
    {
        public BillingModel(PatBillingEntity entity)
        {
            Practice = entity.Practice;
            PayerType = entity.PayerType;
            PayerClass = entity.PayerClass;
            PatientNumber = entity.PatNum;

            EmployerPk = entity.EmpEmployerPk;
            CompanyPk = entity.PrimCompanyPk;
            InsurancePk = entity.PrimInsurancePk;

            EffectiveDate = entity.EftDate;
        }

            public string Practice { get; set; }
            public string PayerType { get; set; }
            public string PayerClass { get; set; }
            public string VisitType { get; set; }
            public int PatientNumber { get; set; }


            public Guid? EmployerPk { get; set; }
            public Guid? CompanyPk { get; set; }
            public Guid? InsurancePk { get; set; }

            public DateTime EffectiveDate { get; set; }
    }

    public class HomeModule : NancyModule
    {
        private string _methodName;
        private string _methodAction;
        private string _query;
        private string _requestor;
        private Stopwatch _timer ;
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
                _timer = Stopwatch.StartNew();
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
                    _timer.ElapsedMilliseconds.ToString()
                );
            };

            Get["/"] = _ => "Billing";
            Get["GetPatBilling/{practice}&{patNum}"] = parameters =>
            {
                UpdateActionParams("GetPatBilling", "Get");
                var results = GetPatBilling(parameters.practice, parameters.patNum);
                return new JsonResponse(results, new DefaultJsonSerializer());
            };
        }

        private static BillingModel GetPatBilling(string practice, int patNum)
        {
            var environment = PracticeManager.GetEnvironmentFromPractice(practice);

            var collection = new EntityCollection<PatBillingEntity>();

            var bucket = new RelationPredicateBucket();
            bucket.PredicateExpression.Add(PatBillingFields.Practice == practice);
            bucket.PredicateExpression.Add(PatBillingFields.BillTo == "P");
            bucket.PredicateExpression.Add(PatBillingFields.PatNum == patNum);

            using (var adapter = AdapterFactory.CreateAdapter(MyNameSpace, environment)) { adapter.FetchEntityCollection(collection, bucket); }

            return collection.Count < 1 ? null : new BillingModel(collection.Single());
        }
    }
}
