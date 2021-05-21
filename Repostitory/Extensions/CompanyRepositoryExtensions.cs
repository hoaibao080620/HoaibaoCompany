using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using Entities.Models;

namespace HoaiBaoCompanyApi.Extensions {
    public static class CompanyRepositoryExtensions {
        public static IQueryable<Company> OrderCompanies(this IQueryable<Company> companies, string orderQueryParameters) {
            if (string.IsNullOrEmpty(orderQueryParameters))
                return companies;
        
            var orderParams = orderQueryParameters.Trim().Split(",");
            var propertyInfos = typeof(Company).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var orderQueryBuilder = new StringBuilder();
        
            foreach (var param in orderParams) {
                if(string.IsNullOrWhiteSpace(param))
                    continue;
                var orderProperty = param.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0];
                var property = propertyInfos.FirstOrDefault(p =>
                    p.Name.Equals(orderProperty, StringComparison.InvariantCultureIgnoreCase));
                if(property is null) 
                    continue;
                var orderDirection = param.EndsWith(" desc") ? "desc" : "asc";
                orderQueryBuilder.Append($"{property.Name} {orderDirection},");
            }

            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
            return string.IsNullOrEmpty(orderQuery) ? 
                companies.OrderBy(c => c.Name) : companies.OrderBy(orderQuery);
        }
    }
}