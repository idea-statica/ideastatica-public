using Dlubal.RSTAB8;
using IdeaRstabPlugin.Factories;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;

namespace IdeaRstabPlugin.BimApi
{
	internal class RstabSectionResult : IIdeaSectionResult
	{
		public IIdeaLoading Loading => _objectFactory.GetLoading(_loading);

		public IIdeaResultData Data => _internalForces;

		public double Position { get; }

		private readonly InternalForcesData _internalForces;
		private readonly IObjectFactory _objectFactory;
		private readonly Loading _loading;

		public RstabSectionResult(IObjectFactory objectFactory, Member member, MemberForces memberForces, Loading loading, bool isCSDownwards)
		{
			Position = memberForces.Location / member.Length;

			if (Position > 1.0)
			{
				Position = 1;
			}

			_objectFactory = objectFactory;
			_loading = loading;
			_internalForces = ConvertForces(new InternalForcesData()
			{
				Mx = memberForces.Moments.X,
				My = memberForces.Moments.Y,
				Mz = memberForces.Moments.Z,
				N = memberForces.Forces.X,
				Qy = memberForces.Forces.Y,
				Qz = memberForces.Forces.Z,
			}, isCSDownwards);
		}

		private RstabSectionResult(RstabSectionResult other, double pos, InternalForcesData internalForces)
		{
			Position = pos;
			_objectFactory = other._objectFactory;
			_loading = other._loading;
			_internalForces = internalForces;
		}

		public void Add(RstabSectionResult other)
		{
			_internalForces.N += other._internalForces.N;
			_internalForces.Qy += other._internalForces.Qy;
			_internalForces.Qz += other._internalForces.Qz;
			_internalForces.Mx += other._internalForces.Mx;
			_internalForces.My += other._internalForces.My;
			_internalForces.Mz += other._internalForces.Mz;
		}

		public static RstabSectionResult Interpolate(RstabSectionResult a, RstabSectionResult b, double pos)
		{
			double d = (pos - a.Position) / (b.Position - a.Position);

			InternalForcesData ifA = a._internalForces;
			InternalForcesData ifB = b._internalForces;

			InternalForcesData internalForces = new InternalForcesData()
			{
				N = ifA.N + d * (ifB.N - ifA.N),
				Qy = ifA.Qy + d * (ifB.Qy - ifA.Qy),
				Qz = ifA.Qz + d * (ifB.Qz - ifA.Qz),
				Mx = ifA.Mx + d * (ifB.Mx - ifA.Mx),
				My = ifA.My + d * (ifB.My - ifA.My),
				Mz = ifA.Mz + d * (ifB.Mz - ifA.Mz),
			};

			return new RstabSectionResult(a, pos, internalForces);
		}

		private static InternalForcesData ConvertForces(InternalForcesData forces, bool isCSDownwards)
		{
			if (isCSDownwards)
			{
				forces.Qy *= -1.0;
				forces.Qz *= -1.0;
				forces.My *= -1.0;
			}
			else
			{
				forces.Mz *= -1.0;
			}

			return forces;
		}
	}
}