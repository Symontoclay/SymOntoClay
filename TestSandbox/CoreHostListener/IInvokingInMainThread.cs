using System;
using System.Collections.Generic;
using System.Text;

namespace TestSandbox.CoreHostListener
{
    public interface IInvokingInMainThread
    {
        void SetInvocableObj(IInvocableInMainThreadObj invokableObj);
    }
}
