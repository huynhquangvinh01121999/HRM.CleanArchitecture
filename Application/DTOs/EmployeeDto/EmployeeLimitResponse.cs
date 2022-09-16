﻿using System;

namespace Application.DTOs.EmployeeDto
{
    public class EmployeeLimitResponse
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public DateTime DoB { get; set; }

        public string Email { get; set; }

        public string TName { get; set; }

        public string DName { get; set; }

        public string MName { get; set; }
    }
}
