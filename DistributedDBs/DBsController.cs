using DistributedDBs.DAL;
using DistributedDBs.DAL.Settings;
using DistributedDBs.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DistributedDBs
{
   
    [ApiController]
    [Route("api/mssql/[action]")]
    public class MicrosoftSQLController : ControllerBase
    {
        private readonly MyDbContext _context;
        public MicrosoftSQLController(MyDbContext context) => _context = context;

        [HttpPost]
        public IActionResult seed()
        {
            if (_context.Faculty.Any() || _context.Groups.Any() || _context.Students.Any() || _context.Courses.Any())
            {
                return BadRequest("The database is already seeded!");
            }

            var faculty1 = new Faculty { Name = "Informatics" };
            var faculty2 = new Faculty { Name = "Mathematics" };

            var group1 = new Group { Number = 101, Specialization = "Informatics", Faculty = faculty1 };
            var group2 = new Group { Number = 102, Specialization = "Mathematics", Faculty = faculty2 };

            var student1 = new Student { FirstName = "Alice", LastName = "Johnson", Group = group1 };
            var student2 = new Student { FirstName = "Bob", LastName = "Smith", Group = group1 };
            var student3 = new Student { FirstName = "Charlie", LastName = "Brown", Group = group2 };

            var course1 = new Course { Title = "Algorithms", Credits = 3 };
            var course2 = new Course { Title = "Linear Algebra", Credits = 4 };
            var course3 = new Course { Title = "Databases", Credits = 3 };

            var studentCourse1 = new StudentCourse { Student = student1, Course = course1 };
            var studentCourse2 = new StudentCourse { Student = student2, Course = course1 };
            var studentCourse3 = new StudentCourse { Student = student2, Course = course2 };
            var studentCourse4 = new StudentCourse { Student = student3, Course = course3 };

            _context.Groups.AddRange(group1, group2);
            _context.Students.AddRange(student1, student2, student3);
            _context.Courses.AddRange(course1, course2, course3);
            _context.StudentCourses.AddRange(studentCourse1, studentCourse2, studentCourse3, studentCourse4);

            _context.SaveChanges();

            return Ok("Database seeding completed!");
        }



        [HttpDelete]
        public IActionResult clear()
        {
            _context.StudentCourses.RemoveRange(_context.StudentCourses);
            _context.Students.RemoveRange(_context.Students);
            _context.Courses.RemoveRange(_context.Courses);
            _context.Groups.RemoveRange(_context.Groups);
            _context.Faculty.RemoveRange(_context.Faculty);

            _context.SaveChanges();

            return Ok("Data was cleared");
        }
    }

    [ApiController]
    [Route("api/mongodb/[action]")]
    public class MongoNoSQLController : ControllerBase
    {
        private readonly UpdateService _sync;
        private readonly StudentService _studentService;
        private readonly FacultyService _facultyService;
        public MongoNoSQLController(UpdateService sync, 
            StudentService studentService,
            FacultyService facultyService)
        {
            _studentService = studentService;
            _sync = sync;
            _facultyService = facultyService;
        }
        private readonly IMongoCollection<Student> _studentsCollection;

        [HttpPost]
        public async Task<IActionResult> upload()
        {
            try
            {
                await _sync.Sync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            return Ok("Data was uploaded from mssql!");
        }
        [HttpDelete]
        public async Task<IActionResult> clear(IOptions<MongoDBSettings> mongoDBSettings, IMongoClient
        mongoClient)
        {
            await mongoClient.DropDatabaseAsync(mongoDBSettings.Value.DatabaseName);
            return Ok("All documents was truncated!");
        }
        [HttpGet]
        public async Task<IActionResult> stundents() => Ok(await _studentService.GetAsync());


        [HttpGet]
        public async Task<IActionResult> faculties() => Ok(await _facultyService.GetAsync());
    }
    }
