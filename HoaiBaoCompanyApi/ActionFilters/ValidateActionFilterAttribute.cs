using System;
using System.Linq;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HoaiBaoCompanyApi.ActionFilters {
    public class ValidateActionFilterAttribute : IActionFilter {
        private readonly ILoggerManager _logger;

        public ValidateActionFilterAttribute(ILoggerManager logger) {
            _logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context) {
            
        }

        public void OnActionExecuting(ActionExecutingContext context) {
            var controller = context.RouteData.Values["controller"];
            var action = context.RouteData.Values["action"];
            var param = context.ActionArguments
                .SingleOrDefault(c => c.Value.ToString().Contains("Dto")).Value;

            if (param is null) {
                _logger.LogError($"The object dto sent from client is null, controller: {controller}," +
                                 $" action:{action}");
                context.Result = new BadRequestObjectResult("The object dto send is null");
                return;
            }

            if (context.ModelState.IsValid)
                return;
            _logger.LogError($"The model state of object dto sent from client is not valid, controller: {controller}," +
                             $" action:{action}");
            context.Result = new UnprocessableEntityObjectResult(context.ModelState);

        }
    }
}