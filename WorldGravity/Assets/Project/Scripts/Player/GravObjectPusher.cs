using UnityEngine;

namespace GravityVisualiser
{
	public class GravObjectPusher : MonoBehaviour
	{
		[SerializeField] private float maxPushForce = 70;
		[SerializeField] private GravitationalObject gravObject;

		private Vector3 _relativePushDirection;

		public Vector3 RelativePushDirection
		{
			get => _relativePushDirection;
			set
			{
				//Remove the y component, and clamp the magnitude of the direction
				//As to not exceed the maxPushForce
				_relativePushDirection = value;
				_relativePushDirection.y = 0f;
				_relativePushDirection = Vector3.ClampMagnitude(_relativePushDirection, 1f);
			}
		}

		void FixedUpdate()
		{
			Vector3 pushDirection = transform.TransformDirection(RelativePushDirection);
			//If there is a rotational reference frame
			if (transform.parent)
			{
				pushDirection = transform.parent.InverseTransformDirection(pushDirection);
			}
			Vector3 pushForce = pushDirection * (Time.deltaTime * maxPushForce);
			gravObject.Velocity += pushForce;
		}
	}
}