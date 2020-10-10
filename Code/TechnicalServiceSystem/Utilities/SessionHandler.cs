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
    public class SessionHandler
    {
        private static ISession Session { get; set; } = null;
        private static ISessionFactory _factory = null;
        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_factory == null)
                    _factory = CreateSessionFactory();

                return _factory;
            }
        }

        private static ISessionFactory CreateSessionFactory()
        {
            var conSetting = ConfigurationManager.ConnectionStrings["TSSDatabase"];

            if (conSetting == null || String.IsNullOrWhiteSpace(conSetting.ProviderName) || string.IsNullOrWhiteSpace(conSetting.ConnectionString))
                return null;

            var connectionString = conSetting.ConnectionString;
            if(Settings.ConnectionEncrypted)
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

        private ISession OpenSession() => SessionFactory.OpenSession();

        public void BindSession()
        {
            if (CurrentSessionContext.HasBind(SessionFactory))
                return;

            CurrentSessionContext.Bind(OpenSession());
        }
        public void CloseSession()
        {
            ISession session = Session;
            if (Settings.IsWebEnvironment)
            {
                if (CurrentSessionContext.HasBind(SessionFactory))
                    session = CurrentSessionContext.Unbind(SessionFactory);
            }

            if (session?.Transaction?.IsActive ?? false)
                session.Transaction.Rollback();
            session?.Dispose();
        }

        public ISession GetCurrentSession()
        {
            if (Settings.IsWebEnvironment)
            {
                if (!CurrentSessionContext.HasBind(SessionFactory))
                    CurrentSessionContext.Bind(OpenSession());

                return SessionFactory.GetCurrentSession();
            } 
            else
            {
                if (Session == null)
                    Session = OpenSession();

                return Session;
            }
        }
    }
}
