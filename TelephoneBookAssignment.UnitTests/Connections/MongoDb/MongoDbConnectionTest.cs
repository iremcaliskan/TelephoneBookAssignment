using FluentAssertions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using TelephoneBookAssignment.Shared.DataAccess.MongoDb;
using TelephoneBookAssignment.Shared.Entities;
using TelephoneBookAssignment.UnitTests.TestSetup;
using Xunit;

namespace TelephoneBookAssignment.UnitTests.Connections.MongoDb
{
    public class MongoDbConnectionTest : IClassFixture<CommonTestFixture>
    {
        private readonly Mock<IOptions<MongoSettings>> _mockOptions;
        private readonly Mock<IMongoDatabase> _mockDb;
        private readonly Mock<IMongoClient> _mockClient;

        private MongoSettings settings = new()
        {
            ConnectionString = "mongodb+srv://iremcaliskan:Password123@testcluster.avg7k.mongodb.net/myFirstDatabase?retryWrites=true&w=majority",
            DatabaseName = "TelephoneBook"
        };

        public MongoDbConnectionTest(CommonTestFixture fixture)
        {
            _mockOptions = fixture.MockOptions;
            _mockDb = fixture.MockDb;
            _mockClient = fixture.MockClient;
        }

        [Fact]
        public void MongoDbContext_Connection_Success()
        {
            // Arrange
            _mockOptions.Setup(x => x.Value).Returns(settings);
            _mockClient.Setup(x => x.GetDatabase(_mockOptions.Object.Value.DatabaseName, null)).Returns(_mockDb.Object);

            // Act
            var context = new MongoTelephoneBookDbContext(_mockOptions.Object);

            // Assert
            Assert.NotNull(context);
        }

        [Fact]
        public void MongoDbContext_GetCollection_ValidName_Success()
        {
            // Arrange
            _mockOptions.Setup(x => x.Value).Returns(settings);
            _mockClient.Setup(x => x.GetDatabase(_mockOptions.Object.Value.DatabaseName, null)).Returns(_mockDb.Object);

            // Act
            var context = new MongoTelephoneBookDbContext(_mockOptions.Object);
            var myCollection = context.GetCollection<Contact>(nameof(Contact));
            
            // Assert
            Assert.NotNull(myCollection);
        }

        [Fact]
        public void MongoDbContext_GetCollection_InvalidName_Failure()
        {
            // Arrange
            _mockOptions.Setup(x => x.Value).Returns(settings);
            _mockClient.Setup(x => x.GetDatabase(_mockOptions.Object.Value.DatabaseName, null)).Returns(_mockDb.Object);
            
            var context = new MongoTelephoneBookDbContext(_mockOptions.Object);

            // Act and Assert
            FluentActions
                .Invoking(() => context.GetCollection<Contact>(string.Empty))
                .Should().Throw<System.ArgumentException>();
        }
    }
}