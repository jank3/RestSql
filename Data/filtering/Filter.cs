using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Data.filtering
{
    public class Filter : BaseFilter
    {
        public BaseFilter LHS { get; set; }
        public Operators Operator { get; set; }
        public BaseFilter RHS { get; set; }

        public static BaseFilter fromJson(String json)
        {
            BaseFilter f = null;
            try
            {
                f = fromJson(JObject.Parse(json));
            }
            catch(Exception ex)
            {

            }
            return f;
        }

        public static BaseFilter fromJson(JObject json)
        {
            BaseFilter f = null;
            switch(json["Type"].ToString())
            {
                case "Filter":
                    f = new Filter();
                    Filter ff = f as Filter;
                    ff.LHS = Filter.fromJson(json["LHS"].Value<JObject>());
                    ff.Operator = json["Operator"].Value<Operators>();
                    ff.RHS = Filter.fromJson(json["RHS"].Value<JObject>());
                    break;
                case "FilterExpression":
                    f = FilterExpression.fromJson(json);
                    break;
            }
            return f;
        }
    }
}
