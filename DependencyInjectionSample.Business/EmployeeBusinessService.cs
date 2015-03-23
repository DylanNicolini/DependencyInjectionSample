
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

using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjectionSample.BusinessContracts;
using DependencyInjectionSample.PortableDataContracts;

namespace DependencyInjectionSample.Business
{
    public class EmployeeBusinessService : IEmployeeBusinessService
    {
        private readonly IEmployeeRepository _employeeRepository;
        public readonly List<IEmployeeValidationStrategy> EmployeeValidationStrategies;
 
        public EmployeeBusinessService(IEmployeeRepository employeeRepository, List<IEmployeeValidationStrategy> employeeValidationStrategies )
        {
            if (employeeRepository == null)
                throw new ArgumentNullException(paramName: "employeeRepository");

            _employeeRepository = employeeRepository;

            if (employeeValidationStrategies == null)
                throw new ArgumentNullException("employeeValidationStrategies");

            EmployeeValidationStrategies = employeeValidationStrategies;

            // Refactored to use DI
            // Build validation strategies
            // Should be refactored to inject the strategies for validation
            //EmployeeValidationStrategies = new List<IEmployeeValidationStrategy>
            //{
            //    new EmployeeFirstNameValdidationStrategy(),
            //    new EmployeeLastNameValidationStrateg(),
            //    new EmployeeSalaryValidationStrategy()
            //};
        }

        public bool CreateEmployee(Employee employee)
        {
            // First we validate the business rules
            // Then we hit the data layer to create the object
            if (Validate(employee))
                return _employeeRepository.Create(employee);

            return false;
        }

        public List<Employee> GetAllEmployees()
        {
            return _employeeRepository.GetAllEmployees();
        }

        public bool Validate(Employee employee)
        {
            // If any of the validation strategies fail...the object is invalid
            return EmployeeValidationStrategies.All(x => x.IsValid(employee));
        }
    }
}
