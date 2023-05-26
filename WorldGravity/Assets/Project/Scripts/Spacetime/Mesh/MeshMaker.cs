using System;
using UnityEngine;
using System.Collections;
using Unity.Mathematics;
using UnityEngine.Rendering;

namespace GravityVisualiser
{
	[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
	public class MeshMaker : MonoBehaviour
	{

		[SerializeField] private MeshFilter filter;

		public int x = 50, z = 50;
		public float extents = 1f;
		public int uvsPerQuad = 5;

		public Vector2 normalisedCenter;
		public float extentsIncreaseFromCenter;

		public bool rebuild { get; set; }
		
		public void OnEnable()
		{
			rebuild = true;
		}

		public void Update()
		{
			if (rebuild)
			{
				rebuild = false;

				filter = GetComponent<MeshFilter>();
				
				//Destroy any old meshes already here.
				if (filter.mesh != null)
				{
					if (Application.isPlaying)
					{
						// This will destroy the object in game.
						Destroy(filter.mesh);
					}
					else
					{
						// This will destroy the object in the editor.
						DestroyImmediate(filter.mesh);
					}
				}

				filter.sharedMesh = NewQuad(x, z, uvsPerQuad, extentsIncreaseFromCenter, normalisedCenter, extents);
			}
		}

		public static Mesh NewQuad(int xSquares, int zSquares, int uvsPerQuad, float increaseFromCenter,
			Vector2 normalisedCenter, float extent = 1f)
		{
			Vector3[] verts = new Vector3 [(xSquares + 1) * (zSquares + 1)];
			Vector2[] uvs = new Vector2[verts.Length];
			int[] tris = new int[xSquares * zSquares * 6];

			int halfX = Mathf.RoundToInt(xSquares * normalisedCenter.x),
				halfZ = Mathf.RoundToInt(zSquares * normalisedCenter.y);

			for (int x = 0; x <= xSquares; x++)
			{
				for (int z = 0; z <= zSquares; z++)
				{
					int vertIndex = x * (zSquares + 1) + z;
					Vector3 vertPos = new Vector3(SumOfGeoSequence(extent, increaseFromCenter, x - halfX), 0,
						SumOfGeoSequence(extent, increaseFromCenter, z - halfZ));

					verts[vertIndex] = vertPos;
					uvs[vertIndex] = new Vector2(vertPos.x, vertPos.z) / uvsPerQuad;
				}
			}


			int offsetPerZ = zSquares + 1;

			for (int x = 0; x < xSquares; x++)
			{
				for (int z = 0; z < zSquares; z++)
				{
					int bottomLeftVert = ((x + 0) * offsetPerZ + z);
					int bottomRightVert = ((x + 1) * offsetPerZ + z);
					int topRightVert = ((x + 1) * offsetPerZ + z + 1);
					int topLeftVert = ((x + 0) * offsetPerZ + z + 1);

					int firstTri = ((x * zSquares) + z) * 6;
					if (firstTri >= tris.Length)
						continue;
					tris[firstTri + 0] = topLeftVert;
					tris[firstTri + 1] = topRightVert;
					tris[firstTri + 2] = bottomRightVert;

					tris[firstTri + 3] = topLeftVert;
					tris[firstTri + 4] = bottomRightVert;
					tris[firstTri + 5] = bottomLeftVert;
				}
			}

			Mesh newMesh = new Mesh();
			newMesh.indexFormat = IndexFormat.UInt32;

			newMesh.triangles = new int[0];
			newMesh.vertices = verts;
			newMesh.triangles = tris;
			newMesh.uv = uvs;

			//Due to the vertex shader moving vertices outside of the boundes
			//We are making them large enough so that it wouldn't get culled 
			newMesh.bounds = new Bounds(Vector3.zero, Vector3.one * 9999999);


			return newMesh;
		}

		public static float SumOfGeoSequence(float firstTerm, float ratio, int lastTermIndex)
		{
			if (lastTermIndex < 0)
				ratio = 1f / ratio;  // Inverts the ratio for terms less than the center.
			if (Mathf.Abs(ratio - 1f) < Mathf.Epsilon)  // Handles the case where ratio is 1 to prevent division by zero.
				return lastTermIndex * firstTerm;
			return firstTerm * (1f - Mathf.Pow(ratio, lastTermIndex)) / (1f - ratio);
		}

		public static float ValueOfGeoTerm(float firstTerm, float ratio, int term)
		{
			return firstTerm * Mathf.Pow(ratio, term - 1);
		}
	}
}