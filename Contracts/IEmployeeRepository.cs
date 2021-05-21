using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.RequestFeatures;

namespace Contracts {
    public interface IEmployeeRepository {
        Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId,EmployeeParameter parameter,bool trackChanges);
        Task<Employee> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges);
        void CreateEmployee(Guid companyId, Employee employee);
        Task<IEnumerable<Employee>> GetEmployeesByIdsAsync(Guid companyId,IEnumerable<Guid> ids,bool trackChanges);
        void DeleteEmployee(Employee employee);
    }
}