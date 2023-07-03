using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

internal enum MessageType
{
    AUDIO = 0
}

public class WebSocketClient : MonoBehaviour
{
    private AudioClip audioClip;
    private AudioSource audioSource;
    private WebSocket websocket;

    // Start is called before the first frame update
    private async void Start()
    {
        websocket = new WebSocket("ws://localhost:5174");
        audioSource = gameObject.GetComponent<AudioSource>();
        websocket.OnOpen += () => { Debug.Log("Connection open!"); };

        websocket.OnError += e => { Debug.Log("Error! " + e); };

        websocket.OnClose += e =>
        {
            Debug.Log("Connection closed! Reconnecting...");
            websocket.Connect();
        };

        websocket.OnMessage += bytes =>
        {
            Debug.Log("OnMessage!");
            Debug.Log(Application.persistentDataPath);
            StartCoroutine(PlayAudio(bytes));
        };

        // waiting for messages
        await websocket.Connect();
    }

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    private IEnumerator<UnityWebRequestAsyncOperation> PlayAudio(byte[] audioData)
    {
        var tempFilePath = Application.persistentDataPath + "/CharacterTalk.mp3";
        File.WriteAllBytes(tempFilePath, audioData);

        using (var www = UnityWebRequestMultimedia.GetAudioClip($"file://{tempFilePath}", AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log(www);
                var myClip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = myClip;
                audioSource.Play();
            }
        }
    }

    private async Task SendMessage(MessageType messageType, dynamic data)
    {
        if (websocket.State == WebSocketState.Open)
        {
            var jsonData = JsonConvert.SerializeObject(new { messageType, data });
            var buffer = Encoding.UTF8.GetBytes(jsonData);
            await websocket.Send(buffer);
        }
    }

    public async void SendAudioMessage(float[] data)
    {
        await SendMessage(MessageType.AUDIO, data);
    }
}