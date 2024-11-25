using IdeaStatiCa.CheckbotPlugin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Provides connection data.
/// </summary>
public interface IConnectionService
{
	/// <summary>
	/// Returns unbalanced forces for specific connection
	/// </summary>
	/// <param name="connection"></param>
	/// <returns>collection of unbalanced forces</returns>
	Task<IReadOnlyCollection<UnbalancedForce>> GetUnbalancedForces(ModelObject connection);
}