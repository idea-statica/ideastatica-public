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
