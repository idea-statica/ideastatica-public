using ConnectionIomGenerator.Model;
using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.Plugin;

namespace ConnectionIomGenerator.Service
{
	/// <summary>
	/// Generates IDEA Open Model (IOM) containers for steel connections from simplified connection input definitions.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="IomGenerator"/> class orchestrates the complete process of converting high-level connection 
	/// definitions into the IDEA Open Model format, which is the standard data exchange format for IDEA StatiCa applications.
	/// It serves as the main entry point for IOM generation and coordinates multiple components:
	/// </para>
	/// <list type="number">
	/// <item><description><see cref="FeaGenerator"/> - Creates the Finite Element Analysis model from connection parameters</description></item>
	/// <item><description><see cref="FeaModelWrapper"/> - Adapts the FEA model to the IDEA BIM API interface</description></item>
	/// <item><description><see cref="BimImporter"/> - Imports the FEA model and converts it to IDEA Open Model format</description></item>
	/// <item><description>Result generators - Creates load cases and internal forces when loading is provided</description></item>
	/// </list>
	/// <para><b>Generation Workflow:</b></para>
	/// <list type="number">
	/// <item><description><b>FEA Model Generation</b> - Creates nodes, elements, segments, and members using <see cref="FeaGenerator"/></description></item>
	/// <item><description><b>Model Wrapping</b> - Wraps FEA model to implement <c>IIdeaModel</c> interface</description></item>
	/// <item><description><b>Project Mapping</b> - Creates in-memory project for ID mapping between FEA and IOM entities</description></item>
	/// <item><description><b>BIM Import</b> - Uses <see cref="BimImporter"/> to convert FEA model to OpenModel</description></item>
	/// <item><description><b>Loading Assignment</b> - If loading is provided, creates load cases and assigns internal forces to elements</description></item>
	/// <item><description><b>Result Assembly</b> - Packages OpenModel and OpenModelResult into <see cref="OpenModelContainer"/></description></item>
	/// </list>
	/// <para><b>ID Mapping:</b></para>
	/// <para>
	/// During the BIM import process, entity IDs are transformed from FEA model IDs to OpenModel IDs.
	/// The generator maintains bidirectional mapping through an in-memory project to correlate:
	/// </para>
	/// <list type="bullet">
	/// <item><description>Load cases: Original ID → OpenModel load case ID</description></item>
	/// <item><description>Members: Original member ID → OpenModel member ID</description></item>
	/// <item><description>Elements: Original element ID → OpenModel element ID</description></item>
	/// </list>
	/// <para><b>Loading Model:</b></para>
	/// <para>
	/// When <c>loadingInput</c> is provided, the generator creates a complete result model with:
	/// </para>
	/// <list type="bullet">
	/// <item><description><b>Load Cases</b> - One <c>ResultOnMembers</c> per load case from input</description></item>
	/// <item><description><b>Internal Forces</b> - Applied to element ends (positions 0 and 1) based on load impulses</description></item>
	/// <item><description><b>Force Components</b> - N, Vy, Vz, Mx, My, Mz from corresponding load impulse data</description></item>
	/// <item><description><b>Default Values</b> - Missing load impulses default to N=1 to ensure connection generation</description></item>
	/// </list>
	/// <para><b>Error Handling:</b></para>
	/// <para>
	/// The generator uses defensive programming with try-catch blocks around ID parsing and loading assignment.
	/// Invalid inputs are logged as warnings but don't stop the generation process. Critical errors during
	/// FEA generation or BIM import are logged and re-thrown to the caller.
	/// </para>
	/// <para><b>Design Code:</b></para>
	/// <para>
	/// The generator uses <see cref="CountryCode.ECEN"/> (Eurocode EN) as the default design code for 
	/// connection import. This determines the design rules and checks applied to the connection.
	/// </para>
	/// </remarks>
	/// <example>
	/// <para>Basic IOM generation without loading:</para>
	/// <code language="csharp">
	/// var logger = LoggerProvider.GetLogger("MyApp");
	/// var generator = new IomGenerator(logger);
	/// 
	/// var input = new ConnectionInput
	/// {
	///     Material = "S355",
	///     Members = new List&lt;MemberInput&gt;
	///   {
	///         new MemberInput { Id = 1, Name = "Column", CrossSection = "HEB300", IsContinuous = true },
	///         new MemberInput { Id = 2, Name = "Beam", CrossSection = "IPE200", IsContinuous = false }
	///     }
	/// };
	/// 
	/// var container = await generator.GenerateIomAsync(input, null);
	/// 
	/// // container.OpenModel contains the connection geometry
	/// // container.OpenModelResult is null (no loading)
	/// Console.WriteLine($"Generated {container.OpenModel.Member1D.Count} members");
	/// </code>
	/// <para>IOM generation with loading:</para>
	/// <code language="csharp">
	/// var loadingInput = new LoadingInput
	/// {
	///     LoadCases = new List&lt;LoadCase&gt;
	///     {
	/// new LoadCase
	///     {
	///             Id = 1,
	///        Name = "DL",
	///         LoadImpulses = new List&lt;LoadImpulse&gt;
	///             {
	///    new LoadImpulse { MemberId = 1, Position = 0, N = 100, Vy = 10 },
	///       new LoadImpulse { MemberId = 1, Position = 1, N = -100, Vy = -10 },
	///     new LoadImpulse { MemberId = 2, Position = 0, N = 50, Mz = 25 }
	///    }
	///}
	///     }
	/// };
	/// 
	/// var container = await generator.GenerateIomAsync(input, loadingInput);
	/// 
	/// // container.OpenModel contains geometry
	/// // container.OpenModelResult contains load cases and internal forces
	/// Console.WriteLine($"Load cases: {container.OpenModelResult.ResultOnMembers.Count}");
	/// </code>
	/// <para>Using the generated IOM with Connection API:</para>
	/// <code language="csharp">
	/// // Serialize to XML
	/// string iomXml = IdeaRS.OpenModel.Tools.OpenModelContainerToXml(container);
	/// 
	/// // Send to Connection API
	/// using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(iomXml)))
	/// {
	///     var project = await connectionApi.Project.ImportIOMAsync(stream);
	/// Console.WriteLine($"Project created with {project.Connections.Count} connections");
	/// }
	/// </code>
	/// </example>
	public class IomGenerator : IIomGenerator
	{
		private readonly IPluginLogger _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="IomGenerator"/> class.
		/// </summary>
		/// <param name="logger">
		/// The logger instance used to log diagnostic and operational information throughout the IOM generation process.
		/// Logs include debug messages for each generation phase, warnings for invalid inputs, and errors for critical failures.
		/// </param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is null.</exception>
		public IomGenerator(IPluginLogger logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Generates an IDEA Open Model container asynchronously from connection input and optional loading data.
		/// </summary>
		/// <param name="input">
		/// The connection definition containing material, members, and their geometric properties.
		/// Defines the structural configuration of the connection including member directions, cross-sections, and continuity.
		/// </param>
		/// <param name="loadingInput">
		/// Optional loading definition containing load cases and load impulses. When provided, the generator creates
		/// a complete result model with internal forces applied to member elements. Can be null for geometry-only generation.
		/// </param>
		/// <returns>
		/// A task that represents the asynchronous operation. The task result contains an <see cref="OpenModelContainer"/> with:
		/// <list type="bullet">
		/// <item><description><c>OpenModel</c> - The geometric model including nodes, members, cross-sections, and connection points</description></item>
		/// <item><description><c>OpenModelResult</c> - The result model with load cases and internal forces (null if <paramref name="loadingInput"/> is null)</description></item>
		/// </list>
		/// </returns>
		/// <exception cref="Exception">
		/// Thrown when FEA model generation or BIM import fails. The original exception is logged and re-thrown.
		/// Common failure scenarios include invalid cross-section names, incompatible member configurations, or BIM import errors.
		/// </exception>
		/// <remarks>
		/// <para><b>Execution Flow:</b></para>
		/// <list type="number">
		/// <item><description><b>FEA Generation</b> - <see cref="FeaGenerator"/> creates the structural model from <paramref name="input"/></description></item>
		/// <item><description><b>Model Wrapping</b> - FEA model is wrapped in <c>FeaModelWrapper</c> to implement BIM API interfaces</description></item>
		/// <item><description><b>Project Creation</b> - <c>InMemoryProject</c> provides ID mapping between FEA and OpenModel entities</description></item>
		/// <item><description><b>BIM Import</b> - <see cref="BimImporter"/> converts FEA model to OpenModel using Eurocode EN</description></item>
		/// <item><description><b>Loading Processing</b> (if <paramref name="loadingInput"/> is not null):
		///     <list type="bullet">
		///     <item><description>Maps load case IDs from input to OpenModel load case IDs</description></item>
		///     <item><description>For each element, retrieves corresponding load impulses by member ID and position</description></item>
		/// <item><description>Creates <see cref="ResultOfInternalForces"/> with N, Vy, Vz, Mx, My, Mz components</description></item>
		///     <item><description>Applies forces at element ends (positions 0 and 1)</description></item>
		///     <item><description>Uses default N=1 if no load impulse is found (prevents zero impulse omission)</description></item>
		///     </list>
		/// </description></item>
		/// <item><description><b>Result Assembly</b> - Combines OpenModel and OpenModelResult into <see cref="OpenModelContainer"/></description></item>
		/// </list>
		/// <para><b>ID Mapping Details:</b></para>
		/// <para>
		/// The BIM importer assigns new IDs to entities during import. The generator uses <c>project.GetBimApiId()</c>
		/// to retrieve original IDs in format "type-id" (e.g., "LoadCase-1", "Member-2"). The <see cref="ParseBimId"/>
		/// helper extracts the numeric ID for correlation with <paramref name="input"/> and <paramref name="loadingInput"/>.
		/// </para>
		/// <para><b>Loading Model Structure:</b></para>
		/// <code>
		/// OpenModelResult
		///   └─ ResultOnMembers (one per load case)
		///        └─ ResultOnMember (one per element)
		///        └─ ResultOnSection (positions 0 and 1)
		///       └─ ResultOfInternalForces (N, Vy, Vz, Mx, My, Mz)
		/// </code>
		/// <para><b>Error Handling Strategy:</b></para>
		/// <list type="bullet">
		/// <item><description><b>Critical Errors</b> - FEA generation and BIM import failures are logged and re-thrown</description></item>
		/// <item><description><b>ID Mapping Errors</b> - Invalid load case or member IDs are logged as warnings; generation continues</description></item>
		/// <item><description><b>Load Impulse Errors</b> - Missing or invalid impulses are logged; default N=1 is used</description></item>
		/// </list>
		/// <para><b>Performance Considerations:</b></para>
		/// <para>
		/// The method is asynchronous but currently performs all operations synchronously, returning a completed task.
		/// This design allows for future asynchronous operations (e.g., database access, file I/O) without breaking the API.
		/// </para>
		/// <para><b>Thread Safety:</b></para>
		/// <para>
		/// This method is NOT thread-safe. Create separate <see cref="IomGenerator"/> instances for concurrent operations.
		/// The FEA model, BIM importer, and in-memory project maintain internal state that is not synchronized.
		/// </para>
		/// </remarks>
		/// <example>
		/// <para>Generate IOM with multi-load-case scenario:</para>
		/// <code language="csharp">
		/// var input = new ConnectionInput
		/// {
		///     Material = "S235",
		///     Members = new List&lt;MemberInput&gt;
		///     {
		///         new MemberInput 
		///    { 
		///       Id = 1, 
		/// CrossSection = "HEB200",
		///     Direction = 0, 
		///           Pitch = 90,
		///         IsContinuous = true 
		///      }
		///}
		/// };
		/// 
		/// var loading = new LoadingInput
		/// {
		///     LoadCases = new List&lt;LoadCase&gt;
		///     {
		///         new LoadCase 
		///         { 
		///             Id = 1, 
		///     Name = "Dead Load",
		///           LoadImpulses = new List&lt;LoadImpulse&gt;
		///             {
		///     new LoadImpulse { MemberId = 1, Position = 0, N = 150 },
		///       new LoadImpulse { MemberId = 1, Position = 1, N = -150 }
		///     }
		///         },
		///         new LoadCase 
		///         { 
		///     Id = 2, 
		///             Name = "Live Load",
		///    LoadImpulses = new List&lt;LoadImpulse&gt;
		///             {
		///        new LoadImpulse { MemberId = 1, Position = 0, N = 100, Mz = 50 }
		///      }
		///   }
		///     }
		/// };
		/// 
		/// var generator = new IomGenerator(logger);
		/// var container = await generator.GenerateIomAsync(input, loading);
		/// 
		/// // Verify results
		/// Console.WriteLine($"Members: {container.OpenModel.Member1D.Count}");
		/// Console.WriteLine($"Load Cases: {container.OpenModelResult.ResultOnMembers.Count}");
		/// 
		/// foreach (var resOnMembers in container.OpenModelResult.ResultOnMembers)
		/// {
		///     Console.WriteLine($"Results for {resOnMembers.Members.Count} elements");
		/// }
		/// </code>
		/// </example>
		public async Task<OpenModelContainer> GenerateIomAsync(ConnectionInput input, LoadingInput? loadingInput)
		{
			_logger.LogInformation("Starting IOM generation");

			var res = new OpenModelContainer();

			try
			{
				// Generate FEA model
				_logger.LogDebug("Generating FEA model");
				var generator = new FeaGenerator();
				var feaModel = generator.Generate(input, loadingInput);

				// Wrap FeaModel to implement IIdeaModel
				_logger.LogDebug("Creating FEA model wrapper");
				var feaModelWrapper = new FeaModelWrapper(feaModel);

				// Create a simple in-memory project for ID mapping
				var project = new InMemoryProject();

				// Create BimImporter
				_logger.LogDebug("Creating BIM importer");
				var bimImporter = BimImporter.Create(
					ideaModel: feaModelWrapper,
					project: project,
					logger: _logger,
					geometryProvider: null, // Will use DefaultGeometryProvider
					configuration: new BimImporterConfiguration()
				);

				// Import connections and populate OpenModel
				_logger.LogDebug("Importing connections");
				var modelBim = bimImporter.ImportConnections(CountryCode.ECEN);

				// Assign the imported OpenModel to the container
				res.OpenModel = modelBim.Model;

				// add loading if exists
				if(loadingInput != null)
				{
					IdeaRS.OpenModel.Result.OpenModelResult openModelResult = new IdeaRS.OpenModel.Result.OpenModelResult();

					///openModelResult.
					var resOnMembers = new IdeaRS.OpenModel.Result.ResultOnMembers();
					
					foreach (var lc in res.OpenModel.LoadCase)
					{
						LoadCase? feaLc = null;

						try
						{
							// convert id from iom to original Id in input
							var loadCaseBimId = project.GetBimApiId(lc.Id);
							int feaLcId = ParseBimId(loadCaseBimId);
							// find loading in loadingInput
							feaLc = loadingInput.LoadCases.FirstOrDefault(l => l.Id == feaLcId);

						}
						catch(Exception ex)
						{
							_logger.LogWarning($"Invalid input for Loadcase {lc.Id}", ex);
						}

						resOnMembers.Loading = new IdeaRS.OpenModel.Result.Loading(IdeaRS.OpenModel.Result.LoadingType.LoadCase, lc.Id);

						foreach (var mem1D in res.OpenModel.Member1D)
						{
							Model.Member? member = null;
							try
							{
								var memBimId = project.GetBimApiId(mem1D.Id);
								int feaMemMemId = ParseBimId(memBimId);
								member = input.Members.FirstOrDefault(m => m.Id == feaMemMemId);
							}
							catch (Exception ex)
							{
								_logger.LogWarning($"Invalid input for Member {mem1D.Id}", ex);
							}

							List<LoadImpulse> loadImpulsesForMember = null;
							if (feaLc != null && member != null)
							{
								loadImpulsesForMember = feaLc.LoadImpulses.Where(i => i.MemberId == member.Id).ToList();
							}

							// there should be one element for ended member and two elements for continuous member
							int elmInx = 0;
							foreach (var refEl in mem1D.Elements1D)
							{
								var element1D = refEl.Element;
								var elBimId = project.GetBimApiId(refEl.Id);

								int feaElMemId = ParseBimId(elBimId);

								var resOnMember = new ResultOnMember();
								resOnMember.ResultType = ResultType.InternalForces;
								resOnMember.Member = new IdeaRS.OpenModel.Result.Member(MemberType.Element1D, element1D.Id);

								ResultOfInternalForces intForce1 = new ResultOfInternalForces() { N = 1}; // Do not generate a zero impulse; it will be omitted when the connection is created.
								if(loadImpulsesForMember != null)
								{
									try
									{
										var inpulseForElement = loadImpulsesForMember[elmInx];
										intForce1.N = inpulseForElement.N;
										intForce1.Qy = inpulseForElement.Vy;
										intForce1.Qz = inpulseForElement.Vz;
										intForce1.Mx = inpulseForElement.Mx;
										intForce1.My = inpulseForElement.My;
										intForce1.Mz = inpulseForElement.Mz;
									}
									catch (Exception ex)
									{
										_logger.LogWarning($"Invalid load impulses for member {mem1D.Id}", ex);
									}
								}
								

								List<SectionResultBase> forcesList = new List<SectionResultBase>() { intForce1 };

								ResultOnSection resInSection0 = new ResultOnSection() { AbsoluteRelative = AbsoluteRelative.Relative, Position = 0, Results = forcesList };
								ResultOnSection resInSection1 = new ResultOnSection() { AbsoluteRelative = AbsoluteRelative.Relative, Position = 1, Results = forcesList };
								//forcesList.Add(resInSection0);
								//resOnMember.Results = new ResultOnSection
								resOnMember.Results.Add(resInSection0);
								resOnMember.Results.Add(resInSection1);
								resOnMembers.Members.Add(resOnMember);
								elmInx++;
							}
						}
					}

							//resOnMember.
						

					openModelResult.ResultOnMembers.Add(resOnMembers);
					res.OpenModelResult = openModelResult;
				}

				_logger.LogInformation($"IOM generation completed successfully with {res.OpenModel?.Member1D?.Count ?? 0} members");
			}
			catch (Exception ex)
			{
				_logger.LogError("Failed to generate IOM", ex);
				throw;
			}

			return await Task.FromResult(res);
		}

		/// <summary>
		/// Parses the BIM API identifier to extract the original numeric ID.
		/// </summary>
		/// <param name="bimApiId">
		/// The BIM API identifier in format "EntityType-Id" (e.g., "LoadCase-1", "Member-2").
		/// This format is created by the in-memory project during BIM import.
		/// </param>
		/// <returns>
		/// The numeric ID extracted from the BIM API identifier.
		/// </returns>
		/// <exception cref="FormatException">
		/// Thrown when <paramref name="bimApiId"/> doesn't contain a hyphen or the ID portion is not a valid integer.
		/// </exception>
		/// <remarks>
		/// <para>
		/// This helper method is used to correlate OpenModel entity IDs back to original FEA model or input IDs.
		/// During BIM import, entities receive new IDs in the OpenModel, but the in-memory project maintains
		/// a mapping using the "EntityType-OriginalId" format.
		/// </para>
		/// <para><b>ID Format Examples:</b></para>
		/// <list type="bullet">
		/// <item><description>"LoadCase-1" → 1</description></item>
		/// <item><description>"Member-42" → 42</description></item>
		/// <item><description>"Element-123" → 123</description></item>
		/// </list>
		/// <para><b>Usage Context:</b></para>
		/// <para>
		/// Called when mapping OpenModel entities to original input data:
		/// </para>
		/// <list type="bullet">
		/// <item><description>Load cases: To find corresponding <see cref="LoadCase"/> in <c>LoadingInput</c></description></item>
		/// <item><description>Members: To find corresponding <see cref="Model.Member"/> in <c>ConnectionInput</c></description></item>
		/// <item><description>Elements: To correlate with load impulse positions</description></item>
		/// </list>
		/// </remarks>
		/// <example>
		/// <code language="csharp">
		/// string bimId = project.GetBimApiId(loadCaseId);  // Returns "LoadCase-5"
		/// int originalId = ParseBimId(bimId);              // Returns 5
		/// var loadCase = loadingInput.LoadCases.FirstOrDefault(lc => lc.Id == originalId);
		/// </code>
		/// </example>
		static int ParseBimId(string bimApiId)
		{
			int inx = bimApiId.IndexOf("-");
			string parsedId = bimApiId.Substring(inx+1, bimApiId.Length - inx - 1);
			return Int32.Parse(parsedId);
		}
	}
}
