using System;
using System.ComponentModel.DataAnnotations;

namespace Entities.DTO {
    public abstract class EmployeeForManipulationDto {
        [Required(ErrorMessage = "The name field is required")]
        [MaxLength(30,ErrorMessage = "Max length of name is 30 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "The age is required")]
        [Range(10,Int32.MaxValue,ErrorMessage = "The minimum age is 10")]
        public int Age { get; set; }
        [Required(ErrorMessage = "The position field is required")]
        [MaxLength(20,ErrorMessage = "Max length of position is 30 characters")]
        public string Position { get; set; }
    }
}