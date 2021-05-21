using System.Threading.Tasks;
using Contracts;
using Entities;

namespace Repostitory {
    public class RepositoryManager : IRepositoryManager {
        private readonly RepositoryContext _repositoryContext;
        private ICompanyRepository _companyRepository;
        private IEmployeeRepository _employeeRepository;

        public RepositoryManager(RepositoryContext repositoryContext) {
            _repositoryContext = repositoryContext;
        }

        public ICompanyRepository CompanyRepository {
            get {
                return _companyRepository ??= new CompanyRepository(_repositoryContext);
            }
        }

        public IEmployeeRepository EmployeeRepository {
            get {
                return _employeeRepository ??= new EmployeeRepository(_repositoryContext);
            }
        }

        public async Task SaveAsync() {
            await _repositoryContext.SaveChangesAsync();
        }
    }
}