using Fusion;
using UnityEngine;

namespace Multiplayer.Room
{
    public class PlayerSpawner:NetworkBehaviour, IPlayerJoined, IPlayerLeft
    {
        [SerializeField] private NetworkPrefabRef playerNetworkPrefab = NetworkPrefabRef.Empty;

        private bool _gameAlreadyStarted = false;
        private GameStateController _gameStateController;

        public void StartPlayerSpawner(GameStateController controller)
        {
            
            Debug.Log("Spawning players");
            _gameAlreadyStarted = true;
            _gameStateController = controller;
            foreach (var player in Runner.ActivePlayers)
            {
                SpawnPlayer(player);
            }
        }

        private void SpawnPlayer(PlayerRef player)
        {
            var playerObject = Runner.Spawn(playerNetworkPrefab, Vector3.zero, Quaternion.identity, player);
            Runner.SetPlayerObject(player,playerObject);
            
            _gameStateController.TrackNewPlayer(playerObject.GetComponent<PlayerDataNetworked>().Id);
        }

        public void PlayerJoined(PlayerRef player)
        {
            //if (_gameAlreadyStarted == false) return;
            Debug.Log("Player Joined");
            SpawnPlayer(player);
        }

        public void PlayerLeft(PlayerRef player)
        {
            if (Runner.TryGetPlayerObject(player, out var networkObject))
            {
                Runner.Despawn(networkObject);
            }
            
            Runner.SetPlayerObject(player,null);
        }
    }
}