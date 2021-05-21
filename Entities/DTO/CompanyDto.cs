using System;

namespace Entities.DTO {
    public class CompanyDto {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string  FullAddress { get; set; }
    }
}