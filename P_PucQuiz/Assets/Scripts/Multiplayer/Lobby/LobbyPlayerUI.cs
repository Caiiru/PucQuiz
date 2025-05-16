using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Multiplayer.Lobby
{
    public class LobbyPlayerUI:MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;

        private Player _player;

        public void UpdatePlayer(Player player)
        {
            this._player = player;
            playerNameText.text = player.Data[QuizLobby.KEY_PLAYER_NAME].Value;
            //Debug.Log(player.Data[QuizLobby.KEY_PLAYER_NAME].Value);
        }
    }
}