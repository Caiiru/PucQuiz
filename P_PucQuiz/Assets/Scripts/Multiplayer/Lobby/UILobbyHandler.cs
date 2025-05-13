using TMPro;
using UnityEngine;

public class UILobbyHandler : MonoBehaviour
{
    [SerializeField] private GameObject lobbyPlayerPrefab;
    [SerializeField] private TextMeshProUGUI roomCodeText;

    private QuizLobby _quizLobby;
    void Start()
    {
        _quizLobby = QuizLobby.Instance;
        _quizLobby.playerJoined.AddListener(UpdateLobbyUI);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateLobbyUI(int playerCount)
    {
        Debug.Log($"player joined, count: {playerCount}");
        var lobbyPlayer = Instantiate(lobbyPlayerPrefab, FindAnyObjectByType<Canvas>().transform, true);
        lobbyPlayer.transform.position = new Vector3(-100 + (150 % playerCount), -100, 0);
        roomCodeText.text = $"Room Code: {QuizLobby.Instance.GetJoinedLobby().LobbyCode}";
        //lobbyPlayer.transform.GetComponentInChildren<TextMeshProUGUI>().text = QuizLobby.Instance.
    }
}
