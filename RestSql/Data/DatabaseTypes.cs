using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSql.Data
{
    public class DatabaseTypes
    {
        public enum DB_TYPES
        {
            MySQL,
            UNKNOWN
        }

        public static Type convert(DB_TYPES type)
        {
            Type t = null;
            switch (type)
            {
                case DB_TYPES.MySQL:
                    t = typeof(MySql.Data.MySqlClient.MySqlConnection);
                    break;
            }
            return t;
        }

        public static DB_TYPES convert(String type)
        {
            DB_TYPES dbType = (DB_TYPES)Enum.Parse(typeof(DB_TYPES), type);
            return dbType;
        }

        public static Data.Database convert(object database, Data.DatabaseTypes.DB_TYPES type)
        {
            Data.Database dbRet = null;
            if (database is Data.Database)
            {
                Data.Database db = database as Data.Database;
                dbRet = db;
                switch(type)
                {
                    case DB_TYPES.MySQL:
                        dbRet = new mysql.Database(db);
                        break;
                }
                dbRet.DatabaseType = type;
            }
            return dbRet;
        }
    }
}
