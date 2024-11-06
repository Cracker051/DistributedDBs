using DistributedDBs.DAL;
using DistributedDBs.DAL.Settings;
using DistributedDBs.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Transactions;

namespace DistributedDBs
{
    public class UpdateService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly string _databaseName;
        private readonly IMongoClient _mongoClient;
        public UpdateService(IServiceScopeFactory serviceScopeFactory,
            IOptions<MongoDBSettings> mongoDBSettings,
            IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
            _serviceScopeFactory = serviceScopeFactory;
            _databaseName = mongoDBSettings.Value.DatabaseName;
        }

        public async Task StartAsync(CancellationToken cancellationToken) => await Sync();
        public async Task Sync()
        {
            await _mongoClient.DropDatabaseAsync(_databaseName);

            await using (var scope = _serviceScopeFactory.CreateAsyncScope())
            using (var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>())
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var studentService = scope.ServiceProvider.GetRequiredService<StudentService>();
                var facultyService = scope.ServiceProvider.GetRequiredService<FacultyService>();

                try
                {
                    var students = dbContext.Students
                        .Include(s => s.Group)
                        .ThenInclude(g => g.Faculty)
                        .Include(s => s.StudentCourses)
                        .ThenInclude(sc => sc.Course)
                        .ToList();
                    foreach (var student in students)
                    {
                        await studentService.CreateAsync(student);
                    }

                    var faculties = dbContext.Faculty
                        .Include(f => f.Groups)
                        .ThenInclude(g => g.Students)
                        .ToList();
                    foreach (var faculty in faculties)
                    {
                        await facultyService.CreateAsync(faculty);
                    }

                    transaction.Complete();
                }
                catch (Exception)
                {
                    transaction.Dispose();
                    throw;
                }
            }
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
