using System;
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

        public HomeModule()
        {
            Get["/"] = _ => "Practice";
            Get["/GetEnvironmentFromPractice"] = _ => "Hello World";

            Get["/{practiceAbbreviation}"] = parameters =>
            {
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
