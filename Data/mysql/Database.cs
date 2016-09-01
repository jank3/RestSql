using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Data.mysql
{
    public class Database : Data.Database
    {
        static DateTime m_LastClean = DateTime.MaxValue;
        public Database():base()
        {
            this.DatabaseType = DatabaseTypes.DB_TYPES.MySQL;
        }

        public Database(Data.Database db):base(db)
        {
            this.DatabaseType = DatabaseTypes.DB_TYPES.MySQL;
        }

        public override void Connect()
        {
            base.Connect();
            LoadEntities();
            LoadQueries();
        }

        public override string getConnectionString()
        {
            String connStr = String.Format(
                "Server={0};Database={1};Uid={2};Pwd={3};MaximumPoolsize=1100",
                ServerAddress,
                Name,
                UserName,
                Utilities.StringEx.ConvertToUnsecureString(Password));
            // TODO : Add ConnectionSettings

            return connStr;
        }

        public override void LoadEntities()
        {
            MySql.Data.MySqlClient.MySqlConnection dbConn = this.ActiveConnection as MySql.Data.MySqlClient.MySqlConnection;
            if (dbConn != null && dbConn.State == System.Data.ConnectionState.Open)
            {
                // get tables
                String sql = "Show tables;";
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = sql;
                var reader = cmd.ExecuteReader();
                Entities.Clear();
                while (reader.HasRows && reader.Read())
                {
                    Entity ent = new Entity();
                    Object obj = reader[0];
                    ent.Name = obj.ToString();
                    Entities.Add(ent);
                }
                reader.Close();
                foreach(Entity ent in Entities)
                    ent.Load(dbConn);
            }
        }

        public override void LoadQueries()
        {
            MySql.Data.MySqlClient.MySqlConnection dbConn = this.ActiveConnection as MySql.Data.MySqlClient.MySqlConnection;
            if (dbConn != null && dbConn.State == System.Data.ConnectionState.Open)
            {
                // get procedures
                String sql = "SHOW PROCEDURE STATUS;";
                //String sql = "SHOW FUNCTION STATUS;";
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = sql;
                var reader = cmd.ExecuteReader();
                Queries.Clear();
                while (reader.HasRows && reader.Read())
                {
                    Query q = new Query();
                    q.Name = reader[1].ToString();
                    Queries.Add(q);
                }
                reader.Close();
                foreach(Query q in Queries)
                {
                    q.Load(dbConn);
                }
            }
        }

        /// <summary>
        /// 
        /// Note:
        /// max_connect_errors needs to be set.
        /// there is some kind of network socket stream error happening
        /// and mysql chocks it up as bad client and eventually
        /// blocks the client.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public override List<List<object>> CallQuery(String name, List<object> parms)
        {
            List<List<object>> results = new List<List<object>>();

            MySql.Data.MySqlClient.MySqlConnection dbConn = this.ConnectAsync() as MySql.Data.MySqlClient.MySqlConnection;

            if (dbConn != null && dbConn.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    String sql = "call " + name + "(";
                    for (int i = 0; i < parms.Count; i++)
                    {
                        if (Utilities.Util.isNumber(parms[i]))
                            sql += parms[i] + ",";
                        else if (parms[i] is String) // should be the last type
                            sql += "\'" + MySql.Data.MySqlClient.MySqlHelper.EscapeString((String)parms[i]) + "\'" + ",";
                    }
                    if (sql.EndsWith(","))
                        sql = sql.Substring(0, sql.Length - 1);
                    sql += ");";
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = sql;
                    var reader = cmd.ExecuteReader();
                    try
                    {
                        while (reader.HasRows && reader.Read())
                        {
                            List<object> row = new List<object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                                row.Add(reader[i]);
                            results.Add(row);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        if (reader != null)
                        {
                            try
                            {
                                reader.Close();
                            }
                            finally
                            {
                                reader.Dispose();
                            }
                        }
                    }
                }
                catch(Exception ex)
                {

                }
                finally
                {
                    if(dbConn != null)
                    {
                        try
                        {
                            dbConn.Close();
                        }
                        finally
                        {
                            dbConn.Dispose();
                        }
                    }
                }
                decrementOpenConnections();
            }

            return results;
        }
    }
}
