using AutoMapper;
using MyScore.Core.Domain;
using MyScore.Core.Dtos;
using MyScore.Persistence;
using MyScore.Persistence.Consts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace MyScore.api
{
    [Authorize]
    public class CoursesController : ApiController
    {
        private ApplicationDbContext _context;
        public CoursesController()
        {
            this._context = new ApplicationDbContext();
        }
        
        [HttpGet]
        public async Task<IHttpActionResult> GetCoursesOfUser()
        {
            var courses = await this._context.Courses
                .Where(c => c.User.UserName == User.Identity.Name)
                .ToListAsync();

            var CoursesIds = courses.Select(c => c.ID);
            var UserIds = courses.Select(c => c.UserId);

            await this._context.Users.Where(u => UserIds.Contains(u.Id)).LoadAsync();

            var result = Mapper.Map<IList<Course>, IList<CourseDto>>(courses);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IHttpActionResult> CalCredits()
        {
            var user = await this._context.Users.SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);

            //Load Course 
            await this._context.Courses.Where(c => c.UserId == user.Id).LoadAsync();

            var sumCredits = user.Courses.Select(c => c.NumberOfCredits).Sum();

            return Ok(sumCredits);
        }

        [HttpGet]
        public async Task<IHttpActionResult> CalScore()
        {
            var user = await this._context.Users.SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);

            //Load Course 
            await this._context.Courses.Where(c => c.UserId == user.Id).LoadAsync();

            var coursesHaveScore = user.Courses.Where(c => c.Score != null);

            var scores = coursesHaveScore.Select(c => Convert.ToDouble(c.Score * c.NumberOfCredits));
            var sumCredits = coursesHaveScore.Select(c => c.NumberOfCredits).Sum();

            var sumScore = scores.Sum();

            var averageScore = sumScore / sumCredits;
            
            var roundAvgScore = Math.Round(averageScore, 2);

            //average score need to boot to 8
            var nonScoreCredits = user.Courses
                .Where(c => c.Score == null)
                .Select(c => c.NumberOfCredits)
                .Sum();

            var totalCredits = sumCredits + nonScoreCredits;
            var totalScore8 = 8 * totalCredits;
            var sumScoreNeeded = totalScore8 - sumScore;
            double averageScoreNeeded = -1;
            
            if(nonScoreCredits != 0)
                averageScoreNeeded = sumScoreNeeded / nonScoreCredits;

            var result = new
            {
                SumScore = sumScore,
                SumCredits = sumCredits,
                AverageScore = averageScore,
                RoundAvgScore = roundAvgScore,
                AverageScoreNeededTo8 = averageScoreNeeded,
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Create(CourseDto courseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await this._context.Users.SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);
            
            //Course order handling
            if (courseDto.OrderNumber == null)
            {
                var courseOrders = await this._context.Courses
                    .Where(c => c.UserId == user.Id)
                    .Select(c => c.OrderNumber)
                    .ToListAsync();

                if (courseOrders.Count == 0)
                    courseDto.OrderNumber = 1;
                else
                {
                    var lastOrder = courseOrders.Max();
                    courseDto.OrderNumber = lastOrder + 1;
                }
            }
            else
            {
                var greaterOrderCourses = await this._context.Courses
                    .Where(c => c.UserId == user.Id && c.OrderNumber >= courseDto.OrderNumber)
                    .ToListAsync();

                foreach (var greaterCourse in greaterOrderCourses)
                {
                    greaterCourse.OrderNumber += 1;
                }
            }

            var course = Mapper.Map<CourseDto, Course>(courseDto);
            course.User = user;

            this._context.Courses.Add(course);
            await this._context.SaveChangesAsync();

            var result = Mapper.Map<Course, CourseDto>(course);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IHttpActionResult> UpdateScore(int courseId, double newScore)
        {
            if (newScore < 0 || newScore > 10)
                if(newScore != StaticValues.ResetScore)
                    return BadRequest();

            newScore = Math.Round(newScore, 1);

            var course = await this._context.Courses.SingleOrDefaultAsync(c => c.ID == courseId);
            if (course == null)
                return BadRequest();

            //Load User
            await this._context.Users.Where(u => u.Id == course.UserId).LoadAsync();

            if (course.User.UserName != User.Identity.Name)
                return Unauthorized();

            course.Score = newScore;

            if (newScore == StaticValues.ResetScore)
                course.Score = null;

            await this._context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        public async Task<IHttpActionResult> SwapOrderNumber(int courseId1, int courseId2)
        {
            var course1 = await this._context.Courses.SingleOrDefaultAsync(c => c.ID == courseId1);
            var course2 = await this._context.Courses.SingleOrDefaultAsync(c => c.ID == courseId2);

            if (course1 == null || course2 == null)
                return BadRequest();

            //Load User
            await this._context.Users.Where(u => u.Id == course1.UserId).LoadAsync();
            await this._context.Users.Where(u => u.Id == course2.UserId).LoadAsync();

            if (course1.UserId != course2.UserId)
                return Unauthorized();

            if (course1.User.UserName != User.Identity.Name)
                return Unauthorized();

            var tempOrder = course1.OrderNumber;
            course1.OrderNumber = course2.OrderNumber;
            course2.OrderNumber = tempOrder;

            await this._context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var course = await this._context.Courses.SingleOrDefaultAsync(c => c.ID == id);
            if (course == null)
                return BadRequest();

            //Load User
            await this._context.Users.Where(u => u.Id == course.UserId).LoadAsync();

            if (course.User.UserName != User.Identity.Name)
                return Unauthorized();

            //Decreasing order of all next course
            var nextCourses = this._context.Courses.Where(c => c.OrderNumber > course.OrderNumber);
            foreach (var nextCourse in nextCourses)
            {
                nextCourse.OrderNumber -= 1;
            }

            this._context.Courses.Remove(course);
            await this._context.SaveChangesAsync();

            return Ok();
        }
    }
}
