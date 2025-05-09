using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

namespace Multiplayer.Room
{
    public class PlayersManager:MonoBehaviour
    {
        
        #region Singleton
        public static PlayersManager Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            
            }

            Instance = this;
            DontDestroyOnLoad(this.gameObject);

        }
        #endregion
        
        private Dictionary<PlayerRef, PlayerDataNetworked> _playerEntries = new();
        private Dictionary<PlayerRef, string> _playerNicknames = new ();
        private Dictionary<PlayerRef, int> _playerScores = new();


        public void AddEntry(PlayerRef playerRef, PlayerDataNetworked playerData)
        {
            Debug.Log("Add Entry Point");
            _playerEntries.TryAdd(playerRef, playerData);
            _playerNicknames.TryAdd(playerRef, String.Empty);
            _playerScores.TryAdd(playerRef, 0);
        }

        public void UpdateNickname(PlayerRef playerRef, string nickName)
        {
            if (_playerEntries.TryGetValue(playerRef, out var playerDataNetworked) == false) return;
            //Dont find
            
            Debug.Log($"Update Nickname:{nickName}");
            _playerNicknames[playerRef] = nickName;
        }

        public void UpdateScore(PlayerRef playerRef, int score)
        {
            if (_playerEntries.TryGetValue(playerRef, out var playerDataNetworked) == false) return;

            _playerScores[playerRef] = score;
        }

        public void RemoveEntry(PlayerRef playerRef)
        {
            if (_playerEntries.TryGetValue(playerRef, out var networked) == false) return;

            if (networked != null)
                Destroy(networked.gameObject);

            _playerScores.Remove(playerRef);
            _playerNicknames.Remove(playerRef);
            _playerEntries.Remove(playerRef);
        }

        public List<String> GetPlayersNicknames()
        {
            return _playerNicknames.Values.ToList();
        }
        
    }
}