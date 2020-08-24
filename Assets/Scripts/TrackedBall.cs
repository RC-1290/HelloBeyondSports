// Copyright Code Animo (Laurens Mathot) 2020, All rights reserved. Example project for Beyond Sports (https://www.beyondsports.nl),
using UnityEngine;

namespace CodeAnimo
{
	public class TrackedBall : MonoBehaviour
	{
		public Parser Parser;
		
		private Transform _transform;
		private float _centimetersToMeters = .01f;

		private void OnEnable()
		{
			_transform = GetComponent<Transform>();
		}

		private void LateUpdate()
		{
			int xCentimeters	= Parser.BallX;
			int yCentimeters	= Parser.BallY;
			int zCentimeters	= Parser.BallZ;
			float speed			= Parser.BallSpeed;

			Vector3 newPosition = new Vector3(xCentimeters * _centimetersToMeters, zCentimeters * _centimetersToMeters, yCentimeters * _centimetersToMeters);

			_transform.position = newPosition;
		}

	}
}