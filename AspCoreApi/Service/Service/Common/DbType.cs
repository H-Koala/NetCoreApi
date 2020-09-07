using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Services.Common
{
    public static class Db_Type
    {
        public enum DbName
        {
            [Description("Oracle")]
            Oracle = 0,
            [Description("MySql")]
            MySql = 1,
            [Description("MMSql")]
            MMSql = 2
        };
    }
}
