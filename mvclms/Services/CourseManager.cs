using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using mvclms.Data;
using mvclms.Models;
using mvclms.ViewModels;
using File = System.IO.File;

namespace mvclms.Services
{
    public class CourseManager : ICourseManager
    {
        private const string UploadPath = "uploads";
        private readonly ApplicationDbContext _dbContext;

        public CourseManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Course> GetCourses(int skip = 0, int count = 15, bool includes = true)
        {
            //TODO: fix this warning
            //The query uses a row limiting operator ('Skip'/'Take') without an 'OrderBy' operator. This may lead to unpredictable results.
            if (includes)
                return _dbContext.Courses.Skip(skip).Take(count).Include(x => x.Lectures).ThenInclude(x => x.Attachment)
                    .Include(x => x.StudentCourses)
                    .ThenInclude(x => x.Student).Include(x => x.Teacher)
                    .Include(x => x.Category).ToList();
            return _dbContext.Courses.Skip(skip).Take(count).ToList();
        }

        public List<Course> GetTeacherCourses(string userid)
        {
            return _dbContext.Courses.Where(x => x.TeacherId == userid).ToList();
        }

        public bool IsCourseCheckedOut(string userid, int courseid, bool checkForTeacher = false)
        {
            if (!checkForTeacher)
                return _dbContext.Users.Where(x => x.Id == userid).Include(x => x.StudentCourses).Single()
                    .StudentCourses
                    .Any(x => x.CourseId == courseid);
            return _dbContext.Users.Where(x => x.Id == userid).Include(x => x.Courses).Single()
                .Courses
                .Any(x => x.Id == courseid);
        }

        public List<StudentCourse> GetStudentCourses(string userid)
        {
            Person student = _dbContext.Users.Where(x => x.Id == userid).Include(x => x.StudentCourses)
                .ThenInclude(x => x.Course)
                .ThenInclude(x => x.Teacher).Include(x => x.StudentCourses).ThenInclude(x => x.Course)
                .ThenInclude(x => x.Category).Single(x => x.Id == userid);
            return student.StudentCourses.ToList();
        }

        public Course GetCourse(int id)
        {
            return _dbContext.Courses.Where(x => x.Id == id).Include(x => x.Lectures).ThenInclude(x => x.Attachment)
                .Include(x => x.StudentCourses)
                .ThenInclude(x => x.Student).Include(x => x.Teacher).FirstOrDefault();
        }

        public List<Lecture> GetLectures(Course course)
        {
            return _dbContext.Lectures.Where(x => x.Course == course).ToList();
        }

        public Lecture GetLecture(int lecId)
        {
            return _dbContext.Lectures.Where(x => x.Id == lecId).Include(x => x.Attachment).Include(x => x.Course)
                .ThenInclude(x => x.Teacher)
                .FirstOrDefault();
        }

        public int CreateCourse(CourseViewModel course, Person teacher)
        {
            Course c = new Course
            {
                Name = course.Name,
                Description = course.Description,
                Price = course.Price,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                Category = GetCategory(course.Category),
                Teacher = teacher
            };

            _dbContext.Add(c);
            _dbContext.SaveChanges();
            return c.Id;
        }

        private Category GetCategory(string cat)
        {
            cat = cat.Trim();
            Category c = _dbContext.Categories.FirstOrDefault(x => x.Name == cat);
            c ??= new Category
            {
                Name = cat
            };
            return c;
        }

        public int CreateLecture(LectureViewModel lecture)
        {
            return CreateLecture(lecture, GetCourse(int.Parse(lecture.Course)));
        }

        public int CreateLecture(LectureViewModel lecture, Course course)
        {
            Lecture l = new Lecture
            {
                Title = lecture.Title,
                Content = lecture.Content,
                Course = course,
                Attachment = new Models.File {Description = lecture.AttachmentDesc, Path = SaveFile(lecture.Attachment)}
            };
            _dbContext.Add(l);
            _dbContext.SaveChanges();
            return l.Id;
        }

        public void UpdateLecture(UpdateLectureViewModel updatedlecture)
        {
            var lecture = GetLecture(updatedlecture.LectureId);

            lecture.Content = updatedlecture.Content;
            lecture.Title = updatedlecture.Title;
            lecture.Attachment.Description = updatedlecture.AttachmentDesc;


            Models.File f = lecture.Attachment;
            if (updatedlecture.Attachment is not null)
            {
                _dbContext.Remove(f);
                RemoveFile(f.Path);
                lecture.Attachment = new Models.File
                    {Description = updatedlecture.AttachmentDesc, Path = SaveFile(updatedlecture.Attachment)};
            }

            _dbContext.SaveChanges();
        }

        /// <summary>
        /// save a file into the server (in the wwwroot/UploadPath) then return it's path.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>path to the file saved in the upload path</returns>
        public string SaveFile(IFormFile file)
        {
            if (file is null)
                return "";

            string relativePath = Path.Combine(UploadPath, Guid.NewGuid() + Path.GetExtension(file.FileName));

            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", UploadPath)))
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", UploadPath));
            using var stream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath),
                FileMode.Create);
            file.CopyToAsync(stream).Wait();

            return relativePath;
        }

        public void RemoveFile(string path)
        {
            string fullpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path);
            if (File.Exists(fullpath))
                File.Delete(fullpath);
        }

        public void CheckoutCourse(int courseId, Person student)
        {
            CheckoutCourse(GetCourse(courseId), student);
        }

        public void CheckoutCourse(Course c, Person student)
        {
            student.StudentCourses.Add(new StudentCourse {Course = c, Student = student});
            _dbContext.SaveChanges();
        }
    }
}