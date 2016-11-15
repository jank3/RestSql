using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Data.filtering
{
    public class FilterExpression : BaseFilter
    {
        public String ColumnName { get; set; }
        public Operators Operator { get; set; }
        public String Value { get; set; }

        public FilterExpression()
        {

        }

        public FilterExpression(String col, Operators op, String value)
        {
            ColumnName = col;
            Operator = op;
            Value = value;
        }

        public static FilterExpression fromJson(String json)
        {
            FilterExpression fe = null;
            try
            {
                fe = FilterExpression.fromJson(JObject.Parse(json));
            }
            catch(Exception ex)
            {

            }
            return fe;
        }

        public static FilterExpression fromJson(JObject json)
        {
            FilterExpression fe = null;
            if(json["Type"].Value<String>() == "FilterExpression")
            {
                fe = new FilterExpression();
                fe.ColumnName = json["ColumnName"].Value<String>();
                fe.Operator = (Operators)json["Operator"].Value<int>();
                fe.Value = json["Value"].Value<String>();
            }
            return fe;
        }
    }
}
