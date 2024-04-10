using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo("LMSControllerTests")]
namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private LMSContext db;
        public StudentController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
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


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var query =
                from e in db.Enrolls
                where e.StudentId == uid

                join c in db.Classes
                on e.ClassId equals c.ClassId

                join co in db.Courses
                on c.CourseId equals co.CourseId

                select new
                {
                    subject = co.Subject,
                    number = co.CourseNo,
                    name = co.CourseName,
                    season = c.Season,
                    year = c.Year,
                    grade = e == null ? "--" : e.Grade
                };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {
            var query =
                from e in db.Enrolls
                where e.StudentId == uid

                join c in db.Classes
                on e.ClassId equals c.ClassId
                where c.Season == season && c.Year == year

                join co in db.Courses
                on c.CourseId equals co.CourseId
                where co.Subject == subject && co.CourseNo == num

                join ac in db.AssignCategories
                on c.ClassId equals ac.ClassId

                join a in db.Assignments
                on ac.CategoryId equals a.CategoryId

                join s in db.Submissions
                on a.AssignId equals s.AssignId
                where s.StudentId == uid

                select new
                {
                    aname = a.Name,
                    cname = ac.Category,
                    due = a.DueDate,
                    score = s == null ? null : (uint?)s.Score,
                };
            return Json(query.ToArray());
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
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

                join s in db.Submissions
                on a.AssignId equals s.AssignId
                where s.StudentId == uid
                select s;

                Submission? submission = query.SingleOrDefault();

                if (submission != null)
                {
                    submission.Time = DateTime.Now;
                    submission.Content = contents;
                }

                else
                {
                    var query2 =
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

                        select a.AssignId;

                    submission = new Submission();
                    submission.StudentId = uid;
                    submission.Content = contents;
                    submission.Time = DateTime.Now;
                    submission.Score = 0;
                    submission.AssignId = query2.First();
                    db.Submissions.Add(submission);
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
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false}. 
        /// false if the student is already enrolled in the class, true otherwise.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {
            var query =
                from co in db.Courses
                where co.Subject == subject && co.CourseNo == num

                join c in db.Classes
                on co.CourseId equals c.CourseId
                where c.Season == season && c.Year == year

                select c.ClassId;

            try
            {
                Enroll enroll = new Enroll();
                enroll.ClassId = query.First();
                enroll.Grade = "--";
                enroll.StudentId = uid;

                db.Enrolls.Add(enroll);
                db.SaveChanges();

                return Json(new { success = true });
            }

            catch (Exception)
            {
                return Json(new { success = false });
            }
        }



        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student is not enrolled in any classes, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {

            var query =
                from e in db.Enrolls
                where e.StudentId == uid
                select e.Grade;
            double average = 0.0;
            if (query.Count() <= 0)
            {
                return Json(new {gpa = 0.0});
            }
            double total = 0.0;
            double classCount = 0;
            foreach (var grade in query)
            {
                // A scale
                if (grade == "A")
                    total += 4.0;
                else if (grade == "A-")
                    total += 3.7;

                // B scale
                else if (grade == "B+")
                    total += 3.3;
                else if (grade == "B")
                    total += 3.0;
                else if (grade == "B-")
                    total += 2.7;

                // C scale
                else if (grade == "C+")
                    total += 2.3;
                else if (grade == "C")
                    total += 2.0;
                else if (grade == "C-")
                    total += 1.7;

                // D scale
                else if (grade == "D+")
                    total += 1.3;
                else if (grade == "D")
                    total += 1.0;
                else if (grade == "D-")
                    total += 0.7;

                // E scale
                else if (grade == "E")
                    total += 0.0;
                else
                    continue;

                classCount++;
            }

            average = total / classCount;
            return Json(new { gpa = average});
        }

        /*******End code to modify********/

    }
}

