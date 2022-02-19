using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyScore.Core.Dtos
{
    public class CourseDto
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0, Int64.MaxValue)]
        public int? NumberOfCredits { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

        [Range(0, 10)]
        [RegularExpression(@"\d(\.\d)?")] // 4.5, 4, 4.
        public double? Score { get; set; }
        public int? OrderNumber { get; set; }

    }
}