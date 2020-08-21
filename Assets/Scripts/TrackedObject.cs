// Copyright Code Animo (Laurens Mathot) 2020, All rights reserved. Example project for Beyond Sports (https://www.beyondsports.nl),
using UnityEngine;

namespace CodeAnimo
{
	public class TrackedObject : MonoBehaviour
	{
		public Parser Parser;
		public int Index = 0;
		public int TeamNumber;
		public int ShirtNumber;

		private Transform _transform;

		private void OnEnable()
		{
			_transform = GetComponent<Transform>();
		}

		private void LateUpdate()
		{
			// Remember that ECS stuff? Yeah, whatever, first version is just gonna ruin the cache for now ;)
			TeamNumber			= Parser.TeamNumbers[Index];
			ShirtNumber			= Parser.ShirtNumbers[Index];
			int xCentimeters	= Parser.CentimetersX[Index];
			int yCentimeters	= Parser.CentimetersY[Index];
			float speed			= Parser.Speeds[Index];

			Vector3 newPosition = new Vector3(xCentimeters / 100.0f, 0, yCentimeters / 100.0f);

			_transform.position = newPosition;

		}

	}
}