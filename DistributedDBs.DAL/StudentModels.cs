using MongoDB.Bson.Serialization.Attributes;

namespace DistributedDBs.DAL
{
    public class Faculty
    {
        [BsonId]
        [BsonElement("_id")]
        public int FacultyId { get; set; }
        public string Name { get; set; }

        [BsonIgnore]
        public ICollection<Group> Groups { get; set; } 
    }

    public class Group
    {
        [BsonId]
        [BsonElement("_id")]
        public int Number { get; set; } 
        public string Specialization { get; set; }
        public int FacultyId { get; set; }

        [BsonIgnore]
        public Faculty Faculty { get; set; }

        [BsonIgnore]
        public ICollection<Student> Students { get; set; }
    }

    public class Student
    {
        [BsonId]
        [BsonElement("_id")]
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int GroupNumber { get; set; } 
        public Group Group { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; }
    }

    public class Course
    {
        [BsonId]
        [BsonElement("_id")]
        public int CourseId { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        [BsonIgnore]
        public ICollection<StudentCourse> StudentCourses { get; set; }
    }

    public class StudentCourse
    {
        [BsonId]
        [BsonElement("_id")]
        public int StudentId { get; set; }

        [BsonIgnore]
        public Student Student { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
