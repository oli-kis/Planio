using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Planio.Models.Database;
using Planio.Models;
using System.Security.Claims;

namespace Planio.Services
{
    public class ClassService
    {
        private readonly IMongoCollection<ClassModel> _classCollection;

        private readonly IHttpContextAccessor _contextAccessor;

        public ClassService(IOptions<PlanioDBSettings> planioDBSettings, IHttpContextAccessor contextAccessor)
        {
            var mongoClient = new MongoClient(
                planioDBSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                planioDBSettings.Value.DatabaseName);

            _classCollection = mongoDatabase.GetCollection<ClassModel>(
                planioDBSettings.Value.ClassesCollectionName);

            _contextAccessor = contextAccessor;
        }

        public async Task<List<ClassModel>> GetAsync() =>
        await _classCollection.Find(_ => true).ToListAsync();

        public async Task<ClassModel?> GetSingle(string id)
        {
            return await _classCollection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();
        }

        public async Task<ClassModel?> GetWithClassName(string className) =>
         await _classCollection.Find(x => x.ClassName == className).FirstOrDefaultAsync();

        public async Task CreateAsync(ClassModel newClass) =>
            await _classCollection.InsertOneAsync(newClass);

        public async Task UpdateAsync(string id, ClassModel updatedClass) =>
            await _classCollection.ReplaceOneAsync(x => x.Id == id, updatedClass);

        public async Task RemoveAsync(string id) =>
            await _classCollection.DeleteOneAsync(x => x.Id == id);

        public async Task DeleteAll()
        {
            await _classCollection.DeleteManyAsync(x => x.Id != null);
        }
        public async Task AddStudentToClassAsync(StudentModel student, ClassModel updatedClass)
        {
            updatedClass.StudentIDs.Add(student.Id);
            await _classCollection.ReplaceOneAsync(x => x.Id == updatedClass.Id, updatedClass);
        }
    }
}
