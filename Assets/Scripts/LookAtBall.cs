// Copyright Code Animo (Laurens Mathot) 2020, All rights reserved. Example project for Beyond Sports (https://www.beyondsports.nl),
using UnityEngine;

namespace CodeAnimo
{
	public class LookAtBall : MonoBehaviour
	{
		public Transform Ball;

		public float smoothing = 0.1f;

		private Transform _transform;


		private void OnEnable()
		{
			_transform = GetComponent<Transform>();
		}

		private void Update()
		{
			Vector3 lookDirection = Ball.position - _transform.position;
			var exactRotation = Quaternion.LookRotation(lookDirection, _transform.up);
			_transform.rotation = Quaternion.Slerp(_transform.rotation, exactRotation, smoothing);
		}
	}
}