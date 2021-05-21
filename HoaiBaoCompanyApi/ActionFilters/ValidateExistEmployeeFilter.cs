using System;
using System.Threading.Tasks;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HoaiBaoCompanyApi.ActionFilters {
    public class ValidateExistEmployeeFilter : IAsyncActionFilter {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryManager _repositoryManager;

        public ValidateExistEmployeeFilter(ILoggerManager logger,IRepositoryManager repositoryManager) {
            _logger = logger;
            _repositoryManager = repositoryManager;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
            var arguments = context.ActionArguments;
            if (!(arguments.ContainsKey("companyId") && arguments.ContainsKey("id"))) {
                var controller = context.RouteData.Values["controller"].ToString();
                var action = context.RouteData.Values["action"].ToString();
                _logger.LogError($"Argument send from client is wrong in controller {controller}" +
                                 $", action {action}");
                context.Result = new BadRequestObjectResult("Argument is wrong!");
                return;
            }
            
            var method = context.HttpContext.Request.Method;
            var trackChanges = method.Equals("PUT", StringComparison.InvariantCultureIgnoreCase)
                               || method.Equals("PATCH", StringComparison.InvariantCultureIgnoreCase);
            
            var companyId = (Guid)arguments["companyId"];
            var company = await _repositoryManager.CompanyRepository.GetCompanyAsync(companyId, trackChanges);
            if (company is null) {
                _logger.LogError($"Cannot found company with id {companyId} in database.");
                context.Result = new NotFoundResult();
                return;
            }

            var employeeId = (Guid) arguments["id"];
            var employee =
                await _repositoryManager.EmployeeRepository.GetEmployeeAsync(companyId, employeeId, trackChanges);
            if (employee is null) {
                _logger.LogError($"Cannot found employee with id {employeeId} in database.");
                context.Result = new NotFoundResult();
                return;
            }
            context.HttpContext.Items.Add("employee",employee);
            await next();
            
            
        }
    }
}