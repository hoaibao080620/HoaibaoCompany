using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.DTO;
using Entities.Models;
using HoaiBaoCompanyApi.ActionFilters;
using HoaiBaoCompanyApi.ModelBinder;
using LoggerServices;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace HoaiBaoCompanyApi.Controllers {
    [ApiController]
    [Route("api/companies")]
    public class CompaniesController : Controller {
        private readonly ILoggerManager _loggerManager;
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public CompaniesController(ILoggerManager loggerManager
            ,IRepositoryManager repositoryManager
            ,IMapper mapper) {
            _loggerManager = loggerManager;
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCompanies() {
            var companies = await _repositoryManager.CompanyRepository.GetCompaniesAsync(false);
            var mapperCompanies = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return Ok(mapperCompanies);
        }

        [ServiceFilter(typeof(ValidateExistCompanyFilter))]
        [HttpGet("{id}",Name = "CompanyById")]
        public IActionResult GetCompany(Guid id) {
            var company = HttpContext.Items["company"] as Company;
            var companyMapper = _mapper.Map<CompanyDto>(company);
            return Ok(companyMapper);
        }

        [ServiceFilter(typeof(ValidateActionFilterAttribute))]
        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto companyForCreationDto) {
            var companyEntity = _mapper.Map<Company>(companyForCreationDto);
            _repositoryManager.CompanyRepository.CreateCompany(companyEntity);
            await _repositoryManager.SaveAsync();
            var companyDto = _mapper.Map<CompanyDto>(companyEntity);
            return CreatedAtRoute("CompanyById", new {id = companyDto.Id}, companyDto);
        }
        
        [HttpGet("collections/({ids})",Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompaniesByIds([ModelBinder(BinderType = typeof(ArrayModelBinder))]
            IEnumerable<Guid> ids) {
            if (ids is null) {
                _loggerManager.LogError("The ids send from client is null");
                return BadRequest("The ids is null");
            }

            if (!ModelState.IsValid) {
                return BadRequest("The arguments is wrong");
            }

            var companies =await _repositoryManager.CompanyRepository.GetCompaniesByIdsAsync(ids, false);
            if (ids.Count() != companies.Count()) {
                _loggerManager.LogError("Some id are not valid in ids");
                return NotFound();
            }

            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return Ok(companiesDto);
        }

        [HttpPost("collections")]
        public async Task<IActionResult> CreateCompanies([FromBody] IEnumerable<CompanyForCreationDto> companiesDto) {
            if (companiesDto is null) {
                _loggerManager.LogError("The collection send from client is null");
                return BadRequest("Collection is null");
            }

            if (!ModelState.IsValid) {
                _loggerManager.LogError("The model state of dto is not valid");
                return UnprocessableEntity(ModelState);
            }
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companiesDto);
            foreach (var company in companyEntities) {
                _repositoryManager.CompanyRepository.CreateCompany(company);
            }
            
            await _repositoryManager.SaveAsync();
            var companiesReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companiesReturn.Select(c => c.Id));

            return CreatedAtRoute("CompanyCollection", new {ids}, companiesReturn);
        }

        [ServiceFilter(typeof(ValidateExistCompanyFilter))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(Guid id) {
            var company = HttpContext.Items["company"] as Company;
            _repositoryManager.CompanyRepository.DeleteCompany(company);
            await _repositoryManager.SaveAsync();

            return NoContent();
        }

        [ServiceFilter(typeof(ValidateExistCompanyFilter))]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateCompany(Guid id, JsonPatchDocument<CompanyForUpdateDto> patchDocument) {
            var company = HttpContext.Items["company"] as Company;
            
            var companyUpdateDto = _mapper.Map<CompanyForUpdateDto>(company);
            patchDocument.ApplyTo(companyUpdateDto,ModelState);
            if (!ModelState.IsValid) {
                _loggerManager.LogError("The model state of json patch is not valid");
                return UnprocessableEntity(ModelState);
            }
            
            if (!TryValidateModel(companyUpdateDto)) {
                _loggerManager.LogError("The model state of dto is not valid");
                return UnprocessableEntity(ModelState);
            }
            _mapper.Map(companyUpdateDto, company);
            await _repositoryManager.SaveAsync();

            return NoContent();
        }
    }
}