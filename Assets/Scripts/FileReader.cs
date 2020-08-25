// Copyright Code Animo (Laurens Mathot) 2020, All rights reserved. Example project for Beyond Sports (https://www.beyondsports.nl),
using UnityEngine;

using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnimo
{

	public class FileReader : MonoBehaviour
	{
		public string FileName;
		public bool ShouldAttemptRead = true;

		[Header("Output")]
		[System.NonSerialized] public string[] Frames;
		public bool FramesAvailable = false;

		private string _fileReadLocation;
		private StreamReader _reader;
		private Task<string> _readTask;

		private readonly long maxExpectedFileSize = 500000000;

		private char[] _separatorNewline		= { '\r','\n' };

		private void OnEnable()
		{
			_fileReadLocation = Path.Combine(Application.persistentDataPath, FileName);

			ShouldAttemptRead = true;
		}

		private void OnDisable()
		{
			_reader.Close();
			_reader.Dispose();
			_reader = null;
		}

		private void Update()
		{
			if (ShouldAttemptRead && _readTask == null)
			{
				FramesAvailable = false;

				if (File.Exists(_fileReadLocation))
				{
					FileInfo fileInfo = new FileInfo(_fileReadLocation);
					Debug.Assert(fileInfo.Length < maxExpectedFileSize);// If the files get an order of magnitude larger, it might be worth verifying if the "Just load everything at once" approach is still reasonable.

					// Assume the file is complete.
					// TODO: it could be interesting to check for completeness with a known checksum, like Downloader.
					_reader = new StreamReader(_fileReadLocation, Encoding.UTF8);
					_readTask = _reader.ReadToEndAsync();
				}
				else
				{
					Debug.LogError("File '" + _fileReadLocation + "' does not currently exist. Did you download it?");
				}

			}

			if (_readTask != null && _readTask.IsCompleted)
			{
				switch(_readTask.Status)
				{
					case TaskStatus.RanToCompletion:
						Debug.Log("Completed reading '" + _fileReadLocation + "'");
						Frames = _readTask.Result.Split(_separatorNewline, System.StringSplitOptions.RemoveEmptyEntries);
						FramesAvailable = true;
						ShouldAttemptRead = false;
						_readTask = null;
						break;
					case TaskStatus.Canceled:
						Debug.Log("File read task cancelled, for location '" + _fileReadLocation + "'");
						break;
					case TaskStatus.Faulted:
						Debug.LogError("File read task failed, for location '" + _fileReadLocation + "', with the following exception:");
						Debug.LogException(_readTask.Exception);
						break;
					default:
						Debug.LogError("Unexpected status for completed task");
						break;
				}
				_readTask = null;
			}
		}

	}
}