using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSql.Data.mysql
{
    public class Database : Data.Database
    {
        public Database():base()
        {

        }

        public Database(Data.Database db):base(db)
        {

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
                "Server={0};Database={1};Uid={2};Pwd={3};",
                ServerAddress,
                Name,
                UserName,
                Password);
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
    }
}
