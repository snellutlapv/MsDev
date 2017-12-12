using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Responses;
using PV.App.Managers.Standard;
using PV.App.Managers.Standard.Helpers;
using PV.Data.DataModel.PatientFlow;
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
        protected static string MyNameSpace = "PV.Data.Standard.DataManagers";

        public HomeModule()
        {
            Get["/"] = _ => "Billing";
            Get["GetPatBilling/{practice}&{patNum}"] = parameters =>
            {
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
