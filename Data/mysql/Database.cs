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
                    System.Object obj = reader[0];
                    ent.Name = obj.ToString();
                    Entities.Add(ent);
                }
                reader.Close();
                foreach (Entity ent in Entities)
                {
                    ent.Load(dbConn);
                    sql = "show index from `" + ent.Name + "`;";
                    cmd.CommandText = sql;
                    reader = cmd.ExecuteReader();
                    while (reader.HasRows && reader.Read())
                    {
                        String isUnique = reader[1].ToString();
                        if (isUnique == "0")
                        {
                            String col = reader[4].ToString();
                            if (!ent.UniqueColumns.Contains(col))
                            {
                                ent.UniqueColumns.Add(col);
                            }
                        }
                    }
                    reader.Close();
                }
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
        /// This function will replace string values for SQL statements to prevent SQL injection.
        /// </summary>
        /// <param name="args">Ordered array of params.</param>
        /// <returns>Status of the method's atempt to connect.</returns>
        protected virtual long popSQL(MySql.Data.MySqlClient.MySqlCommand cmd, Data.Object data)
        {
            long status = -1;
            if (data != null)
            {
                foreach (String prop in data.getPropertyNames())
                {
                    if (data.get(prop) != null)
                        cmd.Parameters.AddWithValue("@" + prop, data.get(prop));
                    else
                        cmd.Parameters.AddWithValue("@" + prop, "");
                }
            }
            return status;
        }

        public override int save(string tableName, Data.Object row)
        {
            int status = -1;
            MySql.Data.MySqlClient.MySqlConnection dbConn = this.ConnectAsync() as MySql.Data.MySqlClient.MySqlConnection;

            if (dbConn != null && dbConn.State == System.Data.ConnectionState.Open)
            {
                String sql = "";
                try
                {
                    sql = "INSERT INTO `" + tableName + "` (\n";
                    String cols = "";
                    String values = "";
                    String col_val = "";
                    List<String> props = new List<String>();
                    props.AddRange(row.getPropertyNames());
                    props.Sort();
                    Entity ent = null;
                    foreach (Entity e in this.Entities)
                    {
                        if (e.Name == tableName)
                        {
                            ent = e;
                            break;
                        }
                    }
                    foreach (String prop in props)
                    {
                        if (row.get(prop) != null)
                            cols += "`" + prop + "`, ";
                        if (row.get(prop) != null)
                            values += "@" + prop + ", ";
                        if(ent != null && !ent.UniqueColumns.Contains(prop))
                            col_val += "`" + prop + "`= @" + prop + ", ";
                    }
                    cols = cols.Substring(0, cols.Length - 2);
                    sql += cols + ") ";
                    sql += "\nValues(";
                    values = values.Substring(0, values.Length - 2);
                    sql += values + ") ";
                    sql += "\nON DUPLICATE KEY ";

                    sql += "\nUPDATE \n";
                    col_val = col_val.Substring(0, col_val.Length - 2);
                    sql += col_val + ";";

                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = sql;
                    popSQL(cmd, row);
                    status = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    if (m_DebugInfo)
                        Console.WriteLine("Error [{0}]: {1}", tableName, ex.Message);
                    status = -2;
                }
                finally
                {
                    if (dbConn != null)
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
            return status;
        }

        protected System.Data.DataTable parseResults(MySql.Data.MySqlClient.MySqlDataReader reader)
        {
            System.Data.DataTable results = new System.Data.DataTable();
            try
            {
                if(reader.HasRows)
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        System.Data.DataColumn col = new System.Data.DataColumn(reader.GetName(i), typeof(String));
                        results.Columns.Add(col);
                    }
                }
                while (reader.HasRows && reader.Read())
                {
                    System.Data.DataRow row = results.NewRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                        row[reader.GetName(i)] = reader[i];
                    results.Rows.Add(row);
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
            return results;
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
        public override System.Data.DataTable CallQuery(String name, List<object> parms)
        {
            int status = -1;
            System.Data.DataTable results = new System.Data.DataTable();

            MySql.Data.MySqlClient.MySqlConnection dbConn = this.ConnectAsync() as MySql.Data.MySqlClient.MySqlConnection;

            if (dbConn != null && dbConn.State == System.Data.ConnectionState.Open)
            {
                String sql = "";
                try
                {
                    sql = "call " + name + "(";
                    for (int i = 0; parms != null && i < parms.Count; i++)
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
                    results = parseResults(cmd.ExecuteReader());
                    status = 0;
                }
                catch(Exception ex)
                {
                    if (m_DebugInfo)
                        Console.WriteLine("Error [{0}]: {1}", name, ex.Message);
                    status = -2;
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

        protected String convertOperator(filtering.Operators op, String value = "")
        {
            String str = "";
            switch(op)
            {
                case filtering.Operators.AND:
                    str = " AND ";
                    break;
                case filtering.Operators.CONTAINS:
                    str = " LIKE '%" + value + "%' ";
                    break;
                case filtering.Operators.DOES_NOT_CONTAINS:
                    str = " NOT LIKE '%" + value + "%' ";
                    break;
                case filtering.Operators.DOES_NOT_ENDS_WITH:
                    str = " NOT LIKE '" + value + "%' ";
                    break;
                case filtering.Operators.DOES_NOT_STARTS_WITH:
                    str = " NOT LIKE '%" + value + "' ";
                    break;
                case filtering.Operators.ENDS_WITH:
                    str = " LIKE '" + value + "%' ";
                    break;
                case filtering.Operators.EQUALS:
                    str = " = " + value + " ";
                    break;
                case filtering.Operators.GREATER_THAN:
                    str = " > " + value + " ";
                    break;
                case filtering.Operators.GREATER_THAN_EQUAL:
                    str = " >= " + value + " ";
                    break;
                case filtering.Operators.LESS_THAN:
                    str = " < " + value + " ";
                    break;
                case filtering.Operators.LESS_THAN_EQUAL:
                    str = " <= " + value + " ";
                    break;
                case filtering.Operators.NOT_EQUAL:
                    str = " <> " + value + " ";
                    break;
                case filtering.Operators.OR:
                    str = " OR ";
                    break;
                case filtering.Operators.STARTS_WITH:
                    str = " LIKE '%" + value + "' ";
                    break;
            }
            return str;
        }

        protected String buildFilter(filtering.BaseFilter filter)
        {
            String sql = "";
            if(filter is filtering.Filter)
            {
                filtering.Filter f = filter as filtering.Filter;
                sql += "(" + buildFilter(f.RHS);
                sql += convertOperator(f.Operator);
                sql += buildFilter(f.LHS) + ")";
            }
            else if(filter is filtering.FilterExpression)
            {
                filtering.FilterExpression fe = filter as filtering.FilterExpression;
                sql += "(" + fe.ColumnName + convertOperator(fe.Operator, fe.Value) + ")";
            }
            return sql;
        }

        protected String buildSort(filtering.Sort sort)
        {
            String str = "";
            if(sort != null)
            {
                for(int i = 0; i < sort.ColumnNames.Count; i++)
                {
                    str += sort.ColumnNames[i] + ",";
                }
                if (str.Length > 0)
                    str = str.Substring(0, str.Length - 1);
                if (sort.Ascending)
                    str += " ASC ";
                else
                    str += " DESC ";
            }
            return str;
        }

        protected String buildPaging(filtering.Paging pg)
        {
            String str = "";
            if(pg != null)
            {
                str += " " + (pg.PageNumber * pg.CountPerPage) + "," + pg.CountPerPage + " ";
            }
            return str;
        }

        public override System.Data.DataTable getFiltered(String tableName,
                filtering.BaseFilter filter = null,
                filtering.Sort sort = null,
                filtering.Paging page = null)
        {
            int status = -1;
            System.Data.DataTable results = new System.Data.DataTable();

            MySql.Data.MySqlClient.MySqlConnection dbConn = this.ConnectAsync() as MySql.Data.MySqlClient.MySqlConnection;

            if (dbConn != null && dbConn.State == System.Data.ConnectionState.Open)
            {
                String sql = "";
                try
                {
                    sql = "Select * from `" + tableName + "` ";
                    // build filter
                    if(filter != null)
                        sql += " Where " + buildFilter(filter);
                    // build sort
                    if (sort != null)
                        sql += " ORDER BY " + buildSort(sort);
                    // build paging
                    if (page != null)
                        sql += " LIMIT " + buildPaging(page);
                    var cmd = dbConn.CreateCommand();
                    cmd.CommandText = sql;
                    results = parseResults(cmd.ExecuteReader());
                    status = 0;
                }
                catch (Exception ex)
                {
                    if (m_DebugInfo)
                        Console.WriteLine("Error [{0}]: {1}", tableName, ex.Message);
                    status = -2;
                }
                finally
                {
                    if (dbConn != null)
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
