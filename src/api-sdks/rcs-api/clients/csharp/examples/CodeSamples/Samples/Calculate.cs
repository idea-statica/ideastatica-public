using IdeaStatiCa.Api.RCS.Model;
using IdeaStatiCa.RcsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Calculates all avaliable sections in an existing RCS project.
		/// </summary>
		/// <param name="rcsClient">The connected RCS API Client</param>
		public static async Task Calculate(IRcsApiClient rcsClient)
		{
			string filePath = "Inputs/Project1.IdeaRcs";

			RcsProject rcsProject = await rcsClient.Project.OpenProjectAsync(filePath);

			List<RcsSection> sections = await rcsClient.Section.SectionsAsync(rcsProject.ProjectId);

			Console.WriteLine("Avaliable Sections in Project.");
			foreach (RcsSection section in sections)
			{
				Console.WriteLine($"{section.Id}={section.Name}"); 
			}

			List<RcsSectionResultOverview> summary = await rcsClient.Calculation.CalculateAsync(rcsProject.ProjectId, new RcsCalculationParameters
			{
				Sections = sections.ConvertAll(x => x.Id).ToList()
			});

			foreach (RcsSectionResultOverview result in summary)
			{
				Console.WriteLine("Results for secion Id: {result.SectionId}");
				var overrallItem = result.OverallItems;
				foreach (var item in overrallItem)
				{
					Console.WriteLine($"Check: {item.ResultType}, Result: {item.ResultType}, Value: {item.CheckValue}");
				}
			}
		}
	}
}
