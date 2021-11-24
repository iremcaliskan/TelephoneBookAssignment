using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using TelephoneBookAssignment.Application.ContactOperations.Commands.CreateContact;
using TelephoneBookAssignment.Shared.DataAccess.MongoDb;
using TelephoneBookAssignment.Shared.Entities;
using TelephoneBookAssignment.UnitTests.TestSetup;
using Xunit;

namespace TelephoneBookAssignment.UnitTests.ContactOperations.CreateContact
{
    public class CreateContactCommandTest : IClassFixture<CommonTestFixture>
    {
        private readonly Mock<IOptions<MongoSettings>> _mockOptions;
        private readonly Mock<IMongoDatabase> _mockDb;
        private readonly Mock<IMongoClient> _mockClient;
        private readonly IMapper _mapper;

        private readonly MongoSettings settings = new()
        {
            ConnectionString = "mongodb+srv://iremcaliskan:Password123@testcluster.avg7k.mongodb.net/myFirstDatabase?retryWrites=true&w=majority",
            DatabaseName = "TelephoneBook"
        };

        public CreateContactCommandTest(CommonTestFixture fixture)
        {
            _mockOptions = fixture.MockOptions;
            _mockDb = fixture.MockDb;
            _mockClient = fixture.MockClient;
            _mapper = fixture.Mapper;
        }

        [Fact]
        public async Task WhenAlreadyExistPhoneNumberIsGiven_InvalidOperationException_ShouldBeReturn()
        {
            // Arrange
            var contact = new Contact()
            {
                Name = "İrem",
                LastName = "Çalışkan",
                Firm = "Xtanbul",
                Information = new Contact.ContactInformation()
                {
                    PhoneNumber = "12345678912",
                    Location = "İstanbul",
                    Mail = "iremcaliskan0@gmail.com"
                }
            };
            
            _mockOptions.Setup(x => x.Value).Returns(settings);
            _mockClient.Setup(x => x.GetDatabase(_mockOptions.Object.Value.DatabaseName, null)).Returns(_mockDb.Object);
            
            var context = new MongoTelephoneBookDbContext(_mockOptions.Object);
            var contactCollection = context.GetCollection<Contact>(nameof(Contact)); 
            
            CreateContactCommand command = new(contactCollection, _mapper);
            command.Model = new CreateContactModel()
            {
                Name = contact.Name,
                LastName = contact.LastName,
                Firm = contact.Firm,
                Information = new Contact.ContactInformation()
                {
                    PhoneNumber = contact.Information.PhoneNumber,
                    Location = contact.Information.Location,
                    Mail = contact.Information.Mail
                }
            };

            // Act & Assert
            await FluentActions
                .Invoking(() => command.Handle())
                .Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact] // Happy Path
        public async Task WhenValidInputsAreGiven_Contact_ShouldBeCreated()
        {
            // Arrange
            var contact = new Contact()
            {
                Name = "İrem",
                LastName = "Çalışkan",
                Firm = "Xtanbul",
                Information = new Contact.ContactInformation()
                {
                    PhoneNumber = "00000000000",
                    Location = "İstanbul",
                    Mail = "iremcaliskan0@gmail.com"
                }
            };

            _mockOptions.Setup(x => x.Value).Returns(settings);
            _mockClient.Setup(x => x.GetDatabase(_mockOptions.Object.Value.DatabaseName, null)).Returns(_mockDb.Object);

            var context = new MongoTelephoneBookDbContext(_mockOptions.Object);
            var contactCollection = context.GetCollection<Contact>(nameof(Contact));

            CreateContactCommand command = new(contactCollection, _mapper);
            command.Model = new CreateContactModel()
            {
                Name = contact.Name,
                LastName = contact.LastName,
                Firm = contact.Firm,
                Information = new Contact.ContactInformation()
                {
                    PhoneNumber = contact.Information.PhoneNumber,
                    Location = contact.Information.Location,
                    Mail = contact.Information.Mail
                }
            };

            // Act
            await FluentActions.Invoking(() => command.Handle()).Invoke();

            // Assert
            var addedContact = await contactCollection
                .Find(x => x.Information.PhoneNumber == command.Model.Information.PhoneNumber).FirstOrDefaultAsync();
            addedContact.Should().NotBeNull();
            addedContact.Firm.Should().Be(command.Model.Firm);
            addedContact.Information.Location.Should().Be(command.Model.Information.Location);
        }
    }
}