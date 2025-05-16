using Multiplayer.Lobby;
using TMPro;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class UILobbyHandler : MonoBehaviour
{
    [SerializeField] private GameObject lobbyPlayerPrefab;
    [SerializeField] private TextMeshProUGUI roomCodeText;
    [SerializeField] private Transform container;

    [SerializeField] private int _playerIndex = -1;

    private QuizLobby _quizLobby;
    void Start()
    {
        _quizLobby = QuizLobby.Instance;
        QuizLobby.Instance.OnJoinedLobby+=UpdateLobbyUI;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateLobbyUI(object sender, QuizLobby.LobbyEventArgs e)
    {
        int playerCount = e.lobby.Players.Count;
        _playerIndex = _playerIndex == -1 ? playerCount:_playerIndex;
        ClearLobby(); 
        roomCodeText.text = $"Room Code: {QuizLobby.Instance.GetJoinedLobby().LobbyCode}";
        int _index = 0 ;
        foreach (var player in e.lobby.Players)
        {
            var lobbyPlayer = Instantiate(lobbyPlayerPrefab, container.transform, true);
            //lobbyPlayer.GetComponent<RectTransform>().position = new Vector3(0 + (-10 % playerCount - 1), 0, 0);
            LobbyPlayerUI lobbyPlayerUI = lobbyPlayer.GetComponent<LobbyPlayerUI>();
            lobbyPlayerUI.UpdatePlayer(player); 

            //lobbyPlayer.GetComponent<RectTransform>().transform.localPosition = new Vector3(-800+(110*_playerIndex), 0);

        }
        
        
        
        Show();
         
        //lobbyPlayer.transform.GetComponentInChildren<TextMeshProUGUI>().text = QuizLobby.Instance.
    }

    private void ClearLobby()
    {
        foreach (Transform child in container)
        { 
            Destroy(child.gameObject);
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void UpdateNamesPosition()
    {
        
    }
}
