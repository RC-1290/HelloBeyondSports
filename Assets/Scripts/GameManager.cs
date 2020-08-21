// Copyright Code Animo (Laurens Mathot) 2020, All rights reserved. Example project for Beyond Sports (https://www.beyondsports.nl),
using UnityEngine;

using System.Collections.Generic;

namespace CodeAnimo
{
	public class GameManager : MonoBehaviour
	{
		public Parser parser;
		public GameObject TrackedObjectPrefab;
		[Tooltip("Desired TrackedObject Count. Only updated OnEnable.")]
		public int TrackedObjectCount = 30;

		private List<TrackedObject> _trackedObjects;

		private void OnEnable()
		{
			_trackedObjects = new List<TrackedObject>(TrackedObjectCount);
			for (int trackedObjectIndex = 0; trackedObjectIndex < TrackedObjectCount; ++ trackedObjectIndex)
			{
				GameObject createdObject = Instantiate(TrackedObjectPrefab);
				TrackedObject trackedObject = createdObject.GetComponent<TrackedObject>();

				if (trackedObject == null)
				{
					Debug.LogError("The given TrackedObjectPrefab does not contain a TrackedObject components in the root.", this);
				}
				trackedObject.Index = trackedObjectIndex;
				trackedObject.Parser = parser;

				_trackedObjects.Add(trackedObject);

			}




		}
		private void OnDisable()
		{
			for (int trackedObjectIndex = 0; trackedObjectIndex < TrackedObjectCount; ++ trackedObjectIndex)
			{
				TrackedObject trackedObject = _trackedObjects[trackedObjectIndex];
				Destroy(trackedObject.gameObject);
			}
			_trackedObjects.Clear();
			_trackedObjects = null;
		}


	}
}
