// StompHelper.cs
using System;
using System.Collections.Generic;
using WebSocketSharp;

public class StompHelper
{
    private WebSocket webSocket;
    private int subscriptionCounter = 0;
    private Dictionary<string, Action<string>> subscriptions = new Dictionary<string, Action<string>>();

    public void Connect(string url, Action onConnected, Action<string> onError)
    {
        webSocket = new WebSocket(url);
        
        webSocket.OnOpen += (sender, e) => {
            // Envia frame CONNECT STOMP
            SendStompFrame("CONNECT", new Dictionary<string, string> {
                {"accept-version", "1.2"},
                {"heart-beat", "10000,10000"}
            }, null);
            
            onConnected?.Invoke();
        };
        
        webSocket.OnMessage += (sender, e) => {
            ProcessStompFrame(e.Data);
        };
        
        webSocket.OnError += (sender, e) => {
            onError?.Invoke(e.Message);
        };
        
        webSocket.Connect();
    }

    public void Subscribe(string destination, Action<string> callback)
    {
        var subId = "sub-" + (subscriptionCounter++);
        subscriptions[subId] = callback;
        
        SendStompFrame("SUBSCRIBE", new Dictionary<string, string> {
            {"id", subId},
            {"destination", destination}
        }, null);
    }

    public void Send(string destination, string body)
    {
        SendStompFrame("SEND", new Dictionary<string, string> {
            {"destination", destination},
            {"content-type", "application/json"}
        }, body);
    }

    private void SendStompFrame(string command, Dictionary<string, string> headers, string body)
    {
        var frame = command + "\n";
        
        foreach (var header in headers)
        {
            frame += header.Key + ":" + header.Value + "\n";
        }
        
        frame += "\n";
        
        if (body != null)
        {
            frame += body;
        }
        
        frame += "\0";
        
        webSocket.Send(frame);
    }

    private void ProcessStompFrame(string frame)
    {
        var parts = frame.Split(new[] {"\n\n"}, StringSplitOptions.None);
        var headerLines = parts[0].Split('\n');
        var body = parts.Length > 1 ? parts[1] : null;
        
        var command = headerLines[0];
        var headers = new Dictionary<string, string>();
        
        for (int i = 1; i < headerLines.Length; i++)
        {
            if (!string.IsNullOrEmpty(headerLines[i]))
            {
                var headerParts = headerLines[i].Split(new[] {':'}, 2);
                if (headerParts.Length == 2)
                {
                    headers[headerParts[0]] = headerParts[1];
                }
            }
        }
        
        if (command == "MESSAGE" && headers.ContainsKey("subscription"))
        {
            var subId = headers["subscription"];
            if (subscriptions.ContainsKey(subId))
            {
                subscriptions[subId].Invoke(body);
            }
        }
    }
}