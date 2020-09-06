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

using System.ComponentModel;

namespace TechnicalServiceSystem.Lists
{
    //a singleton so we can keep the lists and use them all over the application. not caring about instances, reloading the list etc etc.
    //i do love myself a singleton :P
    //this is a partial class so we can split it up depending on what part of the system we are working on   
    public partial class SystemLists
    {
        //----------------------
        //new implementation
        //----------------------
        public static UserList User = new UserList();
        public static GeneralLists General = new GeneralLists();
        public static TaskList Tasks = new TaskList();
        public static SupplierLists Supplier = new SupplierLists();
    }
}