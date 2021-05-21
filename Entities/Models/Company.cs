using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models {
    public class Company {
        [Column("CompanyId")]
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "The name field is required")]
        [MaxLength(60,ErrorMessage = "Max length of name is 60 characters")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "The address field is required")]
        [MaxLength(60,ErrorMessage = "Max length of address is 60 characters")]
        public string Address { get; set; }

        public string Country { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}