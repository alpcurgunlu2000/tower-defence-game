using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ionic.Zip;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.InputSystem;
using Vosk;

public class VoskSpeechToText : MonoBehaviour
{
	public string ModelPath = "vosk-model-small-ru-0.22.zip";
	public VoiceProcessor VoiceProcessor;
	public int MaxAlternatives = 3;
	public bool AutoStart = true;
	public List<string> KeyPhrases = new List<string>();

	private Model _model;
	private VoskRecognizer _recognizer;
	private bool _recognizerReady;
	private string _decompressedModelPath;
	private string _grammar = "";
	private bool _isDecompressing;
	private bool _isInitializing;
	private bool _didInit;
	private bool _running;

	private readonly ConcurrentQueue<short[]> _threadedBufferQueue = new ConcurrentQueue<short[]>();
	private readonly ConcurrentQueue<string> _threadedResultQueue = new ConcurrentQueue<string>();

	static readonly ProfilerMarker voskRecognizerCreateMarker = new ProfilerMarker("VoskRecognizer.Create");
	static readonly ProfilerMarker voskRecognizerReadMarker = new ProfilerMarker("VoskRecognizer.AcceptWaveform");

	public Action<string> OnStatusUpdated;
	public Action<string> OnTranscriptionResult;

	void Start()
	{
		if (AutoStart)
		{
			StartVoskStt();
		}
	}

	public void StartVoskStt(List<string> keyPhrases = null, string modelPath = default, bool startMicrophone = false, int maxAlternatives = 3)
	{
		if (_isInitializing || _didInit)
			return;

		if (!string.IsNullOrEmpty(modelPath))
			ModelPath = modelPath;
		if (keyPhrases != null)
			KeyPhrases = keyPhrases;

		MaxAlternatives = maxAlternatives;
		StartCoroutine(DoStartVoskStt(startMicrophone));
	}

	private IEnumerator DoStartVoskStt(bool startMicrophone)
	{
		_isInitializing = true;
		yield return WaitForMicrophoneInput();
		yield return Decompress();

		OnStatusUpdated?.Invoke("Loading Model from: " + _decompressedModelPath);
		_model = new Model(_decompressedModelPath);

		yield return null;

		OnStatusUpdated?.Invoke("Initialized");
		VoiceProcessor.OnFrameCaptured += VoiceProcessorOnOnFrameCaptured;
		VoiceProcessor.OnRecordingStop += VoiceProcessorOnOnRecordingStop;

		_isInitializing = false;
		_didInit = true;
	}

	private void UpdateGrammar()
	{
		if (KeyPhrases.Count == 0)
		{
			_grammar = "";
			return;
		}

		JSONArray keywords = new JSONArray();
		foreach (string keyphrase in KeyPhrases)
			keywords.Add(new JSONString(keyphrase.ToLower()));
		keywords.Add(new JSONString("[unk]"));
		_grammar = keywords.ToString();
	}

	private IEnumerator Decompress()
	{
		if (!Path.HasExtension(ModelPath) || Directory.Exists(Path.Combine(Application.persistentDataPath, Path.GetFileNameWithoutExtension(ModelPath))))
		{
			_decompressedModelPath = Path.Combine(Application.persistentDataPath, Path.GetFileNameWithoutExtension(ModelPath));
			yield break;
		}

		string dataPath = Path.Combine(Application.streamingAssetsPath, ModelPath);
		Stream dataStream;
		if (dataPath.Contains("://"))
		{
			UnityWebRequest www = UnityWebRequest.Get(dataPath);
			www.SendWebRequest();
			while (!www.isDone) yield return null;
			dataStream = new MemoryStream(www.downloadHandler.data);
		}
		else
		{
			dataStream = File.OpenRead(dataPath);
		}

		var zipFile = ZipFile.Read(dataStream);
		zipFile.ExtractProgress += (sender, e) =>
		{
			if (e.EventType == ZipProgressEventType.Extracting_AfterExtractAll)
			{
				_isDecompressing = true;
				_decompressedModelPath = e.ExtractLocation;
			}
		};

		zipFile.ExtractAll(Application.persistentDataPath);

		while (!_isDecompressing)
			yield return null;

		yield return new WaitForSeconds(1);
		zipFile.Dispose();
	}

	private IEnumerator WaitForMicrophoneInput()
	{
		while (Microphone.devices.Length <= 0)
			yield return null;
	}

	private void VoiceProcessorOnOnFrameCaptured(short[] samples)
	{
		_threadedBufferQueue.Enqueue(samples);
	}

	private void VoiceProcessorOnOnRecordingStop()
	{
		Debug.Log("Stopped recording");
	}

	private bool isHoldingSpace = false;

	void Update()
	{
		if (Keyboard.current != null)
		{
			if (Keyboard.current.spaceKey.wasPressedThisFrame)
			{
				isHoldingSpace = true;
				StartListening();
			}
			else if (Keyboard.current.spaceKey.wasReleasedThisFrame)
			{
				isHoldingSpace = false;
				StopListening();
			}
		}

		// tanıma sonuçlarını çek
		if (_threadedResultQueue.TryDequeue(out string voiceResult))
			OnTranscriptionResult?.Invoke(voiceResult);
	}

	public void StartListening()
	{
		Debug.Log("StartListening");
		_threadedBufferQueue.Clear();
		_running = true;
		VoiceProcessor.StartRecording();
		Task.Run(() => ThreadedWork());
	}

	public void StopListening()
	{
		Debug.Log("StopListening");
		_running = false;
		VoiceProcessor.StopRecording();
	}

	private async Task ThreadedWork()
	{
		voskRecognizerCreateMarker.Begin();
		if (!_recognizerReady)
		{
			UpdateGrammar();
			_recognizer = string.IsNullOrEmpty(_grammar)
				? new VoskRecognizer(_model, 16000.0f)
				: new VoskRecognizer(_model, 16000.0f, _grammar);
			_recognizer.SetMaxAlternatives(MaxAlternatives);
			_recognizerReady = true;
		}
		voskRecognizerCreateMarker.End();

		voskRecognizerReadMarker.Begin();
		while (_running || !_threadedBufferQueue.IsEmpty)
		{
			if (_threadedBufferQueue.TryDequeue(out short[] voiceResult))
			{
				if (_recognizer.AcceptWaveform(voiceResult, voiceResult.Length))
				{
					var result = _recognizer.Result();
					_threadedResultQueue.Enqueue(result);
				}
				else
				{
					var partial = _recognizer.PartialResult();
					// İstersen partial'ı da enqueue edebilirsin
					// _threadedResultQueue.Enqueue(partial);
				}
			}
			else
			{
				await Task.Delay(50);
			}
		}

		var final = _recognizer.FinalResult();
		_threadedResultQueue.Enqueue(final);

		voskRecognizerReadMarker.End();
	}
}
