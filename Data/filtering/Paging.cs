using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Data.filtering
{
    public class Paging
    {
        public int PageNumber = 0;
        public int CountPerPage = 0;

        public Paging()
        {

        }

        public Paging(int pgNum, int count)
        {
            PageNumber = pgNum;
            CountPerPage = count;
        }

        public static Paging fromJson(String json)
        {
            Paging pg = null;
            try
            {
                pg = Paging.fromJson(JObject.Parse(json));
            }
            catch (Exception ex)
            {

            }
            return pg;
        }

        public static Paging fromJson(JObject json)
        {
            Paging pg = null;
            if (json["Type"].Value<String>() == "Paging")
            {
                pg = new Paging();
                pg.PageNumber = json["PageNumber"].Value<int>();
                pg.CountPerPage = json["CountPerPage"].Value<int>();
            }
            return pg;
        }
    }
}
