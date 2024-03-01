using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var validarToken = new ValidarToken();
        var cookie = context.HttpContext.Request.Cookies["Token"];
        if (cookie == null)
        {
            context.Result = new JsonResult(new { message = "Token vazio" }) { StatusCode = StatusCodes.Status401Unauthorized };
            return;
        }

        int? userId = validarToken.ValidateToken(cookie);
        if (!userId.HasValue)
        {
            context.Result = new JsonResult(new { message = "Token inválido" }) { StatusCode = StatusCodes.Status401Unauthorized };
            return;
        }
    }
}