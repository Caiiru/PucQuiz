using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DEV : MonoBehaviour
{

    public GameObject consoleCanvas;


    public bool isDebug = true;
    public bool isTimerInfinity = true;
    public static DEV Instance;

    private bool wasStarted;

    GameManager gameManager;


    void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        Instance = this;
    }
    void Start()
    {
        consoleCanvas.gameObject.SetActive(true);
        wasStarted = false;
        //DeveloperConsole.Console.AddCommand("PlayersNameCommand", PrintPlayersCommand);
        //DeveloperConsole.Console.AddCommand("AddPlayer", AddPlayerCommand);
        //DeveloperConsole.Console.AddCommand("RemovePlayer", RemovePlayerCommand);
        gameManager = GameManager.Instance;
        gameManager.OnUpdateUI += StartServer;
    
    }

    public void StartServer(object sender, EventArgs e)
    {
        if (wasStarted || !gameManager.IsServer) return;
        wasStarted = true;


        DeveloperConsole.Console.AddCommand("SetPoints", GivePointsToPlayerCommand);
        DeveloperConsole.Console.AddCommand("PlayersCount", PlayersCountCommand);
        DeveloperConsole.Console.AddCommand("PlayersName", PlayersNameCommand);
        //DeveloperConsole.Console.AddCommand("AddCard", AddCardCommand); // -> SERVER
        DeveloperConsole.Console.AddCommand("AddCard", AddCardToMyselfCommand);
    }

    private void AddCardCommand(string[] args)
    {

        var player = gameManager.GetPlayerByID(gameManager.players[int.Parse(args[0])].ClientId.Value.ToString());


        if (player == null)
        {
            DevPrint($"Player {args[0]} not found");
            return;
        }
        gameManager.AddCardToPlayer(player.ClientId.Value.ToString(), int.Parse(args[1]));
    }

    private void AddCardToMyselfCommand(string[] args)
    {
        var player = gameManager.LocalPlayer;

        if (player == null)
        {
            DevPrint("Player not found");
            return;
        }
        player.AddCardByID(int.Parse(args[0]));
        
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

    public void GivePointsToEveryoneCommand(string[] args)
    {
        var players = gameManager.players;
        foreach (var player in players)
        {
            gameManager.SetPlayerScore(player.ClientId.Value.ToString(), int.Parse(args[0]));

            DEV.Instance.DevPrint($"{player.PlayerName.Value} has {player.Score.Value} points");
        }
    }

    public void GivePointsToPlayerCommand(string[] args)
    {
        var _players = gameManager.players;
        if (_players.Count == 0) return;
        var player = gameManager.GetPlayerByID(_players[int.Parse(args[0])].ClientId.Value.ToString());
        gameManager.SetPlayerScore(player.ClientId.Value.ToString(), int.Parse(args[1]));
        DEV.Instance.DevPrint($"{player.PlayerName.Value} has {player.Score.Value} points");
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
