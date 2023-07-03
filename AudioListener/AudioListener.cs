using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioListener : MonoBehaviour
{
    public float threshold = 0.0001f;

    [SerializeField] private WebSocketClient webSocketClient;

    private readonly List<float> _recordedAudioData = new();
    private readonly int recordingLength = 20;
    private bool isAudioDataCapture;
    private bool isRecording;
    private AudioClip recordedClip;
    private float recordTime;

    // Start is called before the first frame update
    private void Start()
    {
        if (Microphone.devices.Length > 0)
            recordedClip = Microphone.Start(null, true, recordingLength, AudioSettings.outputSampleRate);
        else
            Debug.LogWarning("No microphone found.");
    }

    // Update is called once per frame
    private void Update()
    {
        if (Microphone.IsRecording(null))
        {
            var audioData = ExtractAudio(1);
            var rms = CalculateRootMeanSquare(audioData);
            if (rms > threshold)
            {
                if (!isRecording)
                {
                    // Start recording
                    isRecording = true;
                    recordTime = Time.time;
                    Debug.Log("Start recording...");
                }
            }
            else
            {
                if (isRecording)
                {
                    // Stop recording
                    isRecording = false;

                    var elapsedTime = Mathf.RoundToInt(Time.time - recordTime);
                    var recordedAudioData = ExtractAudio(elapsedTime);
                    webSocketClient.SendAudioMessage(recordedAudioData);
                    _recordedAudioData.Clear();
                    Debug.Log("Stop recording...");
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        Microphone.End(null);
    }


    private float[] ExtractAudio(int seconds)
    {
        var maxPosition = AudioSettings.outputSampleRate * recordingLength;

        var endPosition = Microphone.GetPosition(null);
        var extractAudioSize = AudioSettings.outputSampleRate * seconds;
        var startPosition = endPosition - extractAudioSize;
        if (startPosition < 0) startPosition = maxPosition + startPosition;

        var output = new float[extractAudioSize];
        if (startPosition < endPosition)
        {
            recordedClip.GetData(output, startPosition);
            return output;
        }

        var firstChunk = new float[maxPosition - startPosition];
        recordedClip.GetData(firstChunk, startPosition);

        var secondsChunk = new float[endPosition];
        if (endPosition > 0) recordedClip.GetData(secondsChunk, 0);
        return firstChunk.Concat(secondsChunk).ToArray();
    }

    private float CalculateRootMeanSquare(float[] samples)
    {
        var sum = 0f;
        for (var i = 0; i < samples.Length; i++) sum += samples[i] * samples[i];
        var mean = sum / samples.Length;
        var rms = Mathf.Sqrt(mean);
        return rms;
    }
}