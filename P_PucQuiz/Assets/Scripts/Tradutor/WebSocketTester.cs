using UnityEngine;
using WebSocketSharp;
using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Networking;

public class WebSocketTester : MonoBehaviour
{
    private WebSocket ws;
    private string serverUrl = "ws://localhost:8080/websocket";
    private string loginUrl = "http://localhost:8080/api/users/login";
    private bool isConnected = false;
    private bool isAuthenticated = false;
    private string authToken = null;
    private string pendingMessage = null;
    private string email = "admin@gmail.com"; // Defina seu email
    private string password = "admin#@!admin"; // Defina sua senha

    void Start()
    {
        StartCoroutine(LoginAndConnect());
    }

    IEnumerator LoginAndConnect()
    {
        Debug.Log("Iniciando processo de login...");
        
        // 1. Fazer login para obter o token JWT
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        UnityWebRequest loginRequest = UnityWebRequest.Post(loginUrl, form);
        loginRequest.SetRequestHeader("Content-Type", "application/json");
        
        Debug.Log("Enviando requisição de login...");
        yield return loginRequest.SendWebRequest();

        if (loginRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Falha no login: " + loginRequest.error);
            yield break;
        }

        Debug.Log("Login bem-sucedido! Processando token...");
        
        // Extrai o token da resposta (assumindo formato JSON: {"token":"..."})
        try
        {
            AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(loginRequest.downloadHandler.text);
            authToken = authResponse.token;
            isAuthenticated = true;
            Debug.Log("Token JWT obtido com sucesso: " + authToken.Substring(0, 10) + "...");
        }
        catch (Exception e)
        {
            Debug.LogError("Erro ao processar token: " + e.Message);
            yield break;
        }

        // 2. Conectar ao WebSocket com o token
        ConnectWebSocket();
    }

    void ConnectWebSocket()
    {
        if (!isAuthenticated || string.IsNullOrEmpty(authToken))
        {
            Debug.LogError("Tentativa de conexão WebSocket sem autenticação!");
            return;
        }

        Debug.Log("Iniciando conexão WebSocket...");
        ws = new WebSocket(serverUrl);
        
        // Configura headers de autenticação
        //ws.SetCredentials("Bearer " + authToken, true);
        ws.SetCredentials(email,password,false);
        ws.Origin = "unity://localhost";
        
        ws.OnOpen += (sender, e) => 
        {
            Debug.Log("WebSocket conectado, enviando handshake STOMP...");
            
            // Envia frame CONNECT STOMP com headers de autenticação
            string connectFrame = $"CONNECT\naccept-version:1.2\nAuthorization:Bearer {authToken}\n\n\0";
            ws.Send(connectFrame);
            
            // Assina o tópico de respostas
            string subscribeFrame = "SUBSCRIBE\nid:sub-0\ndestination:/topic/greetings\n\n\0";
            ws.Send(subscribeFrame);
            
            isConnected = true;
            Debug.Log("STOMP conectado e inscrito no tópico");
            
            // Envia mensagem pendente se houver
            if (!string.IsNullOrEmpty(pendingMessage))
            {
                SendHelloMessage(pendingMessage);
                pendingMessage = null;
            }
        };

        ws.OnMessage += (sender, e) => 
        {
            Debug.Log("Mensagem recebida: " + e.Data);
            ProcessStompFrame(e.Data);
        };

        ws.OnError += (sender, e) => 
        {
            Debug.LogError("Erro WebSocket: " + e.Message);
            isConnected = false;
        };

        ws.OnClose += (sender, e) => 
        {
            Debug.Log("Conexão fechada: " + e.Reason);
            isConnected = false;
        };

        Debug.Log("Conectando ao endpoint WebSocket...");
        ws.Connect();
    }

    void ProcessStompFrame(string frame)
    {
        if (frame.StartsWith("MESSAGE"))
        {
            string[] parts = frame.Split(new string[] {"\n\n"}, StringSplitOptions.None);
            if (parts.Length > 1)
            {
                string messageBody = parts[1].TrimEnd('\0');
                Debug.Log("Mensagem do servidor: " + messageBody);
            }
        }
        else if (frame.StartsWith("CONNECTED"))
        {
            Debug.Log("Conexão STOMP autenticada com sucesso!");
        }
    }

    public void SendHelloMessage(string name)
    {
        if (!isConnected || !isAuthenticated)
        {
            Debug.LogWarning("Conexão não autenticada. Mensagem será enfileirada.");
            pendingMessage = name;
            return;
        }

        string jsonMessage = $"{{\"name\":\"{name}\"}}";
        string stompFrame = $"SEND\ndestination:/app/hello\ncontent-type:application/json\nAuthorization:Bearer {authToken}\n\n{jsonMessage}\0";
        
        Debug.Log("Enviando mensagem: " + jsonMessage);
        ws.Send(stompFrame);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isAuthenticated)
        {
            SendHelloMessage("Unity Player");
        }
    }

    void OnDestroy()
    {
        if (ws != null && ws.IsAlive)
        {
            Debug.Log("Fechando conexão WebSocket...");
            ws.Close(CloseStatusCode.Normal);
        }
    }

    [Serializable]
    private class AuthResponse
    {
        public string token;
    }
}