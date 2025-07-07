using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
/// <summary>
/// Class that records audio and delivers frames for real-time audio processing
/// </summary>
public class VoiceProcessor : MonoBehaviour
{
    public bool IsRecording
    {
        get { return _audioClip != null && Microphone.IsRecording(CurrentDeviceName); }
    }

    [SerializeField] private int MicrophoneIndex;

    public int SampleRate { get; private set; }
    public int FrameLength { get; private set; }

    public event Action<short[]> OnFrameCaptured;
    public event Action OnRecordingStop;
    public event Action OnRecordingStart;

    public List<string> Devices { get; private set; }
    public int CurrentDeviceIndex { get; private set; }
    public string CurrentDeviceName
    {
        get
        {
            if (CurrentDeviceIndex < 0 || CurrentDeviceIndex >= Microphone.devices.Length)
                return string.Empty;
            return Devices[CurrentDeviceIndex];
        }
    }

    [Header("Voice Detection Settings")]
    [SerializeField, Range(0.0f, 1.0f)]
    private float _minimumSpeakingSampleValue = 0.05f;

    [SerializeField]
    private float _silenceTimer = 1.0f;

    [SerializeField]
    private bool _autoDetect;

    private float _timeAtSilenceBegan;
    private bool _audioDetected;
    private bool _didDetect;
    private bool _transmit;

    private AudioClip _audioClip;
    private Coroutine _recordingCoroutine;
    private event Action RestartRecording;

    void Awake()
    {
        UpdateDevices();
    }

#if UNITY_EDITOR
    void Update()
    {
        if (CurrentDeviceIndex != MicrophoneIndex)
        {
            ChangeDevice(MicrophoneIndex);
        }
    }
#endif

    public void UpdateDevices()
    {
        Devices = new List<string>();
        foreach (var device in Microphone.devices)
            Devices.Add(device);

        if (Devices == null || Devices.Count == 0)
        {
            CurrentDeviceIndex = -1;
            Debug.LogError("There is no valid recording device connected");
            return;
        }

        CurrentDeviceIndex = MicrophoneIndex;
    }

    public void ChangeDevice(int deviceIndex)
    {
        if (deviceIndex < 0 || deviceIndex >= Devices.Count)
        {
            Debug.LogError($"Specified device index {deviceIndex} is not valid");
            return;
        }

        if (IsRecording)
        {
            RestartRecording += () =>
            {
                CurrentDeviceIndex = deviceIndex;
                StartRecording(SampleRate, FrameLength);
                RestartRecording = null;
            };
            StopRecording();
        }
        else
        {
            CurrentDeviceIndex = deviceIndex;
        }
    }

    public void StartRecording(int sampleRate = 16000, int frameSize = 512, bool? autoDetect = null)
    {
        if (autoDetect != null)
        {
            _autoDetect = (bool)autoDetect;
        }

        if (IsRecording)
        {
            Debug.Log("Already recording. Restarting...");
            StopRecording();
        }

        SampleRate = sampleRate;
        FrameLength = frameSize;

        _audioClip = Microphone.Start(CurrentDeviceName, true, 1, sampleRate);

        if (_recordingCoroutine != null)
        {
            StopCoroutine(_recordingCoroutine);
        }
        _recordingCoroutine = StartCoroutine(RecordData());
    }

    public void StopRecording()
    {
        if (!IsRecording)
            return;

        Microphone.End(CurrentDeviceName);
        Destroy(_audioClip);
        _audioClip = null;
        _didDetect = false;

        if (_recordingCoroutine != null)
        {
            StopCoroutine(_recordingCoroutine);
            _recordingCoroutine = null;
        }

        OnRecordingStop?.Invoke();

        if (RestartRecording != null)
        {
            RestartRecording.Invoke();
            RestartRecording = null;
        }
    }

    IEnumerator RecordData()
    {
        float[] sampleBuffer = new float[FrameLength];
        int startReadPos = 0;

        OnRecordingStart?.Invoke();

        while (IsRecording)
        {
            int curClipPos = Microphone.GetPosition(CurrentDeviceName);
            if (curClipPos < startReadPos)
                curClipPos += _audioClip.samples;

            int samplesAvailable = curClipPos - startReadPos;
            if (samplesAvailable < FrameLength)
            {
                yield return null;
                continue;
            }

            int endReadPos = startReadPos + FrameLength;
            if (endReadPos > _audioClip.samples)
            {
                int numSamplesClipEnd = _audioClip.samples - startReadPos;
                float[] endClipSamples = new float[numSamplesClipEnd];
                _audioClip.GetData(endClipSamples, startReadPos);

                int numSamplesClipStart = endReadPos - _audioClip.samples;
                float[] startClipSamples = new float[numSamplesClipStart];
                _audioClip.GetData(startClipSamples, 0);

                Buffer.BlockCopy(endClipSamples, 0, sampleBuffer, 0, numSamplesClipEnd);
                Buffer.BlockCopy(startClipSamples, 0, sampleBuffer, numSamplesClipEnd, numSamplesClipStart);
            }
            else
            {
                _audioClip.GetData(sampleBuffer, startReadPos);
            }

            startReadPos = endReadPos % _audioClip.samples;

            if (_autoDetect == false)
            {
                _transmit = _audioDetected = true;
            }
            else
            {
                float maxVolume = 0.0f;
                for (int i = 0; i < sampleBuffer.Length; i++)
                {
                    if (sampleBuffer[i] > maxVolume)
                        maxVolume = sampleBuffer[i];
                }

                if (maxVolume >= _minimumSpeakingSampleValue)
                {
                    _transmit = _audioDetected = true;
                    _timeAtSilenceBegan = Time.time;
                }
                else
                {
                    _transmit = false;

                    if (_audioDetected && Time.time - _timeAtSilenceBegan > _silenceTimer)
                    {
                        _audioDetected = false;
                    }
                }
            }

            if (_audioDetected)
            {
                _didDetect = true;
                short[] pcmBuffer = new short[sampleBuffer.Length];
                for (int i = 0; i < FrameLength; i++)
                {
                    pcmBuffer[i] = (short)Math.Floor(sampleBuffer[i] * short.MaxValue);
                }

                if (OnFrameCaptured != null && _transmit)
                    OnFrameCaptured.Invoke(pcmBuffer);
            }
            else
            {
                if (_didDetect)
                {
                    OnRecordingStop?.Invoke();
                    _didDetect = false;
                }
            }
        }
    }
}
