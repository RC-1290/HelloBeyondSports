// Copyright Code Animo (Laurens Mathot) 2020, All rights reserved. Example project for Beyond Sports (https://www.beyondsports.nl),
using UnityEngine;

namespace CodeAnimo
{
	public class BackButton : MonoBehaviour
	{
		private void OnEnable()
		{
			Input.backButtonLeavesApp = true;
		}
	}
}
