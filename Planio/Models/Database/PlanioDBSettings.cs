namespace Planio.Models.Database
{
    public class PlanioDBSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string StudentsCollectionName { get; set; } = null!;

        public string TeachersCollectionName { get; set; } = null!;
        public string RoomsCollectionName { get; set; } = null!;
        public string AdminsCollectionName { get; set; } = null!;
        public string ClassesCollectionName { get; set; } = null!;
        public string LessonsCollectionName { get; set; } = null!;
    }
}
