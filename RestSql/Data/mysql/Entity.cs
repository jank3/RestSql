using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSql.Data.mysql
{
    public class Entity : Data.Entity
    {
        public override void Load(DbConnection dbConn)
        {
            base.Load(dbConn);

            System.Data.DataTable schema = dbConn.GetSchema("Tables",
                new String[]
                {
                    null, // database
                    null, // owner
                    Name // table name
                });
            this.Description = schema.Columns["TABLE_COMMENT"].ToString();
        }
    }
}
