using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Ai_LibraryApi.Dto;
using Ai_LibraryApi.Helper;

namespace Ai_LibraryApi.Filters
{
    public class CustomAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.Result is ForbidResult)
            {
                var apiResponse = new ApiResponse<string>(
                    false,
                    "ليس لديك الصلاحيات الكافية للوصول إلى هذه الخدمة. هذه الخدمة متاحة فقط للمسؤولين.",
                    null
                );

                context.Result = new JsonResult(apiResponse)
                {
                    StatusCode = 403
                };
            }
            else if (context.Result is UnauthorizedResult)
            {
                var apiResponse = new ApiResponse<string>(
                    false,
                    "غير مصرح لك بالوصول. يرجى تسجيل الدخول أولاً.",
                    null
                );

                context.Result = new JsonResult(apiResponse)
                {
                    StatusCode = 401
                };
            }
        }
    }
}
