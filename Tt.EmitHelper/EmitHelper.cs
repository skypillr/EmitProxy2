using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Tt.EmitHelper
{
    public class EmitHelper
    {
        public ILGenerator _il;
        public EmitHelper(ILGenerator il)
        {
            _il = il;
        }

        public void EmitDeclareLocal(Type type)
        {
            _il.DeclareLocal(type);
        }

        
        /// <summary>
        /// WriteLine
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void EmitCallWriteLine<T>()
        {
            MethodInfo m = typeof(Console).GetMethod(SysMethodSet.WriteLine,new Type[] { typeof(T)});
            _il.Emit(OpCodes.Call, m);
        }

        /// <summary>
        /// ToString
        /// </summary>
        public void EmitCallToString()
        {
            MethodInfo m = typeof(Object).GetMethod(SysMethodSet.ToString, new Type[0]);
            _il.Emit(OpCodes.Callvirt, m);
        }

        /// <summary>
        /// return
        /// </summary>
        public void EmitRet()
        {
            _il.Emit(OpCodes.Ret);
        }

        public void EmitNop()
        {
            _il.Emit(OpCodes.Nop);
        }

        public void EmitLoadInt32(int i)
        {
            _il.Emit(OpCodes.Ldc_I4, i);
        }
        public void EmitLoadInt32_1()
        {
            _il.Emit(OpCodes.Ldc_I4_1);
        }
        public void EmitLoadInt32_0()
        {
            _il.Emit(OpCodes.Ldc_I4_0);
        }
        public void EmitNewArray(Type type)
        {
            _il.Emit(OpCodes.Newarr, type);
        }
        public void EmitDuplicate()
        {
            _il.Emit(OpCodes.Dup);
        }

        public void EmitLoadArgumentByIndex(UInt16 i)
        {
            _il.Emit(OpCodes.Ldarg,i);
        }
        public void EmitLoadArgument_0()
        {
            _il.Emit(OpCodes.Ldarg_0);
        }
        public void EmitBoxValueType(Type type) 
        {
            _il.Emit(OpCodes.Box, type);
        }

        public void EmitSetArrayElementAtIndexWithRef()
        {
            _il.Emit(OpCodes.Stelem_Ref);
        }

        public void EmitStorToLocal(UInt16 position)
        {
            _il.Emit(OpCodes.Stloc, position);
        }
        public void EmitStorToLocal_0()
        {
            _il.Emit(OpCodes.Stloc_0);
        }

        public void EmitCallMethod(MethodInfo methodinfo)
        {
            _il.Emit(OpCodes.Call,
                  methodinfo);
        }

        public void EmitLoadField(FieldInfo fieldInfo)
        {
            _il.Emit(OpCodes.Ldfld, fieldInfo);
        }

        public void EmitCallMethodVirtual(MethodInfo methodInfo)
        {
            _il.Emit(OpCodes.Callvirt, methodInfo);
        }
        public void EmitPop()
        {
            _il.Emit(OpCodes.Pop);
        }

        public void EmitLoadLocalByIndex(UInt16 i)
        {
            _il.Emit(OpCodes.Ldloc, i);
        }

        public void EmitLoadLocal_0()
        {
            _il.Emit(OpCodes.Ldloc_0);
        }

        public void EmitLoadToken(Type type)
        {
            _il.Emit(OpCodes.Ldtoken, type);
        }







    }
   
}
