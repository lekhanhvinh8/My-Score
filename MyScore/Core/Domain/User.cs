using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyScore.Core.Domain
{
    public class User : IdentityUser
    {
        public User()
            : base()
        {
            this.Courses = new List<Course>();
        }
        public List<Course> Courses { get; set; }
    }
}