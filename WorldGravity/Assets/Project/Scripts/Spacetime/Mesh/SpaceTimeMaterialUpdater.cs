using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GravityVisualiser
{
	public class SpaceTimeMaterialUpdater : MonoBehaviour
	{
		public static float ease;


		//Mesh mesh;
		Material meshMat;
		MeshMaker meshMaker;

		//0 shows no centripetal force
		//1 shows centripetal force
		public float AlongCentripetalForce { get; set; }

		void Awake()
		{
			//mesh = GetComponent<MeshFilter> ().mesh;
			meshMat = GetComponent<MeshRenderer>().material;
			meshMaker = GetComponent<MeshMaker>();
		}

		void LateUpdate()
		{
			Vector4[] materialRelativePlanetInfos = new Vector4[SpaceTimeMath.MAX_PLANETS];
			Vector4[] PlanetInfos = new Vector4[SpaceTimeMath.MAX_PLANETS];
			
			for (int i = 0; i < SpaceTimeMath.MAX_PLANETS; i++)
			{
				if (GravitationalObject.Attractors.Count > i)
				{
					Vector3 gravObjPos = GravitationalObject.Attractors[i].transform.position;
					Vector3 gravObjLocalPos = GravitationalObject.Attractors[i].transform.localPosition;
					
					Vector3 matRelativeGravObjLocalPos = transform.InverseTransformPoint(gravObjPos);

					Vector4 planetInfo = (Vector4)gravObjLocalPos;
					Vector4 materialRelativePlanetInfo = matRelativeGravObjLocalPos;
					
					planetInfo.w = GravitationalObject.Attractors[i].Mass;
					materialRelativePlanetInfo.w = GravitationalObject.Attractors[i].Mass;
					
					materialRelativePlanetInfos[i] = materialRelativePlanetInfo;
					PlanetInfos[i] = planetInfo;
				}
				
				// meshMat.SetVector("_p" + (i).ToString(), p);
			}

			for (int i = 0; i < PlanetInfos.Length; i++)
			{
				SpaceTimeMath._planetsInfo[i] = PlanetInfos[i];
			}

			SpaceTimeMath._centPotentialVisiblity = AlongCentripetalForce;
			meshMat.SetFloat("_centUsageNormal", AlongCentripetalForce);
			
			ease = meshMat.GetFloat("_ease");
			SpaceTimeMath._ease = ease;


			meshMat.SetVectorArray("_planetsInfo", materialRelativePlanetInfos);
			
			
			//Offsets based on position, to allow it to be moved with the camera.
			Vector3 pos = transform.position;
			meshMat.mainTextureOffset = new Vector2(pos.x, pos.z) / (meshMaker.uvsPerQuad * meshMaker.extents);
		}
	}
}