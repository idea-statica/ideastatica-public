#pragma once

#include <string>
#include <vector>

namespace CppFeaExampleApi
{
	/// <summary>
	/// A material of the FEA model. Cross-sections refer to it by id.
	/// </summary>
	struct FeaMaterial
	{
		int Id;
		std::wstring Name;
	};

	/// <summary>
	/// A cross-section of the FEA model, made of one material. Members refer to it by id.
	/// </summary>
	struct FeaCrossSection
	{
		int Id;
		std::wstring Name;
		int MaterialId;
	};

	/// <summary>
	/// A node of the FEA model. Coordinates are in metres.
	/// </summary>
	struct FeaNode
	{
		int Id;
		double X;
		double Y;
		double Z;
	};

	/// <summary>
	/// A straight member of the FEA model, from its start node to its end node.
	/// </summary>
	struct FeaMember
	{
		int Id;
		int StartNodeId;
		int EndNodeId;
		int CrossSectionId;
	};

	/// <summary>
	/// A direction or an offset in the model.
	/// </summary>
	struct FeaVector3D
	{
		double X;
		double Y;
		double Z;
	};

	/// <summary>
	/// The three unit axes of a member.
	/// </summary>
	struct FeaAxes
	{
		FeaVector3D X;
		FeaVector3D Y;
		FeaVector3D Z;
	};

	/// <summary>
	/// Whether the load cases of a group are always there or come and go.
	/// </summary>
	enum class FeaLoadGroupType
	{
		Permanent,
		Variable,
	};

	/// <summary>
	/// A group of load cases of one kind. Load cases refer to it by id.
	/// </summary>
	struct FeaLoadGroup
	{
		int Id;
		std::wstring Name;
		FeaLoadGroupType Type;
	};

	/// <summary>
	/// What causes the loads of a load case.
	/// </summary>
	enum class FeaLoadCaseType
	{
		SelfWeight,
		DeadLoad,
		Snow,
	};

	/// <summary>
	/// A load case: loads that act together. The FEA application has calculated internal forces for each one.
	/// </summary>
	struct FeaLoadCase
	{
		int Id;
		std::wstring Name;
		FeaLoadCaseType Type;
		int LoadGroupId;
	};

	/// <summary>
	/// The limit state a combination is checked for.
	/// </summary>
	enum class FeaLimitState
	{
		Ultimate,
		Serviceability,
	};

	/// <summary>
	/// One load case of a combination and the factor it is multiplied by.
	/// </summary>
	struct FeaCombinationFactor
	{
		int LoadCaseId;
		double Factor;
	};

	/// <summary>
	/// A load combination: the sum of its load cases, each multiplied by its factor.
	/// </summary>
	struct FeaCombination
	{
		int Id;
		std::wstring Name;
		FeaLimitState LimitState;
		std::vector<FeaCombinationFactor> Factors;
	};

	/// <summary>
	/// The internal forces at one point of a member, in the member's own axes. Forces are in kN, moments in kNm.
	/// </summary>
	struct FeaMemberResult
	{
		/// <summary>
		/// The distance from the start of the member, in metres.
		/// </summary>
		double Position;

		double N;
		double Vy;
		double Vz;
		double Mx;
		double My;
		double Mz;
	};
}
