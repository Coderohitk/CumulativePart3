using Cumulative_1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Cumulative_1.Controllers
{
    [Route("api/Course")]
    [ApiController]
    public class CourseAPIController : ControllerBase
    {
        private readonly SchooldbContext _context;
        public CourseAPIController(SchooldbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Retrieves a list of all courses from the database.
        /// </summary>
        /// <returns>
        /// A list of all courses, including their course ID, teacher ID, course code, course name, start date, and finish date.
        /// </returns>
        /// <remarks>
        /// This method connects to the database and retrieves all courses from the `courses` table.
        /// It returns a collection of course objects that include the course details such as ID, teacher, and course dates.
        /// </remarks>
        /// <example>
        /// GET api/Course/listCourse ->[{"courseId":1,"coursecode":"http5101","teacherid":1,"startdate":"2018-09-04T00:00:00","finishdate":"2018-12-14T00:00:00","coursename":"Web Application Development"},...]
        /// </example>
        [HttpGet]
        [Route(template: "listCourse")]
        public List<Course> ListCourse()
        {
            List<Course> Courses = new List<Course>();
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "Select * from courses";
                Command.Prepare();
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    while (ResultSet.Read())
                    {
                        int Couseid = Convert.ToInt32(ResultSet["courseid"]);
                        int teacherId = Convert.ToInt32(ResultSet["teacherid"]);
                        string CourseCode = ResultSet["coursecode"].ToString();
                        string CourseName = ResultSet["coursename"].ToString();
                        DateTime StartDate = Convert.ToDateTime(ResultSet["startdate"]);
                        DateTime FinishDate = Convert.ToDateTime(ResultSet["finishdate"]);

                        Course CurrentCourse = new Course()
                        {
                            courseId = Couseid,
                            teacherid = teacherId,
                            coursecode = CourseCode,
                            coursename = CourseName,
                            startdate = StartDate,
                            finishdate = FinishDate

                        };

                        Courses.Add(CurrentCourse);

                    }
                }

            }
            return Courses;
        }
        /// <summary>
        /// Retrieves details of a specific course by its course ID.
        /// </summary>
        /// <param name="id">The ID of the course to retrieve.</param>
        /// <returns>
        /// The details of the specified course, including its course ID, teacher ID, course code, course name, start date, and finish date.
        /// </returns>
        /// <remarks>
        /// This method connects to the database and retrieves the details of a specific course from the `courses` table
        /// based on the provided course ID. If the course is found, the details will be returned.
        /// </remarks>
        /// <example>
        /// GET api/Course/FindCourse/1 -> {"courseId":1,"coursecode":"http5101","teacherid":1,"startdate":"2018-09-04T00:00:00","finishdate":"2018-12-14T00:00:00","coursename":"Web Application Development"}
        /// </example>
        [HttpGet]
        [Route(template: "FindCourse/{id}")]
        public Course FindCourse(int id)
        {

            Course SelectedCourse = new Course();

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = "Select * from courses WHERE courseid = @id";
                Command.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    while (ResultSet.Read())
                    {
                        int Couseid = Convert.ToInt32(ResultSet["courseid"]);
                        int teacherId = Convert.ToInt32(ResultSet["teacherid"]);
                        string CourseCode = ResultSet["coursecode"].ToString();
                        string CourseName = ResultSet["coursename"].ToString();
                        DateTime StartDate = Convert.ToDateTime(ResultSet["startdate"]);
                        DateTime FinishDate = Convert.ToDateTime(ResultSet["finishdate"]);

                        SelectedCourse.courseId = Couseid;
                        SelectedCourse.teacherid = teacherId;
                        SelectedCourse.coursecode = CourseCode;
                        SelectedCourse.coursename = CourseName;
                        SelectedCourse.startdate = StartDate;
                        SelectedCourse.finishdate = FinishDate;
                    }
                }
            }
            return SelectedCourse;
        }
        /// <summary>
        /// Adds a new course to the database.
        /// </summary>
        /// <param name="CourseData">The course object containing course code, teacher ID, start date, finish date, and course name.</param>
        /// <example>
        /// POST: api/Student/AddCourse
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// {
        ///     "coursecode": "CS101",
        ///     "teacherid": 5,
        ///     "startdate": "2024-01-15T00:00:00",
        ///     "finishdate": "2024-06-01T00:00:00",
        ///     "coursename": "Introduction to Computer Science"
        /// }
        /// Response:
        /// 7  // Returns the inserted Course ID
        /// </example>
        /// <returns>
        /// The ID of the newly inserted course if successful. Returns 0 if unsuccessful.
        /// </returns>
        [HttpPost(template: "AddCourse")]
        public int AddCourse([FromBody] Course CourseData)
        {
            // 'using' ensures the connection is closed after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();

                // Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = "INSERT INTO courses(coursecode, teacherid, startdate, finishdate, coursename) " +
                                      "VALUES(@coursecode, @teacherid, @startdate, @finishdate, @coursename)";

                // Adding parameters to prevent SQL injection
                Command.Parameters.AddWithValue("@coursecode", CourseData.coursecode);
                Command.Parameters.AddWithValue("@teacherid", CourseData.teacherid);
                Command.Parameters.AddWithValue("@startdate", CourseData.startdate);
                Command.Parameters.AddWithValue("@finishdate", CourseData.finishdate);
                Command.Parameters.AddWithValue("@coursename", CourseData.coursename);

                // Execute the query and return the ID of the newly inserted course
                Command.ExecuteNonQuery();
                return Convert.ToInt32(Command.LastInsertedId);
            }
            // Return 0 if the operation fails
            return 0;
        }
        /// <summary>
        /// Deletes a course from the database based on its ID.
        /// </summary>
        /// <param name="courseId">The ID of the course to be deleted.</param>
        /// <example>
        /// DELETE: api/Student/DeleteCourse/3
        /// Response:
        /// 1  // Returns the number of rows affected (1 if successful)
        /// </example>
        /// <returns>
        /// The number of rows affected. Returns 0 if the deletion is unsuccessful.
        /// </returns>
        [HttpDelete(template: "DeleteCourse/{courseId}")]
        public int DeleteCourse(int courseId)
        {
            // 'using' ensures the connection is closed after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();

                // Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = "DELETE FROM courses WHERE courseid = @id";
                Command.Parameters.AddWithValue("@id", courseId);

                // Execute the query and return the number of rows affected
                return Command.ExecuteNonQuery();
            }
            // Return 0 if the operation fails
            return 0;
        }
        [HttpPut("UpdateCourse/{CourseId}")]
        public Course UpdateCourse(int CourseId, [FromBody] Course CourseData)
        {
            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();

                // Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // Parameterized query to prevent SQL injection
                Command.CommandText = "UPDATE courses SET coursecode=@coursecode, teacherid=@teacherid, startdate=@startdate, finishdate=@finishdate, coursename=@coursename WHERE courseid=@id";
                Command.Parameters.AddWithValue("@coursecode", CourseData.coursecode);
                Command.Parameters.AddWithValue("@teacherid", CourseData.teacherid);
                Command.Parameters.AddWithValue("@startdate", CourseData.startdate);
                Command.Parameters.AddWithValue("@finishdate", CourseData.finishdate);
                Command.Parameters.AddWithValue("@coursename", CourseData.coursename);
                Command.Parameters.AddWithValue("@id", CourseId);

                
                Command.ExecuteNonQuery();

               
               
            }

            // Return the updated course details
            return FindCourse(CourseId); // Return the updated course
        }


    }


}

