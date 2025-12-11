using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using WingDing_Party.IdentityProvider.Api.Common.Http;
using WingDing_Party.IdentityProvider.Application.Common.Errors;
using WingDing_Party.IdentityProvider.Application.Common.Errors.Abstract;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Error = WingDing_Party.IdentityProvider.Application.Common.Errors.Abstract.Error;

namespace WingDing_Party.IdentityProvider.Api.Controllers;

// Here we can override the Problem() Method logic to work properly with custom errors
[ApiController]
[AllowAnonymous]
public class ApiController : ControllerBase
{
    protected IActionResult Problem(Error error)
    {
        if (error is ValidationError validationError)
        {
            return ValidationProblem(validationError);
        }

        return Problem(
            title: error.ErrorMessage,
            statusCode: (int)error.StatusCode,
            type: error.Type.Name);
    }

    private IActionResult ValidationProblem(ValidationError error)
    {
        var modelStateDictionary = new ModelStateDictionary();

        foreach (var exception in error.Flatten())
        {
            modelStateDictionary.AddModelError(
                error.Type.Name,
                exception.Message);
        }

        return ValidationProblem(modelStateDictionary: modelStateDictionary);
    }
}
