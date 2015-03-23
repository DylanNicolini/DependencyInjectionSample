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
using DependencyInjectionSample.BusinessContracts;
using DependencyInjectionSample.Implementation;
using DependencyInjectionSample.PortableDataContracts;
using Moq;
using Xunit;

namespace DependencyInjectionSampleTests
{
    public class EmployeeServiceTests
    {
        [Fact]
        public void Construct_EmployeeService_WhenBusinessServiceIsNull_ThrowsException()
        {
            // Arrange & Act
            Exception ex = Assert.Throws<ArgumentNullException>
                (() => new EmployeeService(null));

            // Assert
            Assert.Equal("Value cannot be null.\r\nParameter name: employeeBusinessService", ex.Message);
        }

        [Fact]
        public void CreateEmployee_WhenEmployeeIsNull_ReturnsFalse()
        {
            // Arrange
            var mockedEmployeeBusinessService = new Mock<IEmployeeBusinessService>();
            var employeeService = new EmployeeService(mockedEmployeeBusinessService.Object);

            // Act
            var result = employeeService.CreateEmployee(null);

            // Assert
            Assert.Equal(false, result);
        }

        [Fact]
        public void CreateEmployee_WhenEmployeeIsNull_ReturnsTrue()
        {
            // Arrange
            var mockedEmployeeBusinessService = new Mock<IEmployeeBusinessService>();

            var employeeService = new EmployeeService(mockedEmployeeBusinessService.Object);
            var employee = new Employee { EmployeeNumber = "1", FirstName = "Test", LastName = "Case", Salary = 1111111 };

            // Stage the return result so that we know it was called correctly
            mockedEmployeeBusinessService.Setup(m => m.CreateEmployee(employee)).Returns(true);

            // Act
            var result = employeeService.CreateEmployee(employee);

            // Assert
            // Asserting for the true condition in this case is to verify that all validation was successfull
            // and that the mocked layer was called appropriately.
            // This will only be called once
            mockedEmployeeBusinessService.Verify(m => m.CreateEmployee(employee), Times.Exactly(1));
            Assert.Equal(true, result);
        }
    }
}
