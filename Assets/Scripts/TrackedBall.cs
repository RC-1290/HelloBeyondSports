// Copyright Code Animo (Laurens Mathot) 2020, All rights reserved. Example project for Beyond Sports (https://www.beyondsports.nl),
using UnityEngine;

namespace CodeAnimo
{
	public class TrackedBall : MonoBehaviour
	{
		public Parser Parser;
		public AudioSource KickAudio;
		public float KickSoundSpeedThreshold = 100000;
		public float MinimumSecondsBetweenKicks = 1.0f;
		
		private Transform _transform;
		private float _centimetersToMeters = .01f;

		private float _lastSpeed = 0;
		private float _lastKickTime = 0;

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


			float kickForceEstimate = speed - _lastSpeed;
			if (kickForceEstimate > KickSoundSpeedThreshold && Time.time - _lastKickTime > MinimumSecondsBetweenKicks)
			{
				_lastKickTime = Time.time;
				KickAudio.Play();
			}

			_lastSpeed = speed;
		}

	}
}