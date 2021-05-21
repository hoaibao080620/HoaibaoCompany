using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Entities.Models;

namespace Entities.DTO {
    public abstract class CompanyForManipulationDto {
        [Required(ErrorMessage = "The name field is required")]
        [MaxLength(60,ErrorMessage = "Max length of name is 60 characters")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "The address field is required")]
        [MaxLength(60,ErrorMessage = "Max length of address is 60 characters")]
        public string Address { get; set; }

        public string Country { get; set; }
        public IEnumerable<Employee> Employees { get; set; }
    }
}