using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.Models;
using Entities.RequestFeatures;
using HoaiBaoCompanyApi.Extensions;
using Microsoft.EntityFrameworkCore;


namespace Repostitory {
    public class EmployeeRepository : RepositoryBase<Employee>,IEmployeeRepository {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext) {
        }

        public async Task<PagedList<Employee>>  GetEmployeesAsync(Guid companyId,EmployeeParameter parameter,
            bool trackChanges) {
            var employees = FindByCondition(e => e.CompanyId == companyId, trackChanges)
                .FilterEmployees(parameter.MinAge,parameter.MaxAge)
                .SearchEmployees(parameter.Query)
                .OrderEmployees(parameter.OrderBy);
            return await PagedList<Employee>.ToPageList(employees, parameter.PageNumber, parameter.PageSize);
        }

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges) {
            return await FindByCondition(e => e.CompanyId == companyId && e.Id == employeeId, trackChanges)
                .SingleOrDefaultAsync();
        }

        public void CreateEmployee(Guid companyId, Employee employee) {
            employee.CompanyId = companyId;
            Add(employee);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByIdsAsync(Guid companyId,IEnumerable<Guid> ids, bool trackChanges) {
            var employees = await FindByCondition(e => e.CompanyId == companyId
                                                 && ids.Contains(e.Id), trackChanges).ToListAsync();
            return employees;
        }

        public void DeleteEmployee(Employee employee) {
            Delete(employee);
        }
    }
}