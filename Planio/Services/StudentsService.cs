using MongoDB.Driver;
using System.Security.Claims;
using Planio.Models;
using Planio.Models.Database;
using Microsoft.Extensions.Options;


namespace Planio.Services
{
    public class StudentsService
    {
        private readonly IMongoCollection<StudentModel> _studentsCollection;

        private readonly IHttpContextAccessor _contextAccessor;

        public StudentsService(IOptions<PlanioDBSettings> planioDBSettings, IHttpContextAccessor contextAccessor)
        {
            var mongoClient = new MongoClient(
                planioDBSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                planioDBSettings.Value.DatabaseName);

            _studentsCollection = mongoDatabase.GetCollection<StudentModel>(
                planioDBSettings.Value.StudentsCollectionName);

            _contextAccessor = contextAccessor;
        }

        public async Task<List<StudentModel>> GetAsync() =>
        await _studentsCollection.Find(_ => true).ToListAsync();

        public async Task<StudentModel?> GetById()
        {
            string id = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();
            return await _studentsCollection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();
        }

        public async Task<StudentModel?> GetSingle(string id)
        {
            return await _studentsCollection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();
        }

        public async Task<StudentModel?> GetWithEmail(string email) =>
         await _studentsCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

        public async Task CreateAsync(StudentModel newStudent) =>
            await _studentsCollection.InsertOneAsync(newStudent);

        public async Task UpdateAsync(string id, StudentModel updatedStudent) =>
            await _studentsCollection.ReplaceOneAsync(x => x.Id == id, updatedStudent);

        public async Task RemoveAsync(string id) =>
            await _studentsCollection.DeleteOneAsync(x => x.Id == id);

        public async Task DeleteAll()
        {
            await _studentsCollection.DeleteManyAsync(x => x.Id != null);
        }
    }
}
