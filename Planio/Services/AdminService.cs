using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Planio.Models.Database;
using Planio.Models;
using System.Security.Claims;

namespace Planio.Services
{
    public class AdminService
    {
        private readonly IMongoCollection<Administrators> _administratorsCollection;

        private readonly IHttpContextAccessor _contextAccessor;

        public AdminService(IOptions<PlanioDBSettings> planioDBSettings, IHttpContextAccessor contextAccessor)
        {
            var mongoClient = new MongoClient(
                planioDBSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                planioDBSettings.Value.DatabaseName);

            _administratorsCollection = mongoDatabase.GetCollection<Administrators>(
                planioDBSettings.Value.AdminsCollectionName);

            _contextAccessor = contextAccessor;
        }

        public async Task<List<Administrators>> GetAsync() =>
        await _administratorsCollection.Find(_ => true).ToListAsync();

        public async Task<Administrators?> GetById()
        {
            string id = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();
            return await _administratorsCollection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();
        }

        public async Task<Administrators?> GetSingle(string id)
        {
            return await _administratorsCollection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();
        }

        public async Task<Administrators?> GetWithEmail(string email) =>
         await _administratorsCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

        public async Task CreateAsync(Administrators newAdmin) =>
            await _administratorsCollection.InsertOneAsync(newAdmin);
    }
}
