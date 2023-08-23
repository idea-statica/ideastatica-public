namespace IdeaStatiCa.Plugin
{
	public sealed class Constants
	{
		// the version of the compatible IdeaStatiCa
		public const string IdeaStatiCaVersion = "21.1";

		#region gRPC Constants 
		public const string GrpcControlPortParam = "-grpcPort";
		public const string GrpcDataPort = "-grpcConnectionPort";
		public const string GrpcReflectionErrorException = "GrpcReflectionError";
		public const string BlobStorageId = "blobStorageId";
		public const string ContentId = "contentId";
		#endregion

		public const string IdeaConnectionAppName = "IdeaConnection.exe";
		public const string CheckbotAppName = "IdeaCheckbot.exe";
		public const string ViewerPluginAppName = "IdeaStatiCa.ViewerPlugin.exe";
		public const string AutomationParam = "-automation";
		public const string ProjectParam = "-project";
		public const string LibraryReposPath = "-libReposPath";

		public const string ConnectionChangedEventFormat = "IdeaStatiCaConnectionChanged{0}";
		public const string MemberChangedEventFormat = "IdeaStatiCaMemberChanged{0}";
		public const string ConCalculatorChangedEventFormat = "IdeaStatiCa.ConnHiddenCalculator-{0}";
		public const string ConCalculatorCancelEventFormat = "IdeaStatiCa.ConnHiddenCalculatorCancel-{0}";
		public const string MemHiddenCalcChangedEventFormat = "IdeaStatiCa.MemberHiddenCalculator-{0}";
		public const string MemHiddenCalcCancelEventFormat = "IdeaStatiCa.MemberHiddenCalculatorCancel-{0}";
		public const string DetailChangedEventFormat = "IdeaStatiCaDetailChanged{0}";

		#region BIM Plugin default constants

		public const string DefaultPluginEventName = "IdeaStatiCaBIMPluginEvent";
		public const string DefaultPluginUrlFormat = "net.pipe://localhost/IdeaBIMPlugin{0}";
		public const string DefaultIdeaStaticaAutoUrlFormat = "net.pipe://localhost/IdeaStatiCaAuto{0}";
		public const string ProgressCallbackUrlFormat = "net.pipe://localhost/IdeaStatiCaProgress{0}";

		#endregion BIM Plugin default constants

		#region Member plugin constants

		public const string MemberEventName = "MemberPluginEvent";
		public const string MemberUrlFormat = "net.pipe://localhost/IdeaMember{0}";

		#endregion Member plugin constants

		public const string ConnHiddenCalculatorUrlFormat = "net.pipe://localhost/IdeaStatiCa.ConnHiddenCalculator{0}";

		public const string MemberHiddenCalculatorUrlFormat = "net.pipe://localhost/IdeaStatiCa.MemberHiddenCalculator{0}";

		#region grpc message handlers
		public const int MinGrpcPort = 50000;
		public const int MaxGrpcPort = 50500;
		public const int GRPC_MAX_MSG_SIZE = 100 * 1024 * 1024; // 100MB max
		public const int GRPC_CHUNK_SIZE = 64 * 1024; // 64 KB
		public const string GRPC_REFLECTION_HANDLER_MESSAGE = "Grpc.Handlers.Reflection";
		public const string GRPC_CHECKBOT_HANDLER_MESSAGE = "Grpc.Handlers.CheckBot";
		public const string GRPC_PROJECTCONTENT_HANDLER_MESSAGE = "Grpc.Handlers.ProjContent";
		#endregion
	}
}