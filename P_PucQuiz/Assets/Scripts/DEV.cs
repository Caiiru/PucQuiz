using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DEV : MonoBehaviour
{

    public GameObject consoleCanvas;


    public bool isDebug = true;
    public static DEV Instance;

    public OnlineMode OnlineMode;

    GameManager gameManager;


    void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        Instance = this;
    }
    void Start()
    {
        //consoleCanvas.gameObject.SetActive(true);
        //wasStarted = false;
        //DeveloperConsole.Console.AddCommand("PlayersNameCommand", PrintPlayersCommand);
        //DeveloperConsole.Console.AddCommand("AddPlayer", AddPlayerCommand);
        //DeveloperConsole.Console.AddCommand("RemovePlayer", RemovePlayerCommand);
        gameManager = GameManager.Instance;
        LobbyManager.Instance.OnJoiningGame += StartServer;
        if (!isDebug)
            Destroy(consoleCanvas);

    }

    public void StartServer(object sender, EventArgs e)
    {
        if (!isDebug) return;
        DeveloperConsole.Console.AddCommand("PlayersCount", PlayersCountCommand);
        DeveloperConsole.Console.AddCommand("PrintPlayers", PrintPlayersScoreCommand);
        DeveloperConsole.Console.AddCommand("SetPoints", SetMyPointsCommand);
        DeveloperConsole.Console.AddCommand("AddCard", AddCardToMyselfCommand);
 

        //DeveloperConsole.Console.AddCommand("AddCard", AddCardCommand); // -> SERVER
    }

    private void AddCardToMyselfCommand(string[] args)
    {
        var player = gameManager.LocalPlayer;

        if (player == null)
        {
            DevPrint("Player not found");
            return;
        }
        var _card = CardsManager.Instance.GetCardByID(int.Parse(args[0]));

        player.AddCard(_card);
        FindAnyObjectByType<CardContainer>().ActivateContainer();
        

    }

    private void PlayersNameCommand(string[] args)
    {
        List<QuizPlayer> _players = gameManager.players;
        string textToPrint = "";
        int _index = 0;
        foreach (QuizPlayer player in _players)
        {
            if (_index == 0)
            {
                textToPrint = player.PlayerName.Value.ToString();
            }
            else
            {
                textToPrint += ", " + player.PlayerName.Value.ToString();
            }

        }

        DevPrint($"players: {textToPrint}");
    }

    public void SetMyPointsCommand(string[] args)
    { 
        gameManager.SetPlayerScore(int.Parse(args[0]));
         
    }
     
    private void PlayersCountCommand(string[] args)
    {
        DevPrint($"Players Count: {gameManager.players.Count}");
    }

    public void AddPlayerCommand(string[] args)
    {
        string playerID = (args[0]);
        string playerName = args[1].ToString();

        gameManager.AddConnectedPlayer(playerID, playerName);
    }
    public void PrintPlayersCommand(string[] args)
    {
        var printText = "";
        var _index = 0;
        foreach (var player in gameManager.ConnectedPlayers)
        {
            if (_index == 0)
            {
                printText = $"{player.ClientId}:{player.PlayerName}, ";
            }
            else
            {
                printText = $"{printText}{player.ClientId}:{player.PlayerName}, ";
            }
            _index++;
        }
        DevPrint($"Players connected: {printText}");

    }

    public void PrintPlayersScoreCommand(string[] args)
    {
        QuizPlayerData[] p = GameManager.Instance.GetTop5Players();
        foreach (QuizPlayerData player in p)
        {
            DevPrint($"Player {player.PlayerName} has {player.Score} points");

        }
    }
    public void RemovePlayerCommand(string[] args)
    {
        gameManager.RemoveConnectedPlayerByName(args[0]);
    }

    public void DevPrint(string text)
    {
        if (!isDebug) return;
        Debug.Log(text);
        DeveloperConsole.Console.Print(text);
    }
}

public enum OnlineMode
{
    Local,
    Relay
}
