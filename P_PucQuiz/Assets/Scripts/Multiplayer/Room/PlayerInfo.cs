using System;
using Fusion;
using TMPro;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{ 
    [Networked, OnChangedRender(nameof(OnPlayerNameChanged))]
    public NetworkString<_16> PlayerName { get; set; }

    public TextMeshProUGUI nameText;

    public override void Spawned()
    {
        base.Spawned();
        OnPlayerNameChanged();
    }

    [Rpc(RpcSources.InputAuthority,RpcTargets.StateAuthority)]
    public void RPC_Config(string playerName)
    {
        PlayerName = playerName;
    }

    void OnPlayerNameChanged()
    {
        //nameText.text = PlayerName.Value;
    }
}
