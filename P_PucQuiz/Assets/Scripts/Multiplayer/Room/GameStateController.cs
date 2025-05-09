using System;
using System.Collections.Generic;
using Fusion;
using Multiplayer.Room;
using TMPro;
using UnityEngine;

public class GameStateController : NetworkBehaviour
{

    [SerializeField] private TextMeshProUGUI startEndDisplay;
    [SerializeField] private LobbyScreenUI lobbyUI;
    
    [SerializeField] private float startDelay = 5f;
    
    private List<NetworkBehaviourId> _playerIds = new();
    [Networked] private TickTimer Timer { get; set; }

    public enum GameStateEnum
    {
        Null,
        Lobby,
        Starting,
        Running,
        Ending
    }
    
    [Networked] [SerializeField] private GameStateEnum GameState { set; get; }

    private List<NetworkBehaviourId> _playerDataNetworkedIds = new();
    public override void Spawned()
    {
        Debug.Log("Game State Spawned");
        if (GameState == GameStateEnum.Running)
        {
            Debug.Log("Is running already");
            foreach (var player in Runner.ActivePlayers)
            {
                if (Runner.TryGetPlayerObject(player, out var playerObject) == false) return;
                
                TrackNewPlayer(playerObject.GetComponent<PlayerDataNetworked>().Id);
            }
        }
        if (!Object.HasStateAuthority)
            return;

        GameState = GameStateEnum.Lobby; 
        
        FindAnyObjectByType<PlayerSpawner>().StartPlayerSpawner(this);
        
        Runner.SetIsSimulated(Object, true);
    }

    public void StartGame()
    {
        //Only Host
        if (!Object.HasStateAuthority)
            return;

        GameState = GameStateEnum.Lobby;
        Timer = TickTimer.CreateFromSeconds(Runner, startDelay);
    }

    public override void FixedUpdateNetwork()
    {
        switch (GameState)
        {
            case GameStateEnum.Lobby:
                UpdateLobbyDisplay();
                break;
            case GameStateEnum.Starting:
                UpdateStartingDisplay();
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ChangeState(GameStateEnum newState)
    {
        GameState = newState;
       
        
    }

    public void UpdateLobbyDisplay()
    {
        //Host & Client
        Debug.Log("UpdatingLobbyDisplay");
        lobbyUI.UpdatePlayersListUI(
            $"Connected Players:{PlayersConnectedsNames(PlayersManager.Instance.GetPlayersNicknames())} ");
        
        //HOST
        if (Object.HasStateAuthority == false) return;
        
    }

    void UpdateStartingDisplay()
    {
        // Host & Client
        
        startEndDisplay.text = $"Game Starts In {Mathf.RoundToInt(Timer.RemainingTime(Runner) ?? 0)}";
        
        //Host
        if (Object.HasStateAuthority == false) return;
        
        if (Timer.ExpiredOrNotRunning(Runner) == false) return;

        GameState = GameStateEnum.Running;

    }

    private String PlayersConnectedsNames(List<String> playersList)
    {
        String text = String.Empty;
        foreach (var players in playersList)
        {
            text += "${players}\n";
        }

        return text;
    }

    public void TrackNewPlayer(NetworkBehaviourId id)
    {
        _playerIds.Add(id);
    }
}
