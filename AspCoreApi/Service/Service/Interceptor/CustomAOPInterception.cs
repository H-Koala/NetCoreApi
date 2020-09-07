using Castle.DynamicProxy;
using IServices.ICommon.IRepository.IDapper;
using Services.Common.Repository.Dapper;
using Services.Identification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Services.Interceptor
{
    /// <summary>
    /// AOP 攔截器
    /// </summary>
    public class CustomAOPInterception : IInterceptor
    {
        private  ISqlDapperService SqlDapperService;

        public CustomAOPInterception(ISqlDapperService _SqlDapperService)
        {
            this.SqlDapperService = _SqlDapperService;
        }

        /// <summary>
        /// 拦截器
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            if (methodInfo == null)
                methodInfo = invocation.Method;
            WorkCommitAttribute transaction = methodInfo.GetCustomAttributes<WorkCommitAttribute>(true).FirstOrDefault();
            //如果标记了 [WorkCommitAttribute]，并且不在事务嵌套中。
            if (transaction != null && SqlDapperService.Committed)//
            {
                //开启事务
                SqlDapperService.BeginTransaction();
                try
                {
                    invocation.Proceed();
                    //提交事务
                    SqlDapperService.CommitTransaction();

                }
                catch (Exception ex)
                {
                    //回滚
                    SqlDapperService.RollBackTransaction();
                    throw ex;
                }
            }
            else
            {
                //如果没有标记[WorkCommit]，直接执行方法
                invocation.Proceed();
            }

        }
    }
}
