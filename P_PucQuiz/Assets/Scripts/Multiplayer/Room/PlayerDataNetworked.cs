using Fusion;
using Multiplayer.Room;
using UnityEngine;

public class PlayerDataNetworked : NetworkBehaviour
{
    private const int STARTING_POINTS = 0;
    
    //Local runtime references
    private PlayersManager _playersManager;
    private ChangeDetector _changeDetector;
    
    [Networked]
    public NetworkString<_16> NickName { get; private set; }
    
    [Networked]
    public int Score { get; private set; }

    public override void Spawned()
    {
        base.Spawned();
        
        Debug.Log("Player Data Spawned");
        //HOST AND CLIENT
        _playersManager = PlayersManager.Instance;
        
        //CLIENT
        if (Object.HasInputAuthority)
        {
            var nickname = FindAnyObjectByType<PlayerData>().GetNickname();
            Rpc_SetNickname(nickname);
            
        }
        //HOST
        if (Object.HasStateAuthority)
        {
            Score = STARTING_POINTS;
        }

        //HOST AND CLIENT
        _playersManager.AddEntry(Object.InputAuthority, this);
        _playersManager.UpdateNickname(Object.InputAuthority, NickName.ToString());
        _playersManager.UpdateScore(Object.InputAuthority, Score);

        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(NickName):
                    _playersManager.UpdateNickname(Object.InputAuthority,NickName.ToString());
                    break;
                case nameof(Score):
                    _playersManager.UpdateNickname(Object.InputAuthority,NickName.ToString());
                    break;
                
                
            }
        }
    }

    public override void Despawned(NetworkRunner runner, bool hassSstate)
    {
        _playersManager.RemoveEntry(Object.InputAuthority);
    }

    public void AddToScore(int points)
    {
        Score += points;
    }
    
    //Rpc used to send player information to the host
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void Rpc_SetNickname(string nickName)
    {
        if (string.IsNullOrEmpty(nickName)) return;
        NickName = nickName;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
