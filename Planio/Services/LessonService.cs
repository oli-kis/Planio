using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Planio.Models.Database;
using Planio.Models;

namespace Planio.Services
{
    public class LessonService
    {
        private readonly IMongoCollection<LessonModel> _lessonCollection;
        private readonly IMongoCollection<ClassModel> _classCollection;
        private readonly IMongoCollection<TeacherModel> _teacherCollection;

        private readonly IHttpContextAccessor _contextAccessor;

        public LessonService(IOptions<PlanioDBSettings> planioDBSettings, IHttpContextAccessor contextAccessor)
        {
            var mongoClient = new MongoClient(
                planioDBSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                planioDBSettings.Value.DatabaseName);

            _lessonCollection = mongoDatabase.GetCollection<LessonModel>(
                planioDBSettings.Value.LessonsCollectionName);
            _classCollection = mongoDatabase.GetCollection<ClassModel>(
                planioDBSettings.Value.ClassesCollectionName);
            _teacherCollection = mongoDatabase.GetCollection<TeacherModel>(
                planioDBSettings.Value.TeachersCollectionName);

            _contextAccessor = contextAccessor;
        }

        public async Task<List<LessonModel>> GetAsync() =>
        await _lessonCollection.Find(_ => true).ToListAsync();

        public async Task<LessonModel?> GetSingle(string id)
        {
            return await _lessonCollection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(LessonModel newLesson) =>
            await _lessonCollection.InsertOneAsync(newLesson);

        public async Task UpdateAsync(string id, LessonModel updatedLesson) =>
            await _lessonCollection.ReplaceOneAsync(x => x.Id == id, updatedLesson);

        public async Task RemoveAsync(string id) =>
            await _lessonCollection.DeleteOneAsync(x => x.Id == id);

        public async Task DeleteAll()
        {
            await _lessonCollection.DeleteManyAsync(x => x.Id != null);
        }
        public async Task AddLessonToClass(LessonModel lesson, ClassModel classToAddTo)
        {
            classToAddTo.LessonIDs.Add(lesson.Id);
            await _classCollection.ReplaceOneAsync(x => x.Id == classToAddTo.Id, classToAddTo);
        }
        public async Task AddLessonToTeacher(LessonModel lesson, TeacherModel teacherToAddTo)
        {
            teacherToAddTo.LessonIDs.Add(lesson.Id);
            await _teacherCollection.ReplaceOneAsync(x => x.Id == teacherToAddTo.Id, teacherToAddTo);
        }
    }
}
