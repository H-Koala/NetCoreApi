using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreApi.Filter.Exception
{
    /// <summary>
    /// 全局異常攔截器
    /// </summary>
    public class GlobalExceptionFilter : IAsyncExceptionFilter, IExceptionFilter
    {
        /// <summary>
        /// 发生异常时进入
        /// </summary>
        /// <param name="context"></param>
        public  void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled == false)
            {
                context.Result =  new JsonResult(
                    new ReturnMessageModel<object>()
                    {
                        Status = 1,
                        Message = context.Exception.Message
                    }
                );
            }
            context.ExceptionHandled = true;
        }

        /// <summary>
        /// 发生异常时进入
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public  Task OnExceptionAsync(ExceptionContext context)
        {
            OnException(context);
            return Task.CompletedTask;
        }
    }


}
