using ConsoleApplication1;
using SadasSof.Aspects.Attributes;
using System;
using System.Reflection;

namespace SadasSof.Aspects
{
    //public class TempAssemblyInjection__ProxyIMyclassMyClassTest : IMyclass
    //{
    //    private object target;

    //    private Type iface;

    //    public TempAssemblyInjection__ProxyIMyclassMyClassTest(object obj, Type type)
    //    {
    //        this.target = obj;
    //        this.iface = type;
    //    }

    //    public void MyClassTestInterface()
    //    {
    //        object[] parameters = new object[0];
    //        CodeInjection.InjectHandler(this.target, Helper.GetMethodFromType(this.target.GetType(), MethodBase.GetCurrentMethod()), parameters, Helper.AspectUnion(Helper.GetMethodFromType(this.iface, MethodBase.GetCurrentMethod()).GetCustomAttributes(typeof(AspectAttribute), true)));
    //    }

    //    public void MyClassTestInterface(int num)
    //    {
    //        object[] parameters = new object[]
    //        {
    //            num
    //        };
    //        CodeInjection.InjectHandler(this.target, Helper.GetMethodFromType(this.target.GetType(), MethodBase.GetCurrentMethod()),
    //            parameters,
    //            Helper.AspectUnion(Helper.GetMethodFromType(this.iface, MethodBase.GetCurrentMethod()).GetCustomAttributes(typeof(AspectAttribute), true)));
    //    }
    //}
}
