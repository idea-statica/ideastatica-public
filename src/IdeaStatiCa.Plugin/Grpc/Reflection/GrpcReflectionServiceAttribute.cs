using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc.Reflection
{
    /// <summary>
    /// Determines that interface decorated with this attribtue is a grpc reflection service.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class GrpcReflectionServiceAttribute : Attribute
    {

    }
}
