#if !NET48
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
	public class ServiceContractAttribute : Attribute
	{
		public SessionMode SessionMode { get; set; }
		public Type CallbackContract { get; set; }
	}

	public class OperationContractAttribute : Attribute
	{
		public bool IsOneWay { get; set; }
		public bool AsyncPattern { get; set; }
	}

	public class ErrorHandlerAttribute : Attribute
	{
		public ErrorHandlerAttribute(Type errorHandler)
		{
		}
	}

	//
	// Summary:
	//     Specifies the values available to indicate the support for reliable sessions
	//     that a contract requires or supports.
	public enum SessionMode
	{
		//
		// Summary:
		//     Specifies that the contract supports sessions if the incoming binding supports
		//     them.
		Allowed,
		//
		// Summary:
		//     Specifies that the contract requires a sessionful binding. An exception is thrown
		//     if the binding is not configured to support session.
		Required,
		//
		// Summary:
		//     Specifies that the contract never supports bindings that initiate sessions.
		NotAllowed
	}

	//
	// Summary:
	//     Specifies the number of service instances available for handling calls that are
	//     contained in incoming messages.
	public enum InstanceContextMode
	{
		//
		// Summary:
		//     A new System.ServiceModel.InstanceContext object is created for each session.
		PerSession,
		//
		// Summary:
		//     A new System.ServiceModel.InstanceContext object is created prior to and recycled
		//     subsequent to each call. If the channel does not create a session this value
		//     behaves as if it were System.ServiceModel.InstanceContextMode.PerCall.
		PerCall,
		//
		// Summary:
		//     Only one System.ServiceModel.InstanceContext object is used for all incoming
		//     calls and is not recycled subsequent to the calls. If a service object does not
		//     exist, one is created.
		Single
	}

	//
	// Summary:
	//     Specifies the type of match semantics used by the dispatcher to route incoming
	//     messages to the correct endpoint.
	public enum AddressFilterMode
	{
		//
		// Summary:
		//     Indicates a filter that does an exact match on the address of an incoming message.
		Exact,
		//
		// Summary:
		//     Indicates a filter does the longest prefix matches on the address of an incoming
		//     message.
		Prefix,
		//
		// Summary:
		//     Indicates a filter that matches on any address of an incoming message.
		Any
	}
}
#endif