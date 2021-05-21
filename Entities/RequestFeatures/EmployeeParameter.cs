using System;
using System.Linq;
using Entities.Models;

namespace Entities.RequestFeatures {
    public class EmployeeParameter : RequestParameter {
        public uint MinAge { get; set; }
        public uint MaxAge { get; set; } = int.MaxValue;
        public bool IsValidRange => MinAge < MaxAge;

        public EmployeeParameter() {
            OrderBy = "Name";
        }
        
    }
}