using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cumulative_1.Models;
using System;
using MySql.Data.MySqlClient;

namespace Cumulative_1.Controllers
{
    [Route("api/Teacher")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {
        private readonly SchooldbContext _context;
        public TeacherAPIController(SchooldbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Retrieves a list of teachers, including their courses, with an optional filter by hire date range.
        /// </summary>
        /// <param name="StartDate">The start date of the hire date range (optional).</param>
        /// <param name="EndDate">The end date of the hire date range (optional).</param>
        /// <returns>
        /// A list of teachers, each containing their details such as ID, first name, last name, hire date, salary, employee number, and associated course names.
        /// </returns>
        /// <remarks>
        /// This method connects to the database, retrieves teacher and course information, and optionally filters the teachers by hire date range.
        /// If no date range is provided, all teachers will be returned along with their courses.
        /// </remarks>
        /// <example>
        /// GET api/Teacher/ListTeachers -> [{"teacherId": 1,"teacherFName": "Alexander","teacherLName": "Bennett","teacherHireDate": "2016-08-05T00:00:00","teacherSalary": "55.30","teacherEmpNu": "T378","courseNames": ["Web Application Development"]},....]
        /// GET api/Teacher/ListTeachers?StartDate=2016-01-01&EndDate=2018-01-01 -> [{"teacherId":1,"teacherFName":"Alexander","teacherLName":"Bennett","teacherHireDate":"2016-08-05T00:00:00","teacherSalary":"55.30","teacherEmpNu":"T378","courseNames":["Web Application Development"]},{"teacherId":6,"teacherFName":"Thomas","teacherLName":"Hawkins","teacherHireDate":"2016-08-10T00:00:00","teacherSalary":"54.45","teacherEmpNu":"T393","courseNames":["Career Connections"]}]
        /// </example>


        [HttpGet]
        [Route(template: "ListTeachers")]
        public List<Teacher> ListTeachers(DateTime? StartDate = null, DateTime? EndDate = null)
        {
            List<Teacher> Teachers = new List<Teacher>();

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();


                string query = "SELECT * FROM teachers LEFT JOIN courses ON teachers.teacherid = courses.teacherid";


                bool hasConditions = false;
                if (StartDate.HasValue && EndDate.HasValue)
                {
                    query += " WHERE hiredate BETWEEN @startDate AND @endDate";
                    Command.Parameters.AddWithValue("@startDate", StartDate.Value);
                    Command.Parameters.AddWithValue("@endDate", EndDate.Value);
                    hasConditions = true;
                }

                Command.CommandText = query;
                Command.Prepare();

                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    Dictionary<int, Teacher> teacherDict = new Dictionary<int, Teacher>();

                    while (ResultSet.Read())
                    {
                        int Id = Convert.ToInt32(ResultSet["teacherid"]);
                        string FirstName = ResultSet["teacherfname"].ToString();
                        string LastName = ResultSet["teacherlname"].ToString();
                        string TeacherEmpNu = ResultSet["employeenumber"].ToString();
                        DateTime TeacherHireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                        decimal TeacherSalary = Convert.ToDecimal(ResultSet["salary"]);
                        string CourseName = ResultSet["coursename"].ToString();

                        if (!teacherDict.ContainsKey(Id))
                        {
                            teacherDict[Id] = new Teacher()
                            {
                                TeacherId = Id,
                                TeacherFName = FirstName,
                                TeacherLName = LastName,
                                TeacherHireDate = TeacherHireDate,
                                TeacherSalary = TeacherSalary,
                                TeacherEmpNu = TeacherEmpNu,

                            };
                        }
                    }

                    Teachers.AddRange(teacherDict.Values);
                }
            }

            return Teachers;
        }


        /// <summary>
        /// Retrieves a single teacher's details, including the courses they teach, by their teacher ID.
        /// </summary>
        /// <param name="id">The ID of the teacher to retrieve.</param>
        /// <returns>
        /// The details of the teacher, including their first name, last name, employee number, hire date, salary, and a list of the courses they teach.
        /// </returns>
        /// <remarks>
        /// This method connects to the database and retrieves the teacher's information and the courses they teach.
        /// If the teacher exists, their details and course names will be returned.
        /// </remarks>
        /// <example>
        /// GET api/Teacher/FindTeacher/1 -> {"teacherId":1,"teacherFName":"Alexander","teacherLName":"Bennett","teacherHireDate":"2016-08-05T00:00:00","teacherSalary":"55.30","teacherEmpNu":"T378","courseNames":["Web Application Development"]}
        /// </example>

        [HttpGet]
        [Route(template: "FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {
            Teacher selectedTeacher = null;

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "SELECT * FROM teachers WHERE teacherid=@id";
                Command.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    if (ResultSet.Read())  // Only if there is a result
                    {
                        selectedTeacher = new Teacher
                        {
                            TeacherId = Convert.ToInt32(ResultSet["teacherid"]),
                            TeacherFName = ResultSet["teacherfname"].ToString(),
                            TeacherLName = ResultSet["teacherlname"].ToString(),
                            TeacherEmpNu = ResultSet["employeenumber"].ToString(),
                            TeacherHireDate = Convert.ToDateTime(ResultSet["hiredate"]),
                            TeacherSalary = Convert.ToDecimal(ResultSet["salary"])
                        };
                    }
                }
            }

            return selectedTeacher;  // Will return null if not found.
        }

        /// <summary>
        /// Retrieves a list of courses taught by a specific teacher, identified by their teacher ID.
        /// </summary>
        /// <param name="id">The ID of the teacher whose courses are to be retrieved.</param>
        /// <returns>
        /// A list of course names taught by the specified teacher.
        /// </returns>
        /// <remarks>
        /// This method connects to the database and retrieves the list of courses assigned to the teacher with the given ID.
        /// If no courses are found, an empty list will be returned.
        /// </remarks>
        /// <example>
        /// GET api/Teacher/GetCoursesByTeacher/1 -> ["Web Application Development"]
        /// </example>

        [HttpGet]
        [Route("GetCoursesByTeacher/{id}")]
        public List<string> GetCoursesByTeacher(int id)
        {
            List<string> courses = new List<string>();

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "SELECT CourseName,teacherid FROM courses WHERE teacherid = @id";
                Command.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    while (ResultSet.Read())
                    {
                        string courseName = ResultSet["CourseName"].ToString();
                        int Id = Convert.ToInt32(ResultSet["teacherid"]);


                        courses.Add(courseName);
                    }
                }
            }

            return courses;
        }
        /// <summary>
        /// Adds a teacher to the database
        /// </summary>
        /// <param name="Teacherdata">Teacher Object</param>
        /// <example>
        /// POST: api/Teacher/AddTeacher
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// {
        ///   "teacherId": 20,
        ///    "teacherFName": "Rohit",
        ///    "teacherLName": "Kumar",
        ///    "teacherHireDate": "2024-11-29T04:13:56.436Z",
        ///    "teacherSalary": 80,
        ///    "teacherEmpNu": "N0178"
        /// } -> 20
        /// </example>
        /// <returns></returns>
        [HttpPost(template: "AddTeacher")]
        public int AddTeacher([FromBody] Teacher Teacherdata)
        {
            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "INSERT INTO teachers(teacherfname,teacherlname,hiredate,employeenumber,salary) VALUES(@teacherfname,@teacherlname,@hiredate,@TeacherEmpNu,@salary)";
                Command.Parameters.AddWithValue("@teacherfname", Teacherdata.TeacherFName);
                Command.Parameters.AddWithValue("@teacherlname", Teacherdata.TeacherLName);
                Command.Parameters.AddWithValue("@hiredate", Teacherdata.TeacherHireDate);
                Command.Parameters.AddWithValue("@TeacherEmpNu", Teacherdata.TeacherEmpNu);
                Command.Parameters.AddWithValue("@salary", Teacherdata.TeacherSalary);

                Command.ExecuteNonQuery();

                return Convert.ToInt32(Command.LastInsertedId);

            }
            // if failure
            return 0;
        }
        /// <summary>
        /// Deletes a teacher from the database by their ID
        /// </summary>
        /// <param name="TeacherId">The ID of the teacher to be deleted</param>
        /// <example>
        /// DELETE: api/Teacher/DeleteTeacher/1
        /// -> 1
        /// </example>
        /// <returns>
        /// The number of rows affected (1 if successful, 0 if no rows were deleted)
        /// </returns>

        [HttpDelete(template: "DeleteTeacher/{TeacherId}")]
        public int DeleteTeacher(int TeacherId)
        {
            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();


                Command.CommandText = "delete from teachers where teacherid=@id";
                Command.Parameters.AddWithValue("@id", TeacherId);
                return Command.ExecuteNonQuery();

            }
            // if failure
            return 0;
        }
        /// <summary>
        /// Updates a Teacher in the database. Data is a Teacher object, request query contains Teacher ID.
        /// </summary>
        /// <param name="TeacherData">Teacher Object containing updated teacher details</param>
        /// <param name="TeacherId">The Teacher ID primary key</param>
        /// <example>
        /// PUT: api/Teacher/UpdateTeacher/4
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// {
        ///     "TeacherFName": "Christine",
        ///     "TeacherLName": "Brittle",
        ///     "TeacherHireDate": "2020-08-15",
        ///     "TeacherEmpNu": "T98765",
        ///     "TeacherSalary": 55000
        /// } -> 
        /// {
        ///     "TeacherId": 4,
        ///     "TeacherFName": "Christine",
        ///     "TeacherLName": "Christine",
        ///     "TeacherHireDate": "2020-08-15",
        ///     "TeacherEmpNu": "T98765",
        ///     "TeacherSalary": 55000
        /// }
        /// </example>
        /// <returns>
        /// The updated Teacher object
        /// </returns>

        [HttpPut(template: "UpdateTeacher/{TeacherId}")]
        public Teacher UpdateTeacher(int TeacherId, [FromBody] Teacher TeacherData)
        {
            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // parameterize query
                Command.CommandText = "UPDATE teachers  set teacherfname=@teacherfname,teacherlname=@teacherlname,hiredate=@hiredate,employeenumber=@teacherempnum,salary=@salary WHERE teacherid=@id;";
                Command.Parameters.AddWithValue("@teacherfname", TeacherData.TeacherFName);
                Command.Parameters.AddWithValue("@teacherlname", TeacherData.TeacherLName);
                Command.Parameters.AddWithValue("@hiredate", TeacherData.TeacherHireDate);
                Command.Parameters.AddWithValue("@teacherempnum", TeacherData.TeacherEmpNu);
                Command.Parameters.AddWithValue("@salary", TeacherData.TeacherSalary);

                Command.Parameters.AddWithValue("@id", TeacherId);

                Command.ExecuteNonQuery();
            }

            return FindTeacher(TeacherId);
        }
    }
}