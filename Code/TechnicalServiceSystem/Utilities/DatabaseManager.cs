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

using System;
using NHibernate;
using System.Data;

namespace TechnicalServiceSystem.Utilities
{

    public class DatabaseManager : IDisposable
    {
        
        //TODO : make protected once everything switched to Nhibernate
        private ISession session = null;

        public DatabaseManager()
        {
            session = SessionHandler.TSS_Session;
        }
        public ISession GetSession()
        {
            return session;
        }

        protected IDbConnection GetConnection()
        {
            return session?.Connection;
        }

        public void Dispose()
        {
            SessionHandler.FreeSession(session);
            session = null;
        }
    }
}