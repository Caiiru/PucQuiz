using System;
using DeveloperConsole;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;


public class GameManager : NetworkBehaviour
{

    public string textToSend = "xablauys";
    public static GameManager Instance;

    public GameManager()
    {
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        //var doc = FindAnyObjectByType<UIDocument>();
        //doc.rootVisualElement.Q<Label>("CodeText").text = "Codigo: ABCDERFG"; 
        DeveloperConsole.Console.AddCommand("startquiz", StartQuizCommand);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space Ppressed");
            if (IsClient)
                SendToServer_Rpc(textToSend + " is Client"); 
            if (IsServer)
                SendToClient_Rpc(textToSend + "is Server");
                
                 
        }  
    }
    void StartQuizCommand(string[] args)
    {
        if(IsServer)
            StartQuiz_Rpc();
    }

    [Rpc(SendTo.Everyone)]
    public void StartQuiz_Rpc()
    {
        Debug.Log("Starting quiz...");
        Event_PucQuiz.scene_actualy = "Quiz";
        
    }

    public void ChangeText(string newText)
    {
        textToSend = newText;
    }

    [Rpc(SendTo.Server)]
    void SendToServer_Rpc(string text)
    {
        
        Debug.Log($"Player {text} pressed space");
    }
    [Rpc(SendTo.NotServer)]
    void SendToClient_Rpc(string text)
    {
        Debug.Log(text);
    }
}
