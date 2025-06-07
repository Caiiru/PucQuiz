using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Multiplayer.Lobby
{
    public class LobbyPlayerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;

        private Player _player;
 
        public void UpdatePlayerName(string playerName)
        {
            playerNameText.text = playerName;
        }
    }
}