using IServices.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Services.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreApi.Filter.Authorization
{
    /// <summary>
    /// 在Controller类上添加此特性，以声明该控制器需要权限验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AuthorizationFilterAttribute : Attribute, IAuthorizationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        public IPermissionCheckerService PermissionChecker {  get; set; }
        /// <summary>
        /// 進入方法之前將檢查
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // 如果不是一个控制器方法则直接返回
            if (!(context.ActionDescriptor is ControllerActionDescriptor action))
            {
                return;
            }
            //如果方法上加了[AllowAnonymous]，就跳过权限检查
            if (action.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).Any())
                return;
            var authorizeAttribute = context.Filters.FirstOrDefault(f => f.GetType() == typeof(ApiAuthorizeAttribute)) as ApiAuthorizeAttribute;
            if (authorizeAttribute == null) //如果未加[ApiAuthorize]则直接放过
                return;
            string code = context.HttpContext.Request.Headers["code"].ToString().Trim();
            if (authorizeAttribute?.Permissions.Length > 0)
            {
                //如果声明必须具有全部权限则将pass变量初始值设为true，否则设为false
                var pass = authorizeAttribute.RequireAllPermissions;
                foreach (var permission in authorizeAttribute.Permissions)
                {
                    var granted = PermissionChecker.IsGrantedAsync(permission, code).Result;
                    //如果需要检查全部权限，则使用[与]运算，否则使用[或]运算
                    pass = authorizeAttribute.RequireAllPermissions ? pass && granted : pass || granted;
                    //1.如果需要检查全部权限而本次检查结果为false
                    //2.如果只需具有任一权限而本次检查结果为true
                    //符合以上条件之一则提前退出循环，最后还有可能遇到需要全部权限且每次检查都为真 或 只需任一权限但每次检查都为假
                    if (pass != authorizeAttribute.RequireAllPermissions)
                        break;
                }
                //如果最终检查结果为false则修改上下文的返回值为403(未授权)，否则不必理会
                if (!pass)
                {
                    context.Result = new ObjectResult("未授权") { StatusCode = StatusCodes.Status403Forbidden };
                    return;
                }
            }

        }
    }
}
