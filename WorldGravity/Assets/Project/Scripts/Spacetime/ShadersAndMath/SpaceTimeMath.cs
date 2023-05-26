using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

namespace GravityVisualiser
{
	/// <summary>
	/// Script for calculation space time height at any given point (proportional to gravitational acceleration potential)
	/// This script is supposed to easily match SpaceTimeMatch.cginc to allow for easier copy and pasting.
	/// This solution is not ideal, but it works \o/ Given infinite time, we'd have infinitely good solutions :>! 
	/// </summary>
	public class SpaceTimeMath : MonoBehaviour
	{
		public const int MAX_PLANETS = 15;
		public const float PI = 3.14159265359f;
		public static float4[] _planetsInfo = new float4[MAX_PLANETS];
		public static float _ease;
		public static float _centPotentialVisiblity;


		private static float OrbitalPeriod(float distanceToAclCenter, float mass, float gravConstant)
		{
			float deltaCubed = pow(distanceToAclCenter, 3);
			float mu = gravConstant * mass;

			return 2 * PI * sqrt(deltaCubed / mu);
		}

		private static float RotationRate(float distanceToAclCenter, float mass, float gravConstant)
		{
			return 2 * PI / OrbitalPeriod(distanceToAclCenter, mass, gravConstant);
		}

		private static float CentrifugalForce(float distanceToAclCenter, float massSun, float massPlanet,
			float gravConstant)
		{
			float rotationRate = RotationRate(distanceToAclCenter, massSun, gravConstant);
			float centForce = pow(rotationRate, 2) * distanceToAclCenter * massPlanet;
			return centForce;
		}

		private static float CentrifugalAcl(float rotationRate, float distanceToPoint, float massSun,
			float gravConstant)
		{
			float centForce = pow(rotationRate, 2) * distanceToPoint;
			return centForce;
		}

		private static float AngularMomentum(float mass, float rotationRate, float distanceToAclCenter)
		{
			float aclMom = rotationRate * mass * pow(distanceToAclCenter, 2);
			return aclMom;
		}

		private static float CentPotential(float angularMomentum, float distanceToAclCenter, float otherMass,
			float gravConstant)
		{
			float centPotential = pow(angularMomentum, 2) / (2 * otherMass * pow(distanceToAclCenter, 2));
			return centPotential;
		}

		private static float OrbitSpeed(float distance, float acl)
		{
			float speed = sqrt(1 / (acl * distance));
			return speed;
		}

		public static float GetYPosAtPoint(float2 pos, float withCentPotential, int excludeIndex = -1)
		{
			float yPos = 0;

			float3 centerOfMass =
				((_planetsInfo[0].xyz * _planetsInfo[0].w) + (_planetsInfo[1].xyz * _planetsInfo[1].w)) /
				(_planetsInfo[0].w + _planetsInfo[1].w);

			centerOfMass.y = 0;

			float distanceToCenterFromPerspectiveOfP1 = length(centerOfMass.xz - _planetsInfo[1].xz);
			float distanceToPoint = length(centerOfMass.xz - pos);

			float massOfP0 = _planetsInfo[0].w;
			float massOfP1 = _planetsInfo[1].w;

			float gravConstant = 6.674e-11f;

			float rotationRate = RotationRate(distanceToCenterFromPerspectiveOfP1, massOfP0, gravConstant);
			float angularMomentum = AngularMomentum(1, rotationRate, distanceToPoint);

			float centPotential = CentPotential(angularMomentum, distanceToPoint, 1, gravConstant);

			for (int i = 0; i < MAX_PLANETS; i++)
			{
				if (i != excludeIndex)
				{
					float distance = max(0.01f, length(pos - _planetsInfo[i].xz));
					yPos -= gravConstant * (_planetsInfo[i].w) / distance;
				}
			}

			yPos += -centPotential * withCentPotential;
			yPos *= _ease;
			if (float.IsNaN(yPos))
				yPos = 0f;
			return yPos;
		}
	}
}