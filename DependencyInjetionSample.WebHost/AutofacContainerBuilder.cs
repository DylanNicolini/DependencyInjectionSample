// --------------------------------------------------------------------------------------------------------------------
//
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/. 
// ----------------------------------------------------------------------------------------

using System.Collections.Generic;
using Autofac;
using DependencyInjectionSample.Business;
using DependencyInjectionSample.BusinessContracts;
using DependencyInjectionSample.Data;
using DependencyInjectionSample.Implementation;
using DependencyInjectionSample.OperationContracts;

namespace DependencyInjetionSample.WebHost
{
    public static class AutofacContainerBuilder
    {
        /// <summary>
        /// Used to build the list of dependencies to run the services
        /// </summary>
        /// <returns></returns>
        public static IContainer Build()
        {
            var test = typeof (EmployeeService).AssemblyQualifiedName;

            var builder = new ContainerBuilder();

            builder.RegisterType<EmployeeRepository>()
                .As<IEmployeeRepository>();

            // Create the list of validation strategies to be used as business rules
            var  employeeValidationStrategies = new List<IEmployeeValidationStrategy>
            {
                new EmployeeFirstNameValdidationStrategy(),
                new EmployeeLastNameValidationStrategy(),
                new EmployeeSalaryValidationStrategy()
            };

            builder.RegisterInstance(employeeValidationStrategies)
                .As<List<IEmployeeValidationStrategy>>();
            
            builder.RegisterType<EmployeeBusinessService>()
                .As<IEmployeeBusinessService>();

            builder.RegisterType<EmployeeService>()
                .As<IEmployeeService>()
                .InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}