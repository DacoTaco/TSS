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
        public static ISession TSS_Session => GetSession();

        private static ISessionFactory factory = null;

        public static ISessionFactory GetSessionFactory
        {
            get
            {
                ISessionFactory ret = null;
                {           
                    if(factory == null)
                        factory = CreateSessionFactory();
                    ret = factory;
                }
                return ret;
            }
        }
        private static ISessionFactory CreateSessionFactory()
        {
            var conSetting = ConfigurationManager.ConnectionStrings["TSSDatabase"];
            FluentConfiguration config = null;

            if (conSetting == null || String.IsNullOrWhiteSpace(conSetting.ProviderName))
                return null;

            switch (conSetting.ProviderName)
            {
                case "System.Data.SqlClient":
                    config = Fluently.Configure()
                        .Database(MsSqlConfiguration.MsSql2008.ConnectionString(c =>
                            c.FromConnectionStringWithKey("TSSDatabase")));
                    break;
                default:
                    break;
            }

            if (config == null)
                return null;

            if (Settings.IsWebEnvironment)
                config = config.CurrentSessionContext<WebSessionContext>();

            return config
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<IEntityMapper>())
                .BuildSessionFactory();
        }

        public static ISession GetSession()
        {
            if (Settings.IsWebEnvironment)
            {
                if (!CurrentSessionContext.HasBind(GetSessionFactory))
                {
                    CurrentSessionContext.Bind(GetSessionFactory.OpenSession());
                }

                return GetSessionFactory.GetCurrentSession();
            }
            else
            {
                return GetSessionFactory?.OpenSession();
            }
        }
        public static void UnbindSession()
        {
            if (!Settings.IsWebEnvironment)
                return;

            ISession session = CurrentSessionContext.Unbind(GetSessionFactory);

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
