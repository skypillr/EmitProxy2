
using SadasSof.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
 
namespace ConsoleApplication1
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //var x = new TempAssemblyInjection__ProxyIMyclassMyClassTest(new MyClassTest(), typeof(IMyclass));
            ////x.MyClassTestInterface();
            var my = (IMyclass)CodeInjection.Create(new MyClassTest(), typeof(IMyclass));
            MethodInfo mf =
            my.GetType().GetMethod("MyClassTestInterface", new Type[0]);
            mf.Invoke(my, null);
            //Console.WriteLine(str);
            my.MyClassTestInterface();

            //CodeInjection.InjectHandler(this.target, 
            //    Helper.GetMethodFromType(this.target.GetType(), MethodBase.GetCurrentMethod()), 
            //    parameters,
            //    Helper.AspectUnion(Helper.GetMethodFromType(this.iface, MethodBase.GetCurrentMethod()).GetCustomAttributes(typeof(AspectAttribute), true)));

            Console.Read();
        }
    }

 

}
