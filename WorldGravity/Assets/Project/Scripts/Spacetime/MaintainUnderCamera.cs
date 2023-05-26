using System;
using UnityEngine;
using System.Collections;

namespace GravityVisualiser
{
	public class MaintainUnderCamera : MonoBehaviour
	{
		[SerializeField] private Transform player;

		private void LateUpdate()
		{
			Vector3 pos = player.localPosition;
			pos.y = 0;
			transform.localPosition = pos;
		}
	}
}