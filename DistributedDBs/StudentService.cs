using DistributedDBs.DAL.Settings;
using DistributedDBs.DAL;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DistributedDBs.Services
{
    public class StudentService
    {
        private readonly IMongoCollection<Student> _studentsCollection;
        public StudentService(IOptions<MongoDBSettings> mongoDBSettings, IMongoClient
        mongoClient)
        {
            var mongoDatabase =
            mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _studentsCollection = mongoDatabase.GetCollection<Student>("Students");
        }
        public async Task<List<Student>> GetAsync() =>
            await _studentsCollection.Find(s => true).ToListAsync();
        public async Task<Student> GetByIdAsync(int id) =>
            await _studentsCollection.Find(s => s.StudentId == id).FirstOrDefaultAsync();
        public async Task CreateAsync(Student student) =>
            await _studentsCollection.InsertOneAsync(student);
        public async Task UpdateAsync(int id, Student updatedStudent) =>
            await _studentsCollection.ReplaceOneAsync(s => s.StudentId == id, updatedStudent);
        public async Task RemoveAsync(int id) =>
            await _studentsCollection.DeleteOneAsync(s => s.StudentId == id);
    }

    public class FacultyService
    {
        private readonly IMongoCollection<Faculty> _facultiesCollection;
        public FacultyService(IOptions<MongoDBSettings> mongoDBSettings, IMongoClient mongoClient)
        {
            var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _facultiesCollection = mongoDatabase.GetCollection<Faculty>("Faculties");
        }
        public async Task<List<Faculty>> GetAsync() =>
            await _facultiesCollection.Find(f => true).ToListAsync();
        public async Task<Faculty> GetByIdAsync(int id) =>
            await _facultiesCollection.Find(f => f.FacultyId == id).FirstOrDefaultAsync();
        public async Task CreateAsync(Faculty faculty) =>
            await _facultiesCollection.InsertOneAsync(faculty);
        public async Task UpdateAsync(int id, Faculty updatedFaculty) =>
            await _facultiesCollection.ReplaceOneAsync(f => f.FacultyId == id, updatedFaculty);
        public async Task RemoveAsync(int id) =>
            await _facultiesCollection.DeleteOneAsync(f => f.FacultyId == id);
    }


}
