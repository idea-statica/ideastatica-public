using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using IdeaStatiCa.Plugin.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc
{
    /// <summary>
    /// Creates a dynamic instance of the interface implemented on server to proxy the calls from grpc client service to grpc server.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GrpcReflectionServiceFactory
    {
        public static T CreateInstance<T>(GrpcReflectionClient client) where T:class
        {
            ProxyGenerator generator = new ProxyGenerator();
            return generator.CreateInterfaceProxyWithoutTarget<T>(new ReflectionServiceInterceptor(client));

            //if (!typeof(T).IsInterface || typeof(T).GetCustomAttribute<GrpcReflectionServiceAttribute>() == null)
            //{
            //    throw new ArgumentException($"Type {typeof(T).Name} must be an interface implementing GrpcReflectionServiceAttribute.");
            //} 

            //return (T)Activator.CreateInstance(BuildType(typeof(T)));
        }

        private Type BuildType(Type interfaceType)
        {
            var assemblyName = new AssemblyName($"DynamicAssembly_{Guid.NewGuid():N}");
            var assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
            var typeName = $"{RemoveInterfacePrefix(interfaceType.Name)}_{Guid.NewGuid():N}";
            var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public);

            typeBuilder.AddInterfaceImplementation(interfaceType);

            var methods = interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(w => w.GetCustomAttribute<GrpcReflectionMethodAttribute>() != null);

            foreach(var method in methods)
            {
                BuildMethod(typeBuilder, method);
            }

            var properties = interfaceType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            //BuildProperties(typeBuilder, properties);

            return typeBuilder.CreateType();

            string RemoveInterfacePrefix(string name) => Regex.Replace(name, "^I", string.Empty);
        }

        private void BuildMethod(TypeBuilder typeBuilder, MethodInfo info)
        { 
            MethodBuilder mtbl = typeBuilder.DefineMethod(info.Name, info.Attributes, info.CallingConvention, info.ReturnType, info.GetParameters().Select(x => x.ParameterType).ToArray());
            MethodBody mb = this.GetType().GetMethod("ProxyMethod").GetMethodBody();
            byte[] il = mb.GetILAsByteArray();
            ILGenerator ilg = mtbl.GetILGenerator();
            var opCodes = GetOpCodes(il);
            foreach (var local in mb.LocalVariables)
                ilg.DeclareLocal(local.LocalType);
            for (int i = 0; i < opCodes.Length; ++i)
            {
                if (!opCodes[i].code.HasValue)
                    continue;
                OpCode opCode = opCodes[i].code.Value;
                if (opCode.OperandType == OperandType.InlineBrTarget)
                {
                    ilg.Emit(opCode, BitConverter.ToInt32(il, i + 1));
                    i += 4;
                    continue;
                }
                if (opCode.OperandType == OperandType.ShortInlineBrTarget)
                {
                    ilg.Emit(opCode, il[i + 1]);
                    ++i;
                    continue;
                }
                if (opCode.OperandType == OperandType.InlineType)
                {
                    Type tp = info.Module.ResolveType(BitConverter.ToInt32(il, i + 1), info.DeclaringType.GetGenericArguments(), info.GetGenericArguments());
                    ilg.Emit(opCode, tp);
                    i += 4;
                    continue;
                }
                if (opCode.FlowControl == FlowControl.Call)
                {
                    MethodInfo mi = info.Module.ResolveMethod(BitConverter.ToInt32(il, i + 1)) as MethodInfo;
                    if (mi == info)
                        ilg.Emit(opCode, mtbl);
                    else
                        ilg.Emit(opCode, mi);
                    i += 4;
                    continue;
                }
                ilg.Emit(opCode);
            }
        }

        private OpCodeContainer[] GetOpCodes(byte[] data)
        {
            List<OpCodeContainer> opCodes = new List<OpCodeContainer>();
            foreach (byte opCodeByte in data)
                opCodes.Add(new OpCodeContainer(opCodeByte));
            return opCodes.ToArray();
        }

        public void ProxyMethod(params object[] args)
        {
            Debug.Write(string.Join(", ", args.Select(w => w.ToString())));
        }

        class OpCodeContainer
        {
            public OpCode? code;
            byte data;

            public OpCodeContainer(byte opCode)
            {
                data = opCode;
                try
                {
                    code = (OpCode)typeof(OpCodes).GetFields().First(t => ((OpCode)(t.GetValue(null))).Value == opCode).GetValue(null);
                }
                catch { }
            }
        }


        //private static void BuildProperties(TypeBuilder typeBuilder, IEnumerable<PropertyInfo> properties)
        //{
        //    foreach (var property in properties)
        //    {
        //        BuildProperty(typeBuilder, property);
        //    }
        //}

        //private static PropertyBuilder BuildProperty(TypeBuilder typeBuilder, PropertyInfo property)
        //{
        //    var fieldName = $"<{property.Name}>k__BackingField";

        //    var propertyBuilder = typeBuilder.DefineProperty(property.Name, System.Reflection.PropertyAttributes.None, property.PropertyType, Type.EmptyTypes);

        //    // Build backing-field.
        //    var fieldBuilder = typeBuilder.DefineField(fieldName, property.PropertyType, FieldAttributes.Private);

        //    var getSetAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual;

        //    var getterBuilder = BuildGetter(typeBuilder, property, fieldBuilder, getSetAttributes);
        //    var setterBuilder = BuildSetter(typeBuilder, property, fieldBuilder, getSetAttributes);

        //    propertyBuilder.SetGetMethod(getterBuilder);
        //    propertyBuilder.SetSetMethod(setterBuilder);

        //    return propertyBuilder;
        //}

        //private static MethodBuilder BuildGetter(TypeBuilder typeBuilder, PropertyInfo property, FieldBuilder fieldBuilder, MethodAttributes attributes)
        //{
        //    var getterBuilder = typeBuilder.DefineMethod($"get_{property.Name}", attributes, property.PropertyType, Type.EmptyTypes);
        //    var ilGenerator = getterBuilder.GetILGenerator();

        //    ilGenerator.Emit(OpCodes.Ldarg_0);
        //    ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);

        //    if (property.GetCustomAttribute<NotNullAttribute>() != null)
        //    {
        //        // Build null check
        //        ilGenerator.Emit(OpCodes.Dup);

        //        var isFieldNull = ilGenerator.DefineLabel();
        //        ilGenerator.Emit(OpCodes.Brtrue_S, isFieldNull);
        //        ilGenerator.Emit(OpCodes.Pop);
        //        ilGenerator.Emit(OpCodes.Ldstr, $"{property.Name} isn't set.");

        //        var invalidOperationExceptionConstructor = typeof(InvalidOperationException).GetConstructor(new Type[] { typeof(string) });
        //        ilGenerator.Emit(OpCodes.Newobj, invalidOperationExceptionConstructor);
        //        ilGenerator.Emit(OpCodes.Throw);

        //        ilGenerator.MarkLabel(isFieldNull);
        //    }
        //    ilGenerator.Emit(OpCodes.Ret);

        //    return getterBuilder;
        //}

        //private static MethodBuilder BuildSetter(TypeBuilder typeBuilder, PropertyInfo property, FieldBuilder fieldBuilder, MethodAttributes attributes)
        //{
        //    var setterBuilder = typeBuilder.DefineMethod($"set_{property.Name}", attributes, null, new Type[] { property.PropertyType });
        //    var ilGenerator = setterBuilder.GetILGenerator();

        //    ilGenerator.Emit(OpCodes.Ldarg_0);
        //    ilGenerator.Emit(OpCodes.Ldarg_1);

        //    // Build null check

        //    if (property.GetCustomAttribute<NotNullAttribute>() != null)
        //    {
        //        var isValueNull = ilGenerator.DefineLabel();

        //        ilGenerator.Emit(OpCodes.Dup);
        //        ilGenerator.Emit(OpCodes.Brtrue_S, isValueNull);
        //        ilGenerator.Emit(OpCodes.Pop);
        //        ilGenerator.Emit(OpCodes.Ldstr, property.Name);

        //        var argumentNullExceptionConstructor = typeof(ArgumentNullException).GetConstructor(new Type[] { typeof(string) });
        //        ilGenerator.Emit(OpCodes.Newobj, argumentNullExceptionConstructor);
        //        ilGenerator.Emit(OpCodes.Throw);

        //        ilGenerator.MarkLabel(isValueNull);
        //    }
        //    ilGenerator.Emit(OpCodes.Stfld, fieldBuilder);
        //    ilGenerator.Emit(OpCodes.Ret);

        //    return setterBuilder;
        //}

    }

    internal class ReflectionServiceInterceptor : AsyncInterceptor
    {
        private GrpcReflectionClient client;

        public ReflectionServiceInterceptor(GrpcReflectionClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("Client cannot be null");
            }

            this.client = client;
        }

        protected override async Task<Object> InterceptAsync(object target, MethodBase method, object[] arguments, Func<Task<object>> proceed)
        {
            try
            {
                var returnValue = await client.InvokeMethodAsync<object>(method.Name, arguments);

                return returnValue;
            }
            catch(Exception e)
            {
                throw new InvalidOperationException("Grpc method couldn't be invoked. Check if your server implemnents specified interface", e);
            }
        }

        protected override async Task<object> Intercept(object target, MethodBase method, object[] arguments, Action proceed)
        {
            try
            {
                return await client.InvokeMethodAsync<object>(method.Name, arguments); 
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Grpc method couldn't be invoked. Check if your server implemnents specified interface", e);
            }
        }
    }
}
