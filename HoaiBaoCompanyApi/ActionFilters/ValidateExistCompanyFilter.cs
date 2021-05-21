using System;
using System.Threading.Tasks;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HoaiBaoCompanyApi.ActionFilters {
    public class ValidateExistCompanyFilter : IAsyncActionFilter {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryManager _repositoryManager;

        public ValidateExistCompanyFilter(ILoggerManager logger,IRepositoryManager repositoryManager) {
            _logger = logger;
            _repositoryManager = repositoryManager;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
            var controller = context.RouteData.Values["controller"].ToString();
            var action = context.RouteData.Values["action"].ToString();
            var arguments = context.ActionArguments;
            

            if (!(arguments.ContainsKey("companyId") || arguments.ContainsKey("id"))) {
                _logger.LogError($"Argument send from client is wrong type in controller {controller}" +
                                 $", action {action}");
                context.Result = new BadRequestObjectResult("Argument is not valid type!");
                return;
            }
            
            var idActionArgument = controller == "Companies" ? "id" : "companyId"; 
            
            var method = context.HttpContext.Request.Method;
            var trackChanges = method.Equals("PUT", StringComparison.InvariantCultureIgnoreCase)
                                || method.Equals("PATCH", StringComparison.InvariantCultureIgnoreCase);
            
            var companyId = (Guid)arguments[idActionArgument];
            var company = await _repositoryManager.CompanyRepository.GetCompanyAsync(companyId, trackChanges);

            if (company is null) {
                _logger.LogError($"Cannot found company with id {companyId} in database");
                context.Result = new NotFoundResult();
            }
            else {
                context.HttpContext.Items.Add("company",company);
                await next();
            }
            
            
        }
    }
}