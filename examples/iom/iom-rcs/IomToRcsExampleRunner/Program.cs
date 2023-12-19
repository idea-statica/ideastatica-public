using IdeaRS.OpenModel;
using IomToRcsExamples;
using IdeaStatiCa.RcsClient;
using System.Text;
using System.Xml.Serialization;
using System;
using Microsoft.VisualBasic;
using System.Xml;
using IdeaStatiCa.RcsClient.Factory;
using IdeaStatiCa.RcsClient.Client;

namespace IomToRcsExampleRunner
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			//Lets Create the Open Model 
			OpenModel openModel = RcsExampleBuilder.BuildExampleModel(RcsExampleBuilder.Example.ReinforcedBeam);

			openModel.SaveToXmlFile("IomToRcsExampleRunner.xml");

			string directoryPath = "C:\\Program Files\\IDEA StatiCa\\StatiCa 23.0\\net6.0-windows";

			RcsClientFactory rcsClientFactory = new RcsClientFactory(directoryPath);

			RcsApiClient? client = await rcsClientFactory.CreateRcsApiClient() as RcsApiClient;

			//client.OpenProjectAsync();

			#region SaveIOMtoRCS-TODO


			//placeholder for some example code which will create an RcsFile from selecting one of the Rcs Projects. 

			/*
						//## Results
						In the followed example there is way how to run the check and the get results.Results are stored in the object with considered values for each assessment.

			```csharp
						//Creating instance of Rcs controller
						var rcsController = new IdeaStatiCa.RcsController.IdeaRcsController();
						System.Diagnostics.Debug.Assert(rcsController != null);
						//Assert.IsNotNull(rcsController);

						//Open rcs project from IOM
						IdeaRS.OpenModel.Message.OpenMessages messages;
						var ok = rcsController.OpenIdeaProjectFromIdeaOpenModel(openModel, "Column", out messages);
						System.Diagnostics.Debug.Assert(ok);

						rcsController.SaveAsIdeaProjectFile(fileName);

						//Calculate project
						ok = rcsController.Calculate(new List<int>() { singleCheckSection.Id
				});
						System.Diagnostics.Debug.Assert(ok);

						//gets the results
						var result = rcsController.GetResultOnSection(null);
						System.Diagnostics.Debug.Assert(result != null);

						// Storing to standard xml file
						XmlSerializer xs = new XmlSerializer(typeof(List<IdeaRS.OpenModel.Concrete.CheckResult.SectionConcreteCheckResult>));

						Stream fs = new FileStream(fileName, FileMode.Create);
						XmlTextWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
						writer.Formatting = Formatting.Indented;
						// Serialize using the XmlTextWriter.
						xs.Serialize(writer, result);
						writer.Close();
						fs.Close();

						var sectionResult = result.FirstOrDefault(it => it.SectionId == singleCheckSection.Id);
						System.Diagnostics.Debug.Assert(result != null);
						foreach (var extremeResult in sectionResult.ExtremeResults)
						{
							var overalResult = extremeResult.Overall;
							foreach (var check in overalResult.Checks)
							{
								System.Diagnostics.Debug.WriteLine("{0} - {1} - {2}", check.ResultType, check.Result, check.CheckValue);
							}

							foreach (var checkResult in extremeResult.CheckResults)
							{
								var checkType = checkResult.ResultType;
								foreach (var checkResult1 in checkResult.CheckResults)
								{
									var res = checkResult1.Result;

									switch (checkResult.ResultType)
									{
										case IdeaRS.OpenModel.Concrete.CheckResult.CheckResultType.Capacity:
											var resultCapacity = checkResult1 as IdeaRS.OpenModel.Concrete.CheckResult.ConcreteULSCheckResultDiagramCapacityEc2;
											var fu1 = resultCapacity.Fu1;
											var fu2 = resultCapacity.Fu2;
											break;

										case IdeaRS.OpenModel.Concrete.CheckResult.CheckResultType.Interaction:
											var resultInteraction = checkResult1 as IdeaRS.OpenModel.Concrete.CheckResult.ConcreteULSCheckResultInteractionEc2;
											var checkVT = resultInteraction.CheckValueShearAndTorsion;
											var checkVTB = resultInteraction.CheckValueShearTorsionAndBending;
											break;
									}

									if (checkResult1.NonConformities.Count > 0)
									{
										var issues = rcsController.GetNonConformityIssues(checkResult1.NonConformities.Select(it => it.Guid).ToList());
										foreach (var issue in issues)
										{
											System.Diagnostics.Debug.WriteLine(issue.Description);
										}
									}
								}




						*/

			#endregion

		}

	}
}