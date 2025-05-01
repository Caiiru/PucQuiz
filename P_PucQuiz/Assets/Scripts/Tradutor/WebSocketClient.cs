using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.Serialization;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket _webSocket;
    [SerializeField] private string serverURL = "ws://localhost:8080/websocket";
    void Start()
    {
        
    }

    void ConnectToWebSocket()
    {
        _webSocket = new WebSocket(serverURL);

        _webSocket.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket conectado, enviando mensagem Stomp...");

            string connectFrame = "CONNECT\naccept-version:1.0,1.1,2.0\n\n0";
            _webSocket.Send(connectFrame);

            string subscribeFrame = "SUBSCRIBE\nid:sub-0\ndestination:/topic/greetins\n\n\0";
            _webSocket.Send(subscribeFrame);

            SendHelloMessage("Unity Client");
        };

        _webSocket.OnMessage += (sender, e) =>
        {
            Debug.Log("Mensagem recebida: " + e.Data);

            if (e.Data.Contains("Message"))
            {
                string content = e.Data.Split(new string[] { "\n\n" }, StringSplitOptions.None)[1];
                
                Debug.Log("Greetings::" + content);
            }
        };

        _webSocket.OnError += (send, e) =>
        {
            Debug.LogError("Erro no websocket: " + e.Message);
        };

        _webSocket.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket fechado: " + e.Reason);
        };
        
        _webSocket.Connect();

    }

    private void SendHelloMessage(string helloMessage)
    {
        string message = $"SEND\ndestination:/app/hello\ncontent-type:application/json\n\n{{\"name\":\"{helloMessage}\"}}\0";
        _webSocket.Send(message);
    }

    private void OnDestroy()
    {
        if (_webSocket != null && _webSocket.IsAlive)
        {
            _webSocket.Close();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
