using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyScore.Core.Domain
{
    public class Course
    {
        public Course()
        {
            
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public int NumberOfCredits { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public double? Score { get; set; }
        public int OrderNumber { get; set; }
    }
}