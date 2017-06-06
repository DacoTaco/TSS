/*TSS - Technical Service System , a system build to help Technical Services maintain their reports and equipment
Copyright(C) 2017 - Joris 'DacoTaco' Vermeylen

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see http://www.gnu.org/licenses */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace TechnicalServiceSystem.Base
{
    /// <summary>
    /// Base class of all the manangers. all functions that have to do with the database should go here.
    /// </summary>
    public class DatabaseManager
    {
        /// <summary>
        /// enum containing all supported Database types!
        /// </summary>
        public enum DatabaseTypes
        {
            SQL = 1,
            MYSQL, ORACLE, OLEDB, ODBC
        };

        protected static DbProviderFactory factory = null;
        protected string ConnectionString = "";

        /// <summary>
        /// Get the connection
        /// </summary>
        /// <returns></returns>
        public DbConnection GetConnection()
        {
            if (factory == null || String.IsNullOrEmpty(ConnectionString))
                return null;

            var con = factory.CreateConnection();
            con.ConnectionString = ConnectionString;
            return con;
        }

        /// <summary>
        /// Standard constructor that uses the default con string and type. it'll look for TSSDatabase con string & ProviderName in the application settings
        /// </summary>
        public DatabaseManager()
        {
            ConnectionStringSettings conSetting = ConfigurationManager.ConnectionStrings["TSSDatabase"];
            ConnectionString = conSetting.ConnectionString;
            factory = DbProviderFactories.GetFactory(conSetting.ProviderName);
        }

        /// <summary>
        /// Overload constructor incase we want to connect to somewhere not default.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="providerName"></param>
        public DatabaseManager(string connectionString, string providerName)
        {
            switch (providerName)
            {
                case "MySql.Data.MySqlClient":
                case "System.Data.SqlClient":
                case "System.Data.OracleClient":
                case "System.Data.Odbc":
                case "System.Data.OleDb":
                    factory = DbProviderFactories.GetFactory(providerName);
                    break;
                default:
                    throw new ArgumentException("Database_Failed_Init : Unknown provider name given!");
                    //we will never reach this break anyway xD so to silence the warning...
                    //break;
            }
            ConnectionString = connectionString;
        }
        /// <summary>
        /// Overload constructor incase we wanna pass on our own connection info
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="DatabaseType"></param>
        protected DatabaseManager(string connectionString, DatabaseTypes DatabaseType)
        {
            string Provider = "";
            switch (DatabaseType)
            {
                case DatabaseTypes.MYSQL:
                    Provider = "MySql.Data.MySqlClient";
                    break;
                case DatabaseTypes.SQL:
                    Provider = "System.Data.SqlClient";
                    break;
                case DatabaseTypes.ORACLE:
                    Provider = "System.Data.OracleClient";
                    break;
                case DatabaseTypes.ODBC:
                    Provider = "System.Data.Odbc";
                    break;
                case DatabaseTypes.OLEDB:
                    Provider = "System.Data.OleDb";
                    break;
                default:
                    break;
            }

            factory = DbProviderFactories.GetFactory(Provider);
            ConnectionString = connectionString;
        }
    }
}
