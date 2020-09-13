/*TSS - Technical Service System , a system build to help Technical Services maintain their reports and equipment
Copyright(C) 2019 - Joris 'DacoTaco' Vermeylen

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

using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Context;
using System;
using System.Configuration;
using TechnicalServiceSystem.Mappings;

namespace TechnicalServiceSystem.Utilities
{
    public static class SessionHandler
    {
        public static ISession TSS_Session { get; private set; }
        private static ISessionFactory factory = null;

        static SessionHandler() 
        {
            TSS_Session = SessionFactory.OpenSession();
        }

        public static ISessionFactory SessionFactory
        {
            get
            {
                if (factory == null)
                    factory = CreateSessionFactory();

                return factory;
            }
        }

        private static ISessionFactory CreateSessionFactory()
        {
            var conSetting = ConfigurationManager.ConnectionStrings["TSSDatabase"];

            if (conSetting == null || String.IsNullOrWhiteSpace(conSetting.ProviderName) || string.IsNullOrWhiteSpace(conSetting.ConnectionString))
                return null;

            var connectionString = conSetting.ConnectionString;
            if(ConfigurationManager.AppSettings["ConnectionEncrypted"] == "1")
                connectionString = AESHandler.DecryptString(connectionString);


            IPersistenceConfigurer databaseConfig = null;
            switch (conSetting.ProviderName)
            {
                case "System.Data.SqlClient":
                    databaseConfig = MsSqlConfiguration.MsSql2008.ConnectionString(connectionString);
                    break;
                default:
                    break;
            }

            if (databaseConfig == null)
                return null;

            FluentConfiguration config = Fluently.Configure()
                .Database(databaseConfig)
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<IEntityMapper>());

            if (config == null)
                return null;

            if (Settings.IsWebEnvironment)
                config = config.CurrentSessionContext<WebSessionContext>();

            return config.BuildSessionFactory();
        }

        public static void BindSession()
        {
            if (!Settings.IsWebEnvironment)
                return;

            if (!CurrentSessionContext.HasBind(SessionFactory))
            {
                //TODO : once we use nhibernate completely like we should, remove this creation of a new session...
                TSS_Session = SessionFactory.OpenSession();
                CurrentSessionContext.Bind(TSS_Session);
            }
        }

        public static void UnbindSession()
        {
            if (!Settings.IsWebEnvironment)
                return;

            ISession session = CurrentSessionContext.Unbind(SessionFactory);

            if (session == null)
                return;

            if (session.IsOpen)
            {
                try
                {
                    if (session.Transaction != null && session.Transaction.IsActive)
                    {
                        session.Transaction.Rollback();
                    }
                }
                catch
                {
                    throw;
                }
                session.Close();
            }
            session.Dispose();
            return;
        }

        public static void FreeSession(ISession session)
        {
            if (session == null || Settings.IsWebEnvironment)
                return;

            if (session.IsOpen)
            {
                try
                {
                    if (session.Transaction != null && session.Transaction.IsActive)
                    {
                        session.Transaction.Rollback();
                    }
                }
                catch
                {
                    throw;
                }
                session.Close();
            }            
            session.Dispose();
        }
    }
}
