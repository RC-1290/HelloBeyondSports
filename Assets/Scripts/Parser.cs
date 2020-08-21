// Copyright Code Animo (Laurens Mathot) 2020, All rights reserved. Example project for Beyond Sports (https://www.beyondsports.nl),
using UnityEngine;

using System.Collections.Generic;

namespace CodeAnimo
{
	public class Parser : MonoBehaviour
	{
		[Header("Input")]
		public string	data;
		public int		maxTrackingID = 30;

		[Header("Output")]
		public int		frameNumber = -1;

		public int[]	teamNumbers;
		public int[]	shirtNumbers;
		public int[]	xCentimeters;
		public int[]	yCentimeters;
		public float[]	speeds;

		public int			ballX;
		public int			ballY;
		public int			ballZ;
		public float		ballSpeed;
		public BallFlags	flags;

		[System.Flags]
		public enum BallFlags
		{
			None	= 0 << 0,
			Home	= 1 << 0,
			Away	= 1 << 1,
			Alive	= 1 << 2,
			Dead	= 1 << 3,
			SetHome = 1 << 4,
			SetAway = 1 << 5,
			Whistle = 1 << 6
		};

		private Dictionary<string, BallFlags> _stringToFlagMap;

		private void OnEnable()
		{
			_stringToFlagMap = new Dictionary<string, BallFlags>();
			_stringToFlagMap.Add("H",		BallFlags.Home);
			_stringToFlagMap.Add("A",		BallFlags.Away);
			_stringToFlagMap.Add("Alive",	BallFlags.Alive);
			_stringToFlagMap.Add("Dead",	BallFlags.Dead);
			_stringToFlagMap.Add("SetHome", BallFlags.SetHome);
			_stringToFlagMap.Add("SetAway", BallFlags.SetAway);
			_stringToFlagMap.Add("Whistle", BallFlags.Whistle);

			flags			= BallFlags.None;

			frameNumber		= -1;

			teamNumbers		= new int	[maxTrackingID];
			shirtNumbers	= new int	[maxTrackingID];
			xCentimeters	= new int	[maxTrackingID];
			yCentimeters	= new int	[maxTrackingID];
			speeds			= new float	[maxTrackingID];
			
			//
			char[] separatorColon		= {':'};
			char[] separatorSemicolon	= {';'};
			char[] separatorComma		= {','};

			string[] segments = data.Split(separatorColon, System.StringSplitOptions.RemoveEmptyEntries);

			int expectedSegmentCount = 3;
			if (segments.Length != expectedSegmentCount)
			{
				Debug.LogError("Unexpected format. Expected " + expectedSegmentCount + " segments, got " + segments.Length);
			}

			// Segment 1:
			int.TryParse(segments[0], out frameNumber);

			// Segment 2, trackedObjects:
			string[] trackedObjectSegments = segments[1].Split(separatorSemicolon, System.StringSplitOptions.RemoveEmptyEntries);

			for(
				int trackedObjectSegmentIndex = 0;
				trackedObjectSegmentIndex < trackedObjectSegments.Length;
				++trackedObjectSegmentIndex)
			{
				string segment = trackedObjectSegments[trackedObjectSegmentIndex];
				string[] segmentNumbers = segment.Split(separatorComma, System.StringSplitOptions.RemoveEmptyEntries);

				int trackedObjectIndex = -1;
				bool success = int.TryParse(segmentNumbers[1], out trackedObjectIndex);
				if (!success && trackedObjectIndex < 0)
				{	Debug.LogError("Unexpected format. Could not parse a valid tracked object index.");	}

				success =	int.TryParse(	segmentNumbers[0], out teamNumbers	[trackedObjectIndex]);
				success &=	int.TryParse(	segmentNumbers[2], out shirtNumbers	[trackedObjectIndex]);
				success &=	int.TryParse(	segmentNumbers[3], out xCentimeters	[trackedObjectIndex]);
				success &=	int.TryParse(	segmentNumbers[4], out yCentimeters	[trackedObjectIndex]);
				success &=	float.TryParse(	segmentNumbers[5], out speeds		[trackedObjectIndex]);

				if (!success)
				{	Debug.LogError("Unexpected format. One of the numbers didn't parse successfully.");	}

			}

			// Segment 3, Ball:
			{
				string[] balls = segments[2].Split(separatorSemicolon, System.StringSplitOptions.RemoveEmptyEntries);
				if (balls.Length > 1)
				{
					Debug.Log("Support for more than one ball has not been written.");
				}
				
				string[] ballNumbers = balls[0].Split(separatorComma, System.StringSplitOptions.RemoveEmptyEntries);
				bool success = true;

				success &= int.TryParse(	ballNumbers[0], out ballX);
				success &= int.TryParse(	ballNumbers[1], out ballY);
				success &= int.TryParse(	ballNumbers[2], out ballZ);
				success &= float.TryParse(	ballNumbers[3], out ballSpeed);

				if (!success)
				{	Debug.LogError("Unexpected format. One of the ball numbers didn't parse successfully.");	}

				for (int flagIndex = 4;
					flagIndex < ballNumbers.Length;
					++flagIndex)
				{
					string flag = ballNumbers[flagIndex];
					BallFlags possibleFlag = BallFlags.None;
					success = _stringToFlagMap.TryGetValue(flag, out possibleFlag);
					
					if (success)
					{
						flags |= possibleFlag;
					}
					else
					{
						Debug.LogWarning("Unknown flag: '" + flag + "'");
					}

				}

			}
		}
	}
}