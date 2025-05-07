using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;

public class FusionConnector : MonoBehaviour
{
    #region Network

    private NetworkRunner _runner;
    public static FusionConnector Instance { get; private set; }

    #endregion
    #region Room Config
    public string LocalPlayerName { get; set; }
    public string LocalRoomCode { get; set; }

    public Dictionary<PlayerRef, String> players = new();

    [SerializeField] private TextMeshProUGUI roomCodeText;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject waitingScreen;
    [SerializeField] private GameObject roomScreen;
    
    #endregion
    
    #region Singleton
    private void Awake()
    {
        Application.targetFrameRate = 60;
        if(Instance != null)
            Destroy(gameObject);

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    #endregion

    public async void StartGame(bool isHosting)
    {
        menuScreen.gameObject.SetActive(false);
        waitingScreen.gameObject.SetActive(true);
        feedbackText.text = isHosting ? "Creating Room.." : "Joining to Room...";
        string roomCode = roomCodeText.text.ToString().ToUpper();
        //string roomCode = isHosting? GenerateRandomRoomCode(4):roomCodeText.text.ToString().ToUpper();
        //string roomCode = "Geferson";
        Debug.Log($"RoomCode:{roomCode}");
        StartGameArgs gameArgs = new StartGameArgs()
        {
            GameMode = isHosting? GameMode.Host: GameMode.Client,
            //GameMode = GameMode.Shared,
            PlayerCount = 20,
            SessionName = roomCode
        };
        _runner = gameObject.AddComponent<NetworkRunner>();
        
        StartGameResult result = await _runner.StartGame(gameArgs);

        if (result.Ok)
        {
            waitingScreen.gameObject.SetActive(false);
            roomScreen.gameObject.SetActive(true);
            Debug.Log($"Start Room:{roomCode}");
            roomScreen.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = !_runner.IsClient ? $"Start Room:{roomCode}" : $"Joined Room:{roomCode}"; 
        }
        else
        {
            //TODO ERROR Display
            
            feedbackText.text = $"Error: {result.ErrorMessage}";
        }

    }
    
    private void UpdatePlayerList(TextMeshProUGUI playerListText,List<String> players){
        if (playerListText != null)
        {
            playerListText.text = "Jogadores:\n";
            foreach (string name in players)
            {
                playerListText.text += $"- {name}\n";
            }
        }
    }

    internal void OnPlayerJoin(NetworkRunner runner, String playerName)
    {
        Debug.Log(runner.GameMode == GameMode.Host ? "Game is ready to start" : "Waiting for host to start game");
        players.Add(runner.LocalPlayer, playerName);
        
        Debug.Log($"Player added to Dictionary: {runner.LocalPlayer}, {playerName}");
        UpdatePlayerList(roomScreen.transform.GetChild(1).GetComponent<TextMeshProUGUI>(), players.Values.ToList());
    }

    public void AddPlayerToList(PlayerRef player, String playerName)
    {
        players.Add(player,playerName);
        UpdatePlayerList(roomScreen.transform.GetChild(1).GetComponent<TextMeshProUGUI>(), players.Values.ToList());
    }

    internal void OnPlayerJoin(NetworkRunner runner)
    {
        Debug.Log(runner.GameMode == GameMode.Host ? "Game is ready to start" : "Waiting for host to start game");
    }
    private String GenerateRandomRoomCode(int length)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string code = "";
        for (int i = 0; i < length; i++)
        {
            code += chars[UnityEngine.Random.Range(0, chars.Length)];
        }
        return code;
    }
}
 