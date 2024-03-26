using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Planio.Models.Database;
using Planio.Models;

namespace Planio.Services
{
    public class RoomsService
    {
        private readonly IMongoCollection<RoomModel> _roomCollection;

        private readonly IHttpContextAccessor _contextAccessor;

        public RoomsService(IOptions<PlanioDBSettings> planioDBSettings, IHttpContextAccessor contextAccessor)
        {
            var mongoClient = new MongoClient(
                planioDBSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                planioDBSettings.Value.DatabaseName);

            _roomCollection = mongoDatabase.GetCollection<RoomModel>(
                planioDBSettings.Value.RoomsCollectionName);

            _contextAccessor = contextAccessor;
        }

        public async Task<List<RoomModel>> GetAsync() =>
        await _roomCollection.Find(_ => true).ToListAsync();

        public async Task<RoomModel?> GetSingle(string id)
        {
            return await _roomCollection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();
        }

        public async Task<RoomModel?> GetWithRoomName(string roomName) =>
         await _roomCollection.Find(x => x.RoomName == roomName).FirstOrDefaultAsync();

        public async Task CreateAsync(RoomModel newRoom) =>
            await _roomCollection.InsertOneAsync(newRoom);

        public async Task UpdateAsync(string id, RoomModel updatedRoom) =>
            await _roomCollection.ReplaceOneAsync(x => x.Id == id, updatedRoom);

        public async Task RemoveAsync(string id) =>
            await _roomCollection.DeleteOneAsync(x => x.Id == id);

        public async Task DeleteAll()
        {
            await _roomCollection.DeleteManyAsync(x => x.Id != null);
        }
        public async Task AddLessonToRoom(RoomModel room, LessonModel lesson)
        {
            room.LessonIDs.Add(lesson.Id);
            await _roomCollection.ReplaceOneAsync(x => x.Id == room.Id, room);
        }
    }
}
