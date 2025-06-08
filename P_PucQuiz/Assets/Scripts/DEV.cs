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


    void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        Instance = this;
    }
    void Start()
    {
        consoleCanvas.gameObject.SetActive(true);

        //DeveloperConsole.Console.AddCommand("PlayersNameCommand", PrintPlayersCommand);
        //DeveloperConsole.Console.AddCommand("AddPlayer", AddPlayerCommand);
        //DeveloperConsole.Console.AddCommand("RemovePlayer", RemovePlayerCommand);
        DeveloperConsole.Console.AddCommand("AddPoints", GameManager.Instance.GivePointsToEveryoneCommand);
        DeveloperConsole.Console.AddCommand("PlayersCount", PlayersCountCommand);
        DeveloperConsole.Console.AddCommand("PlayersName", PlayersNameCommand);
    
    }

    private void PlayersNameCommand(string[] args)
    {
        List<QuizPlayer> _players = GameManager.Instance.players;
        string textToPrint = "";
        int _index = 0;
        foreach(QuizPlayer player in _players)
        {
            if(_index == 0)
            {
                textToPrint = player.PlayerName.Value.ToString();
            }
            else
            {
                textToPrint += ", "+player.PlayerName.Value.ToString();
            }

        }

        DevPrint($"players: {textToPrint}");
    }

    private void PlayersCountCommand(string[] args)
    {
        DevPrint($"Players Count: {GameManager.Instance.players.Count}");
    }

    public void AddPlayerCommand(string[] args)
    {
        string playerID = (args[0]);
        string playerName = args[1].ToString();

        GameManager.Instance.AddConnectedPlayer(playerID, playerName);
    }
    public void PrintPlayersCommand(string[] args)
    {
        var printText = "";
        var _index = 0;
        foreach (var player in GameManager.Instance.ConnectedPlayers)
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
        GameManager.Instance.RemoveConnectedPlayerByName(args[0]);
    }

    public void DevPrint(string text)
    {
        if (!isDebug) return;
        Debug.Log(text);
        DeveloperConsole.Console.Print(text);
    }
}
