﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Service.Dtos.StudentDtos
{
    public class StudentGetDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public int GroupId { get; set; }
        public string GroupNo { get; set; }
        public string PhotoUrl { get; set; }
    }
}
