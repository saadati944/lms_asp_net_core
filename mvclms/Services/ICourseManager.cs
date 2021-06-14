using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using mvclms.Models;
using mvclms.ViewModels;

namespace mvclms.Services
{
    public interface ICourseManager
    {
        List<Course> GetCourses(int skip = 0, int count = 10, bool includes = false);
        List<Course> GetTeacherCourses(string userid);
        List<StudentCourse> GetStudentCourses(string userid);
        Course GetCourse(int id);
        List<Lecture> GetLectures(Course course);
        Lecture GetLecture(int lecId);
        int CreateCourse(CourseViewModel course, Person teacher);
        int CreateLecture(LectureViewModel lecture);
        int CreateLecture(LectureViewModel lecture, Course course);
        void UpdateLecture(UpdateLectureViewModel updatedlecture);

        /// <summary>
        /// save a file into the server (in the wwwroot/UploadPath) then return it's path.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>path to the file saved in the upload path</returns>
        string SaveFile(IFormFile file);

        void RemoveFile(string path);
        void CheckoutCourse(int courseId, Person student);
        void CheckoutCourse(Course c, Person student);
    }
}