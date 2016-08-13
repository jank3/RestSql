using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Data.mysql
{
    public class Query : Data.Query
    {
        public override void Load(DbConnection dbConn)
        {
            base.Load(dbConn);
            String sql = "show create procedure " + Name;

            try
            {
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = sql;
                var reader = cmd.ExecuteReader();
                while (reader.HasRows && reader.Read())
                {
                    String def = reader[2].ToString();
                    String pattern = @"((IN|OUT)?\s+([a-z0-9_]+)\s+([a-z]+\([0-9]+\)))";
                    Regex rx = new Regex(pattern);
                    Match mt = rx.Match(def);
                    while (mt.Success)
                    {
                        if (mt.Groups.Count > 3)
                        {
                            Property prop = new Property();
                            prop.Direction = mt.Groups[1].Value;
                            prop.Name = mt.Groups[2].Value;
                            prop.Type = mt.Groups[3].Value;
                            Parameters.Add(prop);
                        }
                        mt = mt.NextMatch();
                    }
                }
                reader.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error loading Query: " + Name);
            }
        }
    }
}
