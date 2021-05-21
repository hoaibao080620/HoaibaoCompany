﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repostitory {
    public class CompanyRepository : RepositoryBase<Company>,ICompanyRepository {
        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext) {
        }

        public async Task<IEnumerable<Company>> GetCompaniesAsync(bool trackChanges) {
            return await FindAll(trackChanges).ToListAsync();
        }

        public async Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges) {
            return await FindByCondition(c => c.Id == companyId, trackChanges).SingleOrDefaultAsync();
        }

        public void CreateCompany(Company company) {
            Add(company);
        }

        public async Task<IEnumerable<Company>> GetCompaniesByIdsAsync(IEnumerable<Guid> ids, bool trackChanges) {
            return await FindByCondition(c => ids.Contains(c.Id), trackChanges)
                .ToListAsync();
        }

        public void DeleteCompany(Company company) {
            Delete(company);
        }
    }
}