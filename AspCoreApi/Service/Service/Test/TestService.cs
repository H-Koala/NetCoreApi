using Autofac.Extras.DynamicProxy;
using IServices.ICommon.IRepository.IDapper;
using IServices.ITest;
using Microsoft.Extensions.Configuration;
using Model.ParameterModel.Test;
using Model.ViewModel;
using Model.ViewModel.Test;
using Services.Interceptor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Test
{
    /// <summary>
    /// 测试类
    /// </summary>
    [Intercept(typeof(CustomAOPInterception))]
    public class TestService : ITestService
    {
        private IConfiguration configuration;
        private ISqlDapperService _SqlDapper;
        /// <summary>
        /// 构造函数注入ISqlDapperService
        /// </summary>
        /// <param name="Configuration"></param>
        /// <param name="SqlDapperService"></param>
        public TestService(IConfiguration Configuration, ISqlDapperService SqlDapperService) {
            configuration = Configuration;
            _SqlDapper = SqlDapperService;//数据库操作接口，由AutoFac 创建
        }
        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="people"></param>
        /// <returns></returns>
        public ReturnMessageModel<MInfo> Test(MPeople people) {
            MInfo info = new MInfo();
            info.name = people.name;
            info.age = people.age;
            ReturnMessageModel<MInfo> returnMessageModel= new ReturnMessageModel<MInfo>();
            returnMessageModel.Status = 0;
            returnMessageModel.Message = "Success";
            returnMessageModel.Data = info;
            return returnMessageModel;
        }
    }
}
