using Esso.Models;
using System.Collections.Generic;
using System.Data.Entity;

namespace Esso.Data
{
    public class EssoSeedData : DropCreateDatabaseIfModelChanges<EssoEntities>
    {


        protected override void Seed(EssoEntities context)
        {

            //GetTags().ForEach(g => context.Tags.Add(g));
            //GetCompanies().ForEach(g => context.Companies.Add(g));
            context.Commit();
        }

        private static List<TBL_TAG> GetTags()
        {
            return new List<TBL_TAG>
            {
                new TBL_TAG {
                    NAME = "AC Akım"
                },
                new TBL_TAG {
                    NAME = "DC Akım"
                },
                new TBL_TAG {
                    NAME = "AC Güç"
                },
                new TBL_TAG {
                    NAME = "DC Güç"
                }
            };
        }

        private static List<TBL_COMPANY> GetCompanies()
        {
            return new List<TBL_COMPANY>
            {
                new TBL_COMPANY {
                    NAME = "ESSO"
                },
                new TBL_COMPANY {
                    NAME = "ENER"
                }
            };
        }
    }
}
