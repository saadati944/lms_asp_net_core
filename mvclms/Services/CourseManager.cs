using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using mvclms.Data;
using mvclms.Models;

namespace mvclms.Services
{
    public class CourseManager
    {
        private readonly ApplicationDbContext _dbContext;

        public CourseManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Course> GetCources(int skip = 0, int count = 10, bool includes = false)
        {
            if (includes)
                return _dbContext.Courses.Skip(skip).Take(count).Include(x => x.Files).Include(x => x.StudentCourses)
                    .ThenInclude(x => x.Student).Include(x => x.Teacher).ToList();
            return _dbContext.Courses.Skip(skip).Take(count).ToList();
        }

        public Course GetCourse(int id)
        {
            return _dbContext.Courses.Where(x => x.Id == id).Include(x => x.Files).Include(x => x.StudentCourses)
                .ThenInclude(x => x.Student).Include(x => x.Teacher).FirstOrDefault();
        }

        public void AddFileToCourse(IFormFile file, Course course, bool savechanges = false)
        {
            course.Files.Add(new File {Path = GetFilePath(file)});
            if(savechanges)
                _dbContext.SaveChanges();
        }

        public string GetFilePath(IFormFile file)
        {
            return ""; // todo : save file to disk and return path to it
        }
    }
}