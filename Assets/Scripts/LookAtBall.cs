// Copyright Code Animo (Laurens Mathot) 2020, All rights reserved. Example project for Beyond Sports (https://www.beyondsports.nl),
using UnityEngine;

namespace CodeAnimo
{
	public class LookAtBall : MonoBehaviour
	{
		public Transform Ball;

		private Transform _transform;


		private void OnEnable()
		{
			_transform = GetComponent<Transform>();
		}

		private void Update()
		{
			_transform.LookAt(Ball, Vector3.up);
		}
	}
}