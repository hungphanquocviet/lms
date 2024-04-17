using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo("LMSControllerTests")]
namespace LMS_CustomIdentity.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {

        private readonly LMSContext db;

        public ProfessorController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var query =
                from co in db.Courses
                where co.Subject == subject && co.CourseNo == num

                join c in db.Classes
                on co.CourseId equals c.CourseId
                where c.Season == season && c.Year == year
               
                join e in db.Enrolls
                on c.ClassId equals e.ClassId

                join s in db.Students
                on e.StudentId equals s.UId

                select new
                {
                    fname = s.FirstName,
                    lname = s.LastName,
                    uid = s.UId,
                    dob = s.DoB,
                    grade = e.Grade
                };

            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            var query =
                from co in db.Courses
                where co.Subject == subject && co.CourseNo == num

                join c in db.Classes
                on co.CourseId equals c.CourseId
                where c.Season == season && c.Year == year

                join ac in db.AssignCategories
                on c.ClassId equals ac.ClassId
                where ac.Category == category || category == null

                join a in db.Assignments
                on ac.CategoryId equals a.CategoryId

                select new
                {
                    aname = a.Name,
                    cname = ac.Category,
                    due = a.DueDate,
                    submissions = a.Submissions.Count(),
                };

            return Json(query.ToArray());
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var query =
                from co in db.Courses
                where co.Subject == subject && co.CourseNo == num

                join c in db.Classes
                on co.CourseId equals c.CourseId
                where c.Season == season && c.Year == year

                join ac in db.AssignCategories
                on c.ClassId equals ac.ClassId

                select new
                {
                    name = ac.Category,
                    weight = ac.GradeWeight,
                };
            return Json(query.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            try
            {
                var query =
                   from co in db.Courses
                   where co.Subject == subject && co.CourseNo == num

                   join c in db.Classes
                   on co.CourseId equals c.CourseId
                   where c.Season == season && c.Year == year

                   select c.ClassId;

                AssignCategory cat = new AssignCategory();
                cat.Category = category;
                cat.GradeWeight = (uint) catweight;
                cat.ClassId = query.First();

                db.AssignCategories.Add(cat);
                db.SaveChanges();

                return Json(new { success = true });
            }

            catch (Exception)
            {
                return Json(new { success = false });
            }

        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            try
            {
                // Create assignment
                var query =
                   from co in db.Courses
                   where co.Subject == subject && co.CourseNo == num

                   join c in db.Classes
                   on co.CourseId equals c.CourseId
                   where c.Season == season && c.Year == year

                   join ac in db.AssignCategories
                   on c.ClassId equals ac.ClassId
                   where ac.Category == category

                   select ac.CategoryId;

                Assignment assignment = new Assignment();
                assignment.CategoryId = query.First();
                assignment.Name = asgname;
                assignment.MaxPts = (uint) asgpoints;
                assignment.Content = asgcontents;
                assignment.DueDate = asgdue;

                db.Assignments.Add(assignment);
                db.SaveChanges();

                var students =
                    (from co in db.Courses
                    where co.Subject == subject && co.CourseNo == num

                    join c in db.Classes
                    on co.CourseId equals c.CourseId
                    where c.Season == season && c.Year == year

                    join e in db.Enrolls
                    on c.ClassId equals e.ClassId

                    select e).ToArray();

                List<string> grades = new();
                foreach ( var student in students )
                {
                    grades.Add(calculateGrade(subject, num, season, year, student.StudentId));
                }

                var studentsQuery =
                     from co in db.Courses
                     where co.Subject == subject && co.CourseNo == num

                     join c in db.Classes
                     on co.CourseId equals c.CourseId
                     where c.Season == season && c.Year == year

                     join e in db.Enrolls
                     on c.ClassId equals e.ClassId

                     select e;
                int index = 0;
                foreach (var student in students)
                {
                    Enroll currentStudent = student;
                    if (student != null)
                    {
                        currentStudent.Grade = grades[index];
                    }

                    index++;
                }
                db.SaveChanges();

                return Json(new { success = true });
            }

            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        private string calculateGrade(string subject, int num, string season, int year, string uid)
        {
            // Get categories
            var queryAssignCategory =
                (from co in db.Courses
                where co.Subject == subject && co.CourseNo == num

                join c in db.Classes
                on co.CourseId equals c.CourseId
                where c.Season == season && c.Year == year

                join ac in db.AssignCategories
                on c.ClassId equals ac.ClassId
                
                select ac).ToArray();
            double totalScore = 0;
            double categoryWeightSum = 0;

            // For each categories
            foreach ( var ac in queryAssignCategory )
            {
                
                var queryAssignment =
                    (from a in db.Assignments
                    where ac.CategoryId == a.CategoryId
                    select a).ToArray();

                if (queryAssignment.Count() <= 0) continue;

                // Calculate the total weight sum
                categoryWeightSum += ac.GradeWeight;

                
                // For each assignment in the assignment categories
                double maxAssignmentScore = 0;
                double studentAssignmentScore = 0;
                foreach ( var assignment in queryAssignment )
                {
                    maxAssignmentScore += assignment.MaxPts;
                    
                    var submissions =
                        (from s in db.Submissions
                        where assignment.AssignId == s.AssignId && uid == s.StudentId
                        select s.Score).ToArray();

                    if (submissions.Count() <= 0) continue;
                    studentAssignmentScore += submissions.First();
                }
                totalScore += (studentAssignmentScore / maxAssignmentScore) * ac.GradeWeight;
            }

            double scalingFactor = 100 / categoryWeightSum;
            double finalScore = totalScore * scalingFactor;


            return toLetterGrade(finalScore);
        }

        private string toLetterGrade(double grade )
        {
            if (grade >= 93)
            {
                return "A";
            }
            else if (grade < 93 && grade >= 90)
            {
                return "A-";
            }
            else if (grade < 90 && grade >= 87)
            {
                return "B+";
            }
            else if (grade < 87 && grade >= 83)
            {
                return "B";
            }
            else if (grade < 83 && grade >= 80)
            {
                return "B-";
            }
            else if (grade < 80 && grade >= 77)
            {
                return "C+";
            }
            else if (grade < 77 && grade >= 73)
            {
                return "C";
            }
            else if (grade < 73 && grade >= 70)
            {
                return "C-";
            }
            else if (grade < 70 && grade >= 67)
            {
                return "D+";
            }
            else if (grade < 67 && grade >= 63)
            {
                return "D";
            }
            else if (grade < 63 && grade >= 60)
            {
                return "D-";
            }
            else
            {
                return "E";
            }
        }

        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            var query =
                   from co in db.Courses
                   where co.Subject == subject && co.CourseNo == num

                   join c in db.Classes
                   on co.CourseId equals c.CourseId
                   where c.Season == season && c.Year == year

                   join ac in db.AssignCategories
                   on c.ClassId equals ac.ClassId
                   where ac.Category == category

                   join a in db.Assignments
                   on ac.CategoryId equals a.CategoryId
                   where a.Name == asgname

                   join s in db.Submissions
                   on a.AssignId equals s.AssignId

                   join st in db.Students
                   on s.StudentId equals st.UId

                   select new
                   {
                       fname = st.FirstName,
                       lname = st.LastName,
                       uid = st.UId,
                       time = s.Time,
                       score = s.Score
                   };

            return Json(query.ToArray());
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            try
            {
                var query =
                    from co in db.Courses
                    where co.Subject == subject && co.CourseNo == num

                    join c in db.Classes
                    on co.CourseId equals c.CourseId
                    where c.Season == season && c.Year == year

                    join ac in db.AssignCategories
                    on c.ClassId equals ac.ClassId
                    where ac.Category == category

                    join a in db.Assignments
                    on ac.CategoryId equals a.CategoryId
                    where a.Name == asgname

                    join s in db.Submissions
                    on a.AssignId equals s.AssignId
                    where s.StudentId == uid

                    select s;

                Submission? submission = query.SingleOrDefault();

                if (submission != null)
                {
                    submission.Score = (uint) score;
                }

                db.SaveChanges();

                var students =
                    from co in db.Courses
                    where co.Subject == subject && co.CourseNo == num

                    join c in db.Classes
                    on co.CourseId equals c.CourseId
                    where c.Season == season && c.Year == year

                    join e in db.Enrolls
                    on c.ClassId equals e.ClassId
                    where e.StudentId == uid

                    select e;


                Enroll currentStudent = students.First();
                if (currentStudent != null)
                {
                    currentStudent.Grade = calculateGrade(subject, num, season, year, uid);
                }

                db.SaveChanges();
                return Json(new { success = true });
            }

            catch (Exception)
            {
                return Json(new { success = false });
            }
            
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var query =
                from c in db.Classes
                where c.ProfId == uid

                join co in db.Courses
                on c.CourseId equals co.CourseId

                select new
                {
                    subject = co.Subject,
                    number = co.CourseNo,
                    name = co.CourseName,
                    season = c.Season,
                    year = c.Year
                };
            return Json(query.ToArray());
        }



        /*******End code to modify********/
    }
}

