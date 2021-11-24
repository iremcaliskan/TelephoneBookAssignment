using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using TelephoneBookAssignment.Mappings;
using TelephoneBookAssignment.Shared.DataAccess.MongoDb;

namespace TelephoneBookAssignment.UnitTests.TestSetup
{
    public class CommonTestFixture
    {
        public Mock<IOptions<MongoSettings>> MockOptions { get; }
        public Mock<IMongoDatabase> MockDb { get; }
        public Mock<IMongoClient> MockClient { get; }
        public IMapper Mapper { get; }

        public CommonTestFixture()
        {
            MockOptions = new Mock<IOptions<MongoSettings>>();
            MockDb = new Mock<IMongoDatabase>();
            MockClient = new Mock<IMongoClient>();
            Mapper = new MapperConfiguration(config => config.AddProfile(new MappingProfile())).CreateMapper();
        }
    }
}