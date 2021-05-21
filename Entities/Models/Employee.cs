using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models {
    public class Employee {
        [Column("EmployeeId")]
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "The name field is required")]
        [MaxLength(30,ErrorMessage = "Max length of name is 30 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The age is required")]
        public int Age { get; set; }

        [Required(ErrorMessage = "The position field is required")]
        [MaxLength(20,ErrorMessage = "Max length of position is 30 characters")]
        public string Position { get; set; }

        [ForeignKey(nameof(Company))]
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
    }
}