using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Data.filtering
{
    public class Sort
    {
        /// <summary>
        /// Order is important
        /// </summary>
        public List<String> ColumnNames = new List<String>();
        public bool Ascending = false;

        public Sort()
        {

        }

        public Sort(String col)
        {
            ColumnNames.Add(col);
        }

        public Sort(String col, bool asc)
        {
            ColumnNames.Add(col);
            Ascending = asc;
        }

        public static Sort fromJson(String json)
        {
            Sort s = null;
            try
            {
                s = Sort.fromJson(JObject.Parse(json));
            }
            catch (Exception ex)
            {

            }
            return s;
        }

        public static Sort fromJson(JObject json)
        {
            Sort s = null;
            if (json["Type"].Value<String>() == "Sort")
            {
                s = new Sort();
                s.ColumnNames.Clear();
                s.ColumnNames.AddRange(json["ColumnNames"].Value<String>().Split(','));
                s.Ascending = json["Ascending"].Value<bool>();
            }
            return s;
        }
    }
}
