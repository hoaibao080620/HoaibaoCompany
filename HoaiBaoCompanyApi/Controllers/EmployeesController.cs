using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.DTO;
using Entities.Models;
using Entities.RequestFeatures;
using HoaiBaoCompanyApi.ActionFilters;
using HoaiBaoCompanyApi.ModelBinder;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HoaiBaoCompanyApi.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IDataShaper<EmployeeDto> _dataShaper;

        public EmployeesController(IRepositoryManager repositoryManager
        ,IMapper mapper,ILoggerManager logger,IDataShaper<EmployeeDto> dataShaper) {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _logger = logger;
            _dataShaper = dataShaper;
        }

        [HttpGet]
        [ServiceFilter(typeof(ValidateExistCompanyFilter))]
        public async Task<IActionResult> GetEmployees(Guid companyId,[FromQuery]EmployeeParameter parameter) {
            if (!parameter.IsValidRange) {
                return BadRequest("The min age cannot larger than max age");
            }
            var employees = await _repositoryManager.EmployeeRepository
                .GetEmployeesAsync(companyId, parameter,false);
            
            Request.Headers.Add("X-pagination",JsonConvert.SerializeObject(employees.Metadata));
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            return Ok(_dataShaper.ShapeData(employeesDto, parameter.Fields));
        }

        [ServiceFilter(typeof(ValidateExistEmployeeFilter))]
        [HttpGet("{id}",Name = "GetEmployeeFromCompany")]
        public IActionResult GetEmployee(Guid companyId, Guid id) {
            var employee = HttpContext.Items["employee"] as Employee;
            var employeeDto = _mapper.Map<EmployeeDto>(employee);
            return Ok(employeeDto);
        }
        
        [ServiceFilter(typeof(ValidateExistCompanyFilter))]
        [HttpGet("collections/({ids})",Name = "EmployeeCollection")]
        public async Task<IActionResult> GetEmployeesByIds(Guid companyId,[ModelBinder(BinderType = typeof(ArrayModelBinder))] 
            IEnumerable<Guid> ids) {

            if (ids is null) {
                _logger.LogError("The collection sent from client is null.");
                return BadRequest("The collection is null");
            }

            var employees =await _repositoryManager.EmployeeRepository
                .GetEmployeesByIdsAsync(companyId, ids, false);
            if (employees.Count() != ids.Count()) {
                _logger.LogError("Some id are not found");
                return NotFound();
            }

            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            return Ok(employeesDto);
        }

        [ServiceFilter(typeof(ValidateExistCompanyFilter))]
        [HttpPost("collections")]
        public async Task<IActionResult> CreateEmployeesCollection(Guid companyId,
            [FromBody]IEnumerable<EmployeeForCreationDto> employeeForCreationDtos) {
            
            if (!ModelState.IsValid) {
                _logger.LogError("Invalid model state from object dto send");
                return UnprocessableEntity(ModelState);
            }

            if (employeeForCreationDtos is null) {
                _logger.LogError($"Cannot found company with id {companyId} in database.");
                return NotFound();
            }

            var employeesEntities = _mapper.Map<IEnumerable<Employee>>(employeeForCreationDtos);
            foreach (var employee in employeesEntities) {
                _repositoryManager.EmployeeRepository.CreateEmployee(companyId,employee);
            }
            await _repositoryManager.SaveAsync();

            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesEntities);
            var ids = string.Join(",", employeesDto.Select(e => e.Id));

            return CreatedAtRoute("EmployeeCollection", new {companyId,ids}, employeesDto);
        }
        
        [ServiceFilter(typeof(ValidateActionFilterAttribute))]
        [ServiceFilter(typeof(ValidateExistCompanyFilter))]
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(Guid companyId, [FromBody] EmployeeForCreationDto employeeForCreationDto) {
            var employeeEntity = _mapper.Map<Employee>(employeeForCreationDto);
            _repositoryManager.EmployeeRepository.CreateEmployee(companyId,employeeEntity);
            await _repositoryManager.SaveAsync();

            var employeeDto = _mapper.Map<EmployeeDto>(employeeEntity);
            return CreatedAtRoute("GetEmployeeFromCompany", new {
                companyId = employeeEntity.CompanyId,
                id = employeeDto.Id
            },employeeDto);
        }

        [ServiceFilter(typeof(ValidateExistEmployeeFilter))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid companyId, Guid id) {
            var employee = HttpContext.Items["employee"] as Employee;

            _repositoryManager.EmployeeRepository.DeleteEmployee(employee);
            await _repositoryManager.SaveAsync();

            return NoContent();
        }
        
        [ServiceFilter(typeof(ValidateActionFilterAttribute))]
        [ServiceFilter(typeof(ValidateExistEmployeeFilter))]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(Guid companyId, Guid id
            ,[FromBody] EmployeeForUpdateDto employeeForUpdate) {

            var employee = HttpContext.Items["employee"] as Employee;

            _mapper.Map(employeeForUpdate, employee);
            await _repositoryManager.SaveAsync();

            return NoContent();
        }

        [ServiceFilter(typeof(ValidateExistEmployeeFilter))]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateEmployee(Guid companyId, Guid id
            , [FromBody] JsonPatchDocument<EmployeeForUpdateDto> employeePatch) {
            if (employeePatch is null) {
                _logger.LogError("The object dto send from client is null");
                return BadRequest("Object dto send is null");
            }

            var employee = HttpContext.Items["employee"] as Employee;

            var employeeForUpdateDto = _mapper.Map<EmployeeForUpdateDto>(employee);
            employeePatch.ApplyTo(employeeForUpdateDto,ModelState);
            if (!ModelState.IsValid) {
                _logger.LogError("Invalid model state from patch document");
                return UnprocessableEntity(ModelState);
            }

            if (!TryValidateModel(employeeForUpdateDto)) {
                _logger.LogError("Invalid model state from object dto send");
                return UnprocessableEntity(ModelState);
            }
            _mapper.Map(employeeForUpdateDto, employee);
            await _repositoryManager.SaveAsync();

            return NoContent();
        }
    }
}