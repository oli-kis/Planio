using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Planio.Models.Database;
using Planio.Models;
using System.Security.Claims;

namespace Planio.Services
{
    public class TeachersService
    {
        private readonly IMongoCollection<TeacherModel> _teachersCollection;

        private readonly IHttpContextAccessor _contextAccessor;

        public TeachersService(IOptions<PlanioDBSettings> planioDBSettings, IHttpContextAccessor contextAccessor)
        {
            var mongoClient = new MongoClient(
                planioDBSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                planioDBSettings.Value.DatabaseName);

            _teachersCollection = mongoDatabase.GetCollection<TeacherModel>(
                planioDBSettings.Value.TeachersCollectionName);

            _contextAccessor = contextAccessor;
        }

        public async Task<List<TeacherModel>> GetAsync() =>
        await _teachersCollection.Find(_ => true).ToListAsync();

        public async Task<TeacherModel?> GetById()
        {
            string id = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();
            return await _teachersCollection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();
        }

        public async Task<TeacherModel?> GetSingle(string id)
        {
            return await _teachersCollection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();
        }

        public async Task<TeacherModel?> GetWithEmail(string email) =>
         await _teachersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

        public async Task CreateAsync(TeacherModel newTeacher) =>
            await _teachersCollection.InsertOneAsync(newTeacher);

        public async Task UpdateAsync(string id, TeacherModel updatedTeacher) =>
            await _teachersCollection.ReplaceOneAsync(x => x.Id == id, updatedTeacher);

        public async Task RemoveAsync(string id) =>
            await _teachersCollection.DeleteOneAsync(x => x.Id == id);

        public async Task DeleteAll()
        {
            await _teachersCollection.DeleteManyAsync(x => x.Id != null);
        }
    }
}
