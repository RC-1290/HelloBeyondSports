// Copyright Code Animo (Laurens Mathot) 2020, All rights reserved. Example project for Beyond Sports (https://www.beyondsports.nl),
using UnityEngine;

using System.Net.Http;
using System.Threading.Tasks;
using System.IO;

namespace CodeAnimo
{
	public class Downloader : MonoBehaviour
	{
		[Header("Settings")]
		[Tooltip("Address of the host.")]
		public string Host = "http://192.168.178.32";// For this demo I'm hosting the file on a machine on the local network. In production systems, you would switch to some other server.
		[Tooltip("Path relative to the host.")]
		public string FileName = "match_data.dat";
		
		private static HttpClient _client;// .Net really wants to have one, so it can handle socket re-use.
		private Task<byte[]> _downloadTask;
		private string _downloadFileLocation;
		private string _fileWriteLocation;

		private FileStream _fileWriter;
		private Task _fileWriteTask;
		
		private void OnEnable()
		{
			if (_client == null)
			{
			    _client = new HttpClient();
			}

			if (_downloadTask == null)
			{
				_fileWriteLocation = Path.Combine(Application.persistentDataPath, FileName);

				if (File.Exists(_fileWriteLocation))
				{
					//Note: The desired use case determines what should happen here.
					// Pre-recorded matches -> compare checksum for file completion
					// Streaming setups -> you might not even store these clientside, or have a ringbuffer for recently viewed matches.
					// For this demo we make the assumption that any previous files are completely what we want.
					Debug.Log("Download skipped, file already exists. Let's hope it's intact.");
				}
				else
				{
					_downloadFileLocation = Host;
					if (!_downloadFileLocation.EndsWith("/"))
					{
						_downloadFileLocation += "/";
					}
					_downloadFileLocation += FileName;

					_downloadTask = _client.GetByteArrayAsync(_downloadFileLocation);
				}
			}
		}

		private void OnDisable()
		{
			if (_fileWriter != null)
			{
				Debug.LogWarning("File writing not complete. '" + _fileWriteLocation + "'");
				_fileWriter.Dispose();
				_fileWriter = null;
			}
		}

		private void Update()
		{
			// Note: not using await stuff, because it's complicated to reason about, and ends up doing the same behind the scenes. And we might as well make use of Unity's frame Updates:
			if (_downloadTask != null && _downloadTask.IsCompleted)
			{
				try
				{
					switch(_downloadTask.Status)
					{
						case TaskStatus.RanToCompletion:
							Debug.Log("File downloaded, from '" + _downloadFileLocation + "'");
							byte[] downloadedFile = _downloadTask.Result;
							_fileWriter = new FileStream(_fileWriteLocation, FileMode.CreateNew, FileAccess.Write, FileShare.None);
							_fileWriteTask = _fileWriter.WriteAsync(downloadedFile, 0, downloadedFile.Length); 

							break;
						case TaskStatus.Canceled:
							Debug.Log("Download task cancelled, for location: '" + _downloadFileLocation + "'");
							break;
						case TaskStatus.Faulted:
							Debug.LogError("Download task failed, for location: '" + _downloadFileLocation + "', with the following exception:");
							Debug.LogException(_downloadTask.Exception);
							break;
						default:
							Debug.LogError("Unexpected status for completed task.");
							break;
					}
				}
				finally
				{
					// Doing this in a finally block to limit the number of exceptions logged by Unity, to one.
					_downloadTask = null;// Tasks don't need to be disposed: https://devblogs.microsoft.com/pfxteam/do-i-need-to-dispose-of-tasks/
				}
			}
			else if (_fileWriteTask != null && _fileWriteTask.IsCompleted)
			{
				switch(_fileWriteTask.Status)
				{
					case TaskStatus.RanToCompletion:
						Debug.Log("File completed writing to '" + _fileWriteLocation + "'");
						_fileWriteTask = null;
						_fileWriter.Dispose();
						_fileWriter = null;
						break;
					case TaskStatus.Canceled:
						Debug.Log("File write task cancelled, for location '" + _fileWriteLocation + "'");
						break;
					case TaskStatus.Faulted:
						Debug.LogError("File write task failed, for location '" + _fileWriteLocation + "', with the following exception:");
						Debug.LogException(_fileWriteTask.Exception);
						break;
					default:
						Debug.LogError("Unexpected status for completed task");
						break;
				}
			}

		}
	
	
	
	}
}