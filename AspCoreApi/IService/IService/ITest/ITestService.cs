using Model.ParameterModel.Test;
using Model.ViewModel;
using Model.ViewModel.Test;
using System;
using System.Collections.Generic;
using System.Text;

namespace IServices.ITest
{
    /// <summary>
    /// 测试接口
    /// </summary>
    public interface ITestService
    {
        ReturnMessageModel<MInfo> Test(MPeople people);
    }
}
