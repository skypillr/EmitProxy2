
#define DEBUG
#define SaveDLL
using System;
using System.Reflection;
using System.Reflection.Emit;
using SadasSof.Aspects.Attributes;
using Tt.EmitHelper;

namespace SadasSof.Aspects
{


    public delegate object MethodCall(object target,
    MethodBase method,
    object[] parameters,
    AspectAttribute[] attributes);

    public class CodeInjection
    {

        public const string assemblyName = "TempAssembly";

        public const string moduleName = "TempModule";

        public const string nameSpaceName = "SadasSof.Aspects";

        public const string dllName = "Test.dll";

        public static string ClassNamePrefix
        {
            get
            {
                string str = nameSpaceName + "." + "Proxy_";
                return str;
            }
        }
        /// <summary>
        /// Create a instance of our external type
        /// </summary>
        /// <param name="target">External type instance</param>
        /// <param name="interfaceType">Decorate interface methods with attributes</param>
        /// <returns>Intercepted type</returns>
        public static object Create(object realTarget, Type interfaceType)
        {
            Type proxyType = EmiProxyType(realTarget.GetType(), interfaceType);

            return Activator.CreateInstance(proxyType, new object[] { realTarget, interfaceType });
        }


        private static TypeBuilder typeBuilder;

        private static FieldBuilder target, iface;

        /// <summary>
        /// Generate proxy type emiting IL code.
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        private static Type EmiProxyType(Type targetType, Type interfaceType)
        {
            string classname = ClassNamePrefix + interfaceType.Name + "_" + targetType.Name;


            AssemblyName myAssemblyName = new AssemblyName();
            myAssemblyName.Name = assemblyName;
            myAssemblyName.Version = new Version(1, 0, 0, 0);

            AppDomain myCurrentDomain = System.Threading.Thread.GetDomain();
            
            //Only save the custom-type dll while debugging
#if SaveDLL && DEBUG
            AssemblyBuilder myAssemblyBuilder = myCurrentDomain.DefineDynamicAssembly(myAssemblyName, AssemblyBuilderAccess.RunAndSave);

            ModuleBuilder modBuilder = myAssemblyBuilder.DefineDynamicModule(moduleName, dllName);
#else
			myAssemblyBuilder = myCurrentDomain.DefineDynamicAssembly(myAssemblyName,AssemblyBuilderAccess.Run);
			ModuleBuilder modBuilder = myAssemblyBuilder.DefineDynamicModule(moduleName);
#endif


            Type type = modBuilder.GetType(classname);


            if (type == null)
            {
                typeBuilder = modBuilder.DefineType(classname,
                    TypeAttributes.Class | TypeAttributes.Public, targetType.BaseType,
                    new Type[] { interfaceType });

                target = typeBuilder.DefineField("target", typeof(object), FieldAttributes.Private);

                iface = typeBuilder.DefineField("iface", typeof(Type), FieldAttributes.Private);

                EmitConstructor(typeBuilder, target, iface);

                //typeBuilder.AddInterfaceImplementation(interfaceType);
                MethodInfo[] methods = interfaceType.GetMethods();

                foreach (MethodInfo m in methods)
                {
                    EmitProxyMethod(m, typeBuilder);
                }


                type = typeBuilder.CreateType();

            }


#if SaveDLL && DEBUG
            myAssemblyBuilder.Save("Test.dll");
#endif

            return type;
        }

        /// <summary>
        /// Generate the method emiting IL Code 
        /// </summary>
        /// <param name="m">External method info</param>
        /// <param name="typeBuilder">TypeBuilder needed to generate proxy type using IL code</param>
        private static void EmitProxyMethod(MethodInfo m, TypeBuilder typeBuilder)
        {
            try
            {
                Type[] paramTypes = Helper.GetParameterTypes(m);
                // public hidebysig newslot virtual final 
                //instance 
                MethodBuilder mb = typeBuilder.DefineMethod(m.Name,
                    MethodAttributes.Public | MethodAttributes .HideBySig| MethodAttributes.NewSlot| MethodAttributes.Final|MethodAttributes.Virtual,

                    m.ReturnType,
                    paramTypes);

                ILGenerator il = mb.GetILGenerator();
                EmitHelper emh = new EmitHelper(il);

                //  .locals init ([0] object[] parameters) 声明局部变量
                emh.EmitDeclareLocal(typeof(object[]));

                emh.EmitNop();
                emh.EmitLoadInt32(paramTypes.Length);
                 
                emh.EmitNewArray(typeof(Object));
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    emh.EmitDuplicate();
                    emh.EmitLoadInt32(i);
                    emh.EmitLoadArgumentByIndex((byte)(i + 1));
                    if (paramTypes[i].IsValueType)
                    {
                        emh.EmitBoxValueType(paramTypes[i]);
                    }
                    emh.EmitSetArrayElementAtIndexWithRef();
                }
                emh.EmitStorToLocal(0);
                
                emh.EmitCallMethod(typeof(CodeInjection).GetProperty("InjectHandler").GetGetMethod());
                emh.EmitLoadArgumentByIndex(0);
          
                emh.EmitLoadField((FieldInfo)target);
                emh.EmitLoadArgumentByIndex(0);
                
                emh.EmitLoadField((FieldInfo)target);
                emh.EmitCallMethodVirtual(typeof(object).GetMethod("GetType", new Type[0]));
                emh.EmitCallMethod(typeof(MethodBase).GetMethod("GetCurrentMethod", new Type[0]));
                emh.EmitCallMethod(typeof(Helper).GetMethod("GetMethodFromType", new Type[2] { typeof(Type), typeof(MethodBase) }));
                emh.EmitLoadLocalByIndex(0);
         
                emh.EmitLoadArgumentByIndex(0);
                 
                emh.EmitLoadField((FieldInfo)iface);
                emh.EmitCallMethod(typeof(MethodBase).GetMethod("GetCurrentMethod", new Type[0]));
                emh.EmitCallMethod(typeof(Helper).GetMethod("GetMethodFromType", new Type[2] { typeof(Type), typeof(MethodBase) }));
                emh.EmitLoadToken(typeof(AspectAttribute));
                emh.EmitCallMethod(typeof(Type).GetMethod("GetTypeFromHandle", new Type[1] { typeof(RuntimeTypeHandle) }));
                emh.EmitLoadInt32(1);
                 
                emh.EmitCallMethodVirtual(typeof(MemberInfo).GetMethod("GetCustomAttributes", new Type[2] { typeof(Type), typeof(bool) }));
                emh.EmitCallMethod(typeof(Helper).GetMethod("AspectUnion", new Type[1] { typeof(object[]) }));
                emh.EmitCallMethodVirtual(typeof(MethodCall).GetMethod("Invoke", new Type[4] { typeof(object), typeof(MethodBase), typeof(object[]), typeof(AspectAttribute[]) }));
                emh.EmitPop();
                emh.EmitRet();
                

                typeBuilder.DefineMethodOverride(mb, m);
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        /// <summary>
        /// Generate the contructor of our proxy type
        /// </summary>
        /// <param name="typeBuilder">TypeBuilder needed to generate proxy type using IL code</param>
        /// <param name="target">Proxy type target</param>
        /// <param name="iface">Proxy type interface </param>
        private static void EmitConstructor(TypeBuilder typeBuilder, FieldBuilder target, FieldBuilder iface)
        {


            Type objType = Type.GetType("System.Object");
            ConstructorInfo objCtor = objType.GetConstructor(new Type[0]);

            ConstructorBuilder pointCtor = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new Type[] { typeof(object), typeof(Type) });
            ILGenerator ctorIL = pointCtor.GetILGenerator();


            ctorIL.Emit(OpCodes.Ldarg_0);


            ctorIL.Emit(OpCodes.Call, objCtor);


            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_1);
            ctorIL.Emit(OpCodes.Stfld, target);


            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_2);
            ctorIL.Emit(OpCodes.Stfld, iface);

            ctorIL.Emit(OpCodes.Ret);
        }


        public static MethodCall InjectHandler
        {
            get { return new MethodCall(InjectHandlerMethod); }
        }


        /// <summary>
        /// Injection handler
        /// </summary>
        /// <param name="target">Target type witch will be intercepted</param>
        /// <param name="method">Methot to intercept</param>
        /// <param name="parameters">Addtional parameters</param>
        /// <param name="attributes">Attributes decore</param>
        /// <returns></returns>
        public static object InjectHandlerMethod(object target,
                                                 MethodBase method,
                                                 object[] parameters,
                                                 AspectAttribute[] attributes)
        {

            object returnValue = null;

            foreach (AspectAttribute b in attributes)
                if (b is BeforeAttribute)
                    b.Action(target, method, parameters, null);

            try
            {
                Type[] typPs =
                Helper.GetParameterTypes((MethodInfo)method);
                returnValue =
                   target.GetType().GetMethod(method.Name, typPs).Invoke(target, parameters);
            }
            catch (Exception ex)
            {
                foreach (AspectAttribute b in attributes)
                    if (b is LogExceptionAttribute)
                        b.Action(target, method, parameters, ex);
                throw;
            }


            foreach (AspectAttribute a in attributes)
                if (a is AfterAttribute)
                    a.Action(target, method, parameters, returnValue);

            return returnValue;
        }
    }
}
