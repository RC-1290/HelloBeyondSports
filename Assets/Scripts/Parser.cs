// Copyright Code Animo (Laurens Mathot) 2020, All rights reserved. Example project for Beyond Sports (https://www.beyondsports.nl),
using UnityEngine;

using System.Collections.Generic;

namespace CodeAnimo
{
	public class Parser : MonoBehaviour
	{
		[Header("Input")]
		public string	Data;
		public int		MaxTrackingID = 30;

		[Header("Output")]
		public int		FrameNumber = -1;

		// ECS friendly data:
		public int[]	TeamNumbers;
		public int[]	ShirtNumbers;
		public int[]	CentimetersX;
		public int[]	CentimetersY;
		public float[]	Speeds;

		public int			BallX;
		public int			BallY;
		public int			BallZ;
		public float		BallSpeed;
		public BallFlags	Flags;

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

			Flags			= BallFlags.None;

			FrameNumber		= -1;

			TeamNumbers		= new int	[MaxTrackingID];
			ShirtNumbers	= new int	[MaxTrackingID];
			CentimetersX	= new int	[MaxTrackingID];
			CentimetersY	= new int	[MaxTrackingID];
			Speeds			= new float	[MaxTrackingID];
			
			//
			char[] separatorColon		= {':'};
			char[] separatorSemicolon	= {';'};
			char[] separatorComma		= {','};

			string[] segments = Data.Split(separatorColon, System.StringSplitOptions.RemoveEmptyEntries);

			int expectedSegmentCount = 3;
			if (segments.Length != expectedSegmentCount)
			{
				Debug.LogError("Unexpected format. Expected " + expectedSegmentCount + " segments, got " + segments.Length);
			}

			// Segment 1:
			int.TryParse(segments[0], out FrameNumber);

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

				success =	int.TryParse(	segmentNumbers[0], out TeamNumbers	[trackedObjectIndex]);
				success &=	int.TryParse(	segmentNumbers[2], out ShirtNumbers	[trackedObjectIndex]);
				success &=	int.TryParse(	segmentNumbers[3], out CentimetersX	[trackedObjectIndex]);
				success &=	int.TryParse(	segmentNumbers[4], out CentimetersY	[trackedObjectIndex]);
				success &=	float.TryParse(	segmentNumbers[5], out Speeds		[trackedObjectIndex]);

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

				success &= int.TryParse(	ballNumbers[0], out BallX);
				success &= int.TryParse(	ballNumbers[1], out BallY);
				success &= int.TryParse(	ballNumbers[2], out BallZ);
				success &= float.TryParse(	ballNumbers[3], out BallSpeed);

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
						Flags |= possibleFlag;
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