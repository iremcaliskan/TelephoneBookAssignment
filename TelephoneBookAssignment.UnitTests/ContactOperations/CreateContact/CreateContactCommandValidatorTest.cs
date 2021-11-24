using FluentAssertions;
using System.Threading.Tasks;
using TelephoneBookAssignment.Application.ContactOperations.Commands.CreateContact;
using TelephoneBookAssignment.Shared.Entities;
using Xunit;

namespace TelephoneBookAssignment.UnitTests.ContactOperations.CreateContact
{
    public class CreateContactCommandValidatorTest
    {
        [Fact]
        public async Task Fact_WhenInvalidInputsAreGiven_Validator_ShouldBeReturnErrors()
        {
            // Arrange
            CreateContactCommand command = new(null, null);
            command.Model = new CreateContactModel()
            {
                Name = "",
                LastName = "",
                Firm = "",
                Information = new Contact.ContactInformation()
                {
                    PhoneNumber = "",
                    Location = "",
                    Mail = ""
                }
            };

            // Act (Çalıştırma)
            CreateContactCommandValidator validator = new();
            var validationResult = await validator.ValidateAsync(command);

            // Assert (Doğrulama)
            validationResult.Errors.Count.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData("", "", "", "", "", "")]
        [InlineData("İrem", "", "", "", "", "")]
        [InlineData("İrem", "Çalışkan", "", "", "", "")]
        [InlineData("İrem", "Çalışkan", "Xtanbul", "", "", "")]
        [InlineData("İrem", "Çalışkan", "Xtanbul", "12345678910", "", "")]
        [InlineData("İrem", "Çalışkan", "Xtanbul", "12345678910", "iremcaliskan0@gmail.com", "")]
        //[InlineData("İrem", "Çalışkan", "Xtanbul", "12345678910", "iremcaliskan0@gmail.com", "İstanbul")] - passed
        public async Task Theory_WhenInvalidInputsAreGiven_Validator_ShouldBeReturnErrors(string name, string lastName, string firm, string phoneNumber, string mail, string location)
        {
            // Arrange
            CreateContactCommand command = new(null, null);
            command.Model = new CreateContactModel() { Name = name, LastName = lastName, Firm = firm, Information = new Contact.ContactInformation() { PhoneNumber = phoneNumber, Mail = mail, Location = location } };

            // Act
            CreateContactCommandValidator validator = new();
            var validationResult = await validator.ValidateAsync(command);

            // Assert
            validationResult.Errors.Count.Should().BeGreaterThan(0);
        }
    }
}