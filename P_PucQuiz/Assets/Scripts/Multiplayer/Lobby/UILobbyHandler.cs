using UnityEngine;

public class UILobbyHandler : MonoBehaviour
{
    [SerializeField] private GameObject lobbyPlayerPrefab;

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

    private void UpdateLobbyUI()
    {
        Debug.Log("New Player joined bitch sapeca");
    }
}
