using AutoMapper;
using Entities.DTO;
using Entities.Models;

namespace HoaiBaoCompanyApi.Profiles {
    public class MappingProfile : Profile {
        public MappingProfile() {
            CreateMap<Company, CompanyDto>().ForMember(cDto => cDto.FullAddress,
                option => option.MapFrom(x => 
                    string.Join(", ",x.Address,x.Country)));
            CreateMap<Employee, EmployeeDto>();
            CreateMap<CompanyForCreationDto, Company>();
            CreateMap<EmployeeForCreationDto, Employee>();
            CreateMap<EmployeeForUpdateDto, Employee>().ReverseMap();
            CreateMap<CompanyForUpdateDto, Company>().ReverseMap();
        }
    }
}