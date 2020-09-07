using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Model.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
namespace AspCoreApi.Filter.Action
{
    /// <summary>
    /// Action 過濾器
    /// </summary>
    public class CustomActionFilterAttribute : ActionFilterAttribute
    {
        private IConfiguration Configuration;
        /// <summary>
        /// 
        /// </summary>
        public CustomActionFilterAttribute(IConfiguration configuration) {
            Configuration = configuration;
        }

        /// <summary>
        /// 執行方法完成后
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                //對模型驗證失敗的結果進行自定義結果
                ReturnMessageModel<string> result = new ReturnMessageModel<string>();
                foreach (var item in context.ModelState.Values)
                {
                    foreach (var error in item.Errors)
                    {
                        result.Message += error.ErrorMessage + ",";
                        result.Data = "";
                    }
                }
                result.Message = result.Message.TrimEnd(',');
                result.Status = 1;
                context.Result = new JsonResult(result);
            }
            else
            {
                base.OnResultExecuting(context);
            }
        }

        /// <summary>
        /// 執行方法前
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
           
        }
    }
}
