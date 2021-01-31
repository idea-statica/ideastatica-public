using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaRS.IdeaXML
{
	public class IdeaAddToXMLAttribute : Attribute
	{
		public IdeaAddToXMLAttribute()
		{
		}
	}
	
	public class IdeaXMLIgnoreClassAttribute : Attribute
	{
		public IdeaXMLIgnoreClassAttribute()
		{
		}
	}

	public class IdeaXMLIgnorePropertyAttribute : Attribute
	{
		public IdeaXMLIgnorePropertyAttribute()
		{

		}
	}

	public class IdeaXMLDoNotConvertAsReferenceAttribute : Attribute
	{
		public IdeaXMLDoNotConvertAsReferenceAttribute()
		{

		}
	}

	public class IdeaAddToSimpleModel : Attribute
	{
		public Guid ContainerID { get; internal set; }

		public IdeaAddToSimpleModel(Guid containerID)
		{
			ContainerID = containerID;
		}

		public IdeaAddToSimpleModel()
		{
			ContainerID = Guid.Empty;
		}
	}

	public class IdeaSimpleModelContainerReference : Attribute
	{
		public IdeaSimpleModelContainerReference()
		{
		}
	}

	//NEBO

	public class SimpleModel : Attribute
	{
		public String ModelType { get; internal set; }
		public Guid ContainerID { get; internal set; }

		public SimpleModel(string modelType, Guid containerID)
		{
			ModelType = modelType;
			ContainerID = containerID;
		}

		public SimpleModel(string modelType)
		{
			ModelType = modelType;
			ContainerID = Guid.Empty;
		}
	}
}
