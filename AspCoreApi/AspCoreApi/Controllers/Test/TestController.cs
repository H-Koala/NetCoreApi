using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspCoreApi.Filter.Authorization;
using IServices.ITest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.ParameterModel.Test;
using Model.ViewModel;
using Model.ViewModel.Test;

namespace AspCoreApi.Controllers.Test
{
   /// <summary>
   /// 测试
   /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AuthorizationFilter]
    [ApiExplorerSettings(GroupName = "v1")]
    public class TestController : ControllerBase
    {
        private ITestService testService;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="TestService"></param>
        public TestController(ITestService TestService) {
            testService = TestService;
        }
        /// <summary>
        /// 测试Post 接口
        /// </summary>
        /// <param name="people"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ReturnMessageModel<MInfo>> Test(MPeople people)
        {
            return testService.Test(people);
        }
        /// <summary>
        /// 测试get接口
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<ReturnMessageModel<string>> Test(string name)
        {
            return new ReturnMessageModel<string>();
        }

    }
}
