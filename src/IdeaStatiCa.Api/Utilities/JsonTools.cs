using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace IdeaStatiCa.Api.Utilities
{
	public static class JsonTools
	{
		public static JsonSerializerSettings CreateIdeaRestJsonSettings()
		{
			JsonSerializerSettings settings = new JsonSerializerSettings();
			settings.SetForIdea();
			return settings;
		}

		/// <summary>
		/// Creates JSON serializer settings for IDEA REST API that safely handles Perst objects
		/// </summary>
		/// <returns>JsonSerializerSettings configured to safely serialize objects inheriting from Persistent</returns>
		public static JsonSerializerSettings CreateIdeaRestJsonSettingsForPerst()
		{
			JsonSerializerSettings settings = new JsonSerializerSettings();
			settings.SetForIdeaWithPerst();
			return settings;
		}

		public static void SetForIdea(this JsonSerializerSettings settings)
		{
			settings.Converters.Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() });
			settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			// Equivalent of PropertyNamingPolicy = CamelCase
			settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

			// Equivalent of PropertyNameCaseInsensitive = false
			settings.MetadataPropertyHandling = MetadataPropertyHandling.Default;

			// serialize type names for polymorphic types
			settings.TypeNameHandling = TypeNameHandling.Auto;
		}

		// method to serialize data suitable for npm package https://www.npmjs.com/package/@ideastatica/scene
		public static void SetFor3DScene(this JsonSerializerSettings settings)
		{
			settings.SetForIdea();
			// Settings required for proper 3D Scene data to process with npm package
			settings.NullValueHandling = NullValueHandling.Ignore;
			settings.TypeNameHandling = TypeNameHandling.None;
		}

		public static void SetForIdeaWithPerst(this JsonSerializerSettings settings)
		{
			settings.SetForIdea();
			settings.Formatting = Formatting.Indented;
			settings.ContractResolver = new PerstSafeContractResolver();
		}
	}

	/// <summary>
	/// Contract resolver that handles serialization of objects inheriting from Perst.Persistent
	/// by excluding properties that can cause serialization issues
	/// </summary>
	internal sealed class PerstSafeContractResolver : CamelCasePropertyNamesContractResolver
	{
		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var property = base.CreateProperty(member, memberSerialization);
			
			if (property.DeclaringType?.Name == "Persistent" &&
				property.DeclaringType?.Namespace == "Perst")
			{
				property.ShouldSerialize = _ => false;
			}

			return property;
		}
	}
}
