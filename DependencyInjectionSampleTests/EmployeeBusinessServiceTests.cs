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
using DependencyInjectionSample.Business;
using DependencyInjectionSample.BusinessContracts;
using DependencyInjectionSample.PortableDataContracts;
using Moq;
using Xunit;

namespace DependencyInjectionSampleTests
{
    public class EmployeeBusinessServiceTests
    {
        [Fact]
        public void Construct_EmployeeBusinessService_WhenRepositoryIsNull_ThrowsException()
        {
            // Arrange & Act
            var employeeValidationStrategies = new List<IEmployeeValidationStrategy>();

            Exception ex = Assert.Throws<ArgumentNullException>
                (() => new EmployeeBusinessService(null, employeeValidationStrategies));

            // Assert
            Assert.Equal("Value cannot be null.\r\nParameter name: employeeRepository", ex.Message);
        }

        [Fact]
        public void Construct_EmployeeBusinessService_WhenValidationStrategiesIsNull_ThrowsException()
        {
            // Arrange & Act
            var employeeRepository = new Mock<IEmployeeRepository>();

            Exception ex = Assert.Throws<ArgumentNullException>
                (() => new EmployeeBusinessService(employeeRepository.Object, null));

            // Assert
            Assert.Equal("Value cannot be null.\r\nParameter name: employeeValidationStrategies", ex.Message);
        }

        [Fact]
        public void ValidateEmployee_WhenFirstNameIsNull_ReturnsFalse()
        {
            // Arrange
            var employeeRepository = new Mock<IEmployeeRepository>();

            var employeeValidationStrategies = new List<IEmployeeValidationStrategy>();
            var firstNameValidator = new EmployeeFirstNameValdidationStrategy();
            employeeValidationStrategies.Add(firstNameValidator);

            var employeeBusinessService = new EmployeeBusinessService(employeeRepository.Object,
                employeeValidationStrategies);

            var employee = new Employee {FirstName = null};

            // Act
            var result = employeeBusinessService.Validate(employee);

            // Assert
            Assert.Equal(false, result);
        }

        [Fact]
        public void ValidateEmployee_WhenFirstNameIsValid_ReturnsTrue()
        {
            // Arrange
            var employeeRepository = new Mock<IEmployeeRepository>();

            var employeeValidationStrategies = new List<IEmployeeValidationStrategy>();
            var firstNameValidator = new EmployeeFirstNameValdidationStrategy();
            employeeValidationStrategies.Add(firstNameValidator);

            var employeeBusinessService = new EmployeeBusinessService(employeeRepository.Object,
                employeeValidationStrategies);

            var employee = new Employee { FirstName = "Test" };

            // Act
            var result = employeeBusinessService.Validate(employee);

            // Assert
            Assert.Equal(true, result);
        }

        [Fact]
        public void ValidateEmployee_WhenLastNameIsNull_ReturnsFalse()
        {
            // Arrange
            var employeeRepository = new Mock<IEmployeeRepository>();

            var employeeValidationStrategies = new List<IEmployeeValidationStrategy>();
            var lastNameValidator = new EmployeeLastNameValidationStrategy();
            employeeValidationStrategies.Add(lastNameValidator);

            var employeeBusinessService = new EmployeeBusinessService(employeeRepository.Object,
                employeeValidationStrategies);

            var employee = new Employee { LastName = null};

            // Act
            var result = employeeBusinessService.Validate(employee);

            // Assert
            Assert.Equal(false, result);
        }

        [Fact]
        public void ValidateEmployee_WhenLastNameIsValid_ReturnsTrue()
        {
            // Arrange
            var employeeRepository = new Mock<IEmployeeRepository>();

            var employeeValidationStrategies = new List<IEmployeeValidationStrategy>();
            var lastNameValidator = new EmployeeLastNameValidationStrategy();
            employeeValidationStrategies.Add(lastNameValidator);

            var employeeBusinessService = new EmployeeBusinessService(employeeRepository.Object,
                employeeValidationStrategies);

            var employee = new Employee { LastName = "Test" };

            // Act
            var result = employeeBusinessService.Validate(employee);

            // Assert
            Assert.Equal(true, result);
        }
        [Fact]
        public void ValidateEmployee_SalaryIsZero_ReturnsFalse()
        {
            // Arrange
            var employeeRepository = new Mock<IEmployeeRepository>();

            var employeeValidationStrategies = new List<IEmployeeValidationStrategy>();
            var salaryValidator = new EmployeeSalaryValidationStrategy();
            employeeValidationStrategies.Add(salaryValidator);

            var employeeBusinessService = new EmployeeBusinessService(employeeRepository.Object,
                employeeValidationStrategies);

            var employee = new Employee { Salary = 0};

            // Act
            var result = employeeBusinessService.Validate(employee);

            // Assert
            Assert.Equal(false, result);
        }

        [Fact]
        public void ValidateEmployee_SalaryIsNotZero_ReturnsTrue()
        {
            // Arrange
            var employeeRepository = new Mock<IEmployeeRepository>();

            var employeeValidationStrategies = new List<IEmployeeValidationStrategy>();
            var salaryValidator = new EmployeeSalaryValidationStrategy();
            employeeValidationStrategies.Add(salaryValidator);

            var employeeBusinessService = new EmployeeBusinessService(employeeRepository.Object,
                employeeValidationStrategies);

            var employee = new Employee { Salary = 1000000 };

            // Act
            var result = employeeBusinessService.Validate(employee);

            // Assert
            Assert.Equal(true, result);
        }

        /// <summary>
        /// Mocking the repository (data layer) and the validation strategy to default to false
        /// we can test the behavior of the create employee method and validate that
        /// given any validation strategy failure the data layer persist method is not called
        /// </summary>
        [Fact]
        public void CreateEmployee_WhenEmployeeIsNotValid_ReturnsFalse()
        {
            // Arrange
            var employeeRepository = new Mock<IEmployeeRepository>();

            var employeeValidationStrategies = new List<IEmployeeValidationStrategy>();
            var employeeValidationStrategy = new Mock<IEmployeeValidationStrategy>();
            employeeValidationStrategy.Setup(x => x.IsValid(null)).Returns(false);
            employeeValidationStrategies.Add(employeeValidationStrategy.Object);

            var employeeBusinessService = new EmployeeBusinessService(employeeRepository.Object, employeeValidationStrategies);
            var employee = new Employee();

            // Act
            var result = employeeBusinessService.CreateEmployee(employee);

            // Assert
            Assert.Equal(false, result);
            employeeValidationStrategy.Verify(x => x.IsValid(employee), Times.Once);
            employeeRepository.Verify(x => x.Create(employee), Times.Never);
        }

        /// <summary>
        /// Default the validation strategy to always be True
        ///  we can validate that the behavior given positive validation, that the data layer's create method will be called.
        /// This is run independent of any validation business rules.
        /// </summary>
        [Fact]
        public void CreateEmployee_WhenEmployeeIsValid_ReturnsTrue()
        {
            // Arrange
            var employeeRepository = new Mock<IEmployeeRepository>();

            var employeeValidationStrategies = new List<IEmployeeValidationStrategy>();
            var employeeValidationStrategy = new Mock<IEmployeeValidationStrategy>();
            var employee = new Employee();
            
            employeeValidationStrategy.Setup(x => x.IsValid(employee)).Returns(true);
            employeeValidationStrategies.Add(employeeValidationStrategy.Object);

            var employeeBusinessService = new EmployeeBusinessService(employeeRepository.Object, employeeValidationStrategies);

            // Act
            var result = employeeBusinessService.CreateEmployee(employee);

            // Assert
            Assert.Equal(false, result);
            employeeValidationStrategy.Verify(x => x.IsValid(employee), Times.Once);
            employeeRepository.Verify(x => x.Create(employee),Times.Exactly(1));
        }
    }
}
