using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using Entities.Models;

namespace HoaiBaoCompanyApi.Extensions {
    public static class EmployeeRepositoryExtensions {
        public static IQueryable<Employee> FilterEmployees(this IQueryable<Employee> source,
            uint minAge, uint maxAge) {
            return source.Where(e => e.Age >= minAge && e.Age <= maxAge);
        }

        public static IQueryable<Employee> SearchEmployees(this IQueryable<Employee> employees
        ,string query) {
            if (string.IsNullOrEmpty(query))
                return employees;
            query = query.Trim().ToLower();
            var searchEmployees = employees.Where(e => e.Name.ToLower().Contains(query));
            return searchEmployees;
        }

        public static IQueryable<Employee> OrderEmployees(this IQueryable<Employee> employees, string orderQuery) {
            if (string.IsNullOrEmpty(orderQuery))
                return employees;
            // Order query exp: name asc, age desc
            var orderParams = orderQuery.Trim().Split(",");
            var propertyInfos = typeof(Employee).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var orderQueryStringBuilder = new StringBuilder();
            foreach (var param in orderParams) {
                if(string.IsNullOrEmpty(param))
                    continue;
                var orderProperty = param.Split(" ",StringSplitOptions.RemoveEmptyEntries)[0];
                var property = propertyInfos.FirstOrDefault(p =>
                    p.Name.Equals(orderProperty, StringComparison.InvariantCultureIgnoreCase));
                if(property is null)
                    continue;
                var orderDirection = param.EndsWith(" desc") ? "descending" : "ascending";
                orderQueryStringBuilder.Append($"{property.Name} {orderDirection},");
            }

            var finalOrderQuery = orderQueryStringBuilder.ToString().TrimEnd(',',' ');
            return string.IsNullOrEmpty(finalOrderQuery) ? 
                employees.OrderBy(e => e.Name) 
                : employees.OrderBy(finalOrderQuery);
        }
    }
}