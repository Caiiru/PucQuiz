using System;
using System.Collections.Generic;
using DeveloperConsole;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;


public class GameManager : NetworkBehaviour
{
    [Header("Game Config")]
    [SerializeField] private float timeToShowQuestion = 99f;
    [SerializeField] private float timePerQuestion = 15f;
    [SerializeField] private float timeToShowResults = 5f;

    // --- Variáveis de Rede ---
    public NetworkVariable<GameState> CurrentGameState = new NetworkVariable<GameState>(GameState.WaitingToStart);
    public NetworkVariable<Question> CurrentQuestionData = new NetworkVariable<Question>();
    public NetworkVariable<float> Timer = new NetworkVariable<float>(0f);
    public NetworkVariable<int> CurrentQuestionNumber = new NetworkVariable<int>(0); // Número da pergunta atual na rodada 
    public GameObject playerPrefab;
    public QuizPlayer player;

    public List<QuizPlayer> players = new();
 


    // --- Variáveis do Servidor ---
    [SerializeField] private List<Question> _allQuestions = new List<Question>();

    private Dictionary<ulong, int> _playerAnswers = new Dictionary<ulong, int>();

    [SerializeField]private QuizLobby _quizLobby;

    public EventHandler OnQuizStarted;

    #region Singleton
    public static GameManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

    }
    #endregion


    void Start()
    { 
        //DeveloperConsole.Console.AddCommand("printConnectedPlayers", PrintPlayersConnectedCommand);  
        _quizLobby = FindAnyObjectByType<QuizLobby>();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        CurrentGameState.OnValueChanged += OnGameStateChanged;
        CurrentQuestionData.OnValueChanged += OnQuestionChanged;
        Timer.OnValueChanged += OnTimerChanged;

        var _player = Instantiate(playerPrefab);
        if (IsOwner)
        { 
            if (player == null)
            {
                player = _player.GetComponent<QuizPlayer>();
            }
        }
        if (IsServer)
        {
            CurrentGameState.Value = GameState.WaitingToStart;
            Timer.Value = 0;
            players.Add(player.GetComponent<QuizPlayer>());
             
        }

        HandleGameStateChange(GameState.WaitingToStart, CurrentGameState.Value);
        HandleQuestionChange(default, CurrentQuestionData.Value);
        HandleTimerChange(0, Timer.Value);


        //DEBUG 


    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        CurrentGameState.OnValueChanged -= OnGameStateChanged;
        CurrentQuestionData.OnValueChanged -= OnQuestionChanged;
        Timer.OnValueChanged -= OnTimerChanged;

        if (Instance == this) Instance = null;

    }

    public void Update()
    {
        //ONLY SERVER
        if (!IsServer) return;


        Timer.Value = Timer.Value > 0 ? Timer.Value -= Time.deltaTime : 0;

        CheckGameState(CurrentGameState.Value);
    }

    public void CheckGameState(GameState currentState)
    {
        switch (currentState)
        {
            case GameState.WaitingToStart:
                if (_quizLobby.GetJoinedLobby().Players.Count == _quizLobby.GetJoinedLobby().MaxPlayers)
                {
                    StartQuizRpc();
                }
                break;
            case GameState.DisplayingQuestion:
                if (Timer.Value <= 0)
                {
                    CurrentGameState.Value = GameState.CollectingAnswers;
                    Timer.Value = timePerQuestion;

                }
                break;
            case GameState.CollectingAnswers:
            /*

                if (Timer.Value <= 0 || AllPlayersAnswered())
                    Debug.Log("All players answered or time pass");
                    */
                    break;

            case GameState.GameOver:
                //tela de vitoria
                break;
        }
    }
    public void OnGameStateChanged(GameState previousState, GameState newState)
    {
        HandleGameStateChange(previousState, newState);
    }

    private void HandleGameStateChange(GameState previousState, GameState newState)
    {
        DEV.Instance.DevPrint($"Gamestate was changed from {previousState} to {newState}");
    }

    public void OnQuestionChanged(Question previousQuestion, Question newQuestion)
    {
        HandleQuestionChange(previousQuestion, newQuestion);
    }
    private void HandleQuestionChange(Question previousQuestion, Question newQuestion)
    {
        DEV.Instance.DevPrint($"New Question Loaded: {newQuestion.QuestionText}");
        //TODO -> UPDATE UI
    }

    public void OnTimerChanged(float previousValue, float newValue)
    {
        HandleTimerChange(previousValue, newValue);
    }
    private void HandleTimerChange(float previousValue, float newValue)
    {
        //UI COM TEMPO 
    }

    public void PopulateQuestions()
    {
        //TODO -> Remove this 
        // Conect to Backend later
        if (!IsServer) return;

        _allQuestions.Clear();

        _allQuestions.Add(new Question("Qual a cor do céu em um dia limpo?", "Azul", "Verde", "Vermelho", "Amarelo", 0));
        _allQuestions.Add(new Question("Quantos dias tem uma semana?", "5", "6", "7", "8", 2));
        _allQuestions.Add(new Question("Qual o planeta mais próximo do Sol?", "Vênus", "Terra", "Marte", "Mercúrio", 3));
        _allQuestions.Add(new Question("2 + 2 = ?", "3", "4", "5", "22", 1));
        _allQuestions.Add(new Question("Qual a capital do Brasil?", "Rio de Janeiro", "São Paulo", "Brasília", "Salvador", 2));
        _allQuestions.Add(new Question("Em que ano o Brasil foi descoberto?", "1500", "1492", "1822", "1889", 0));
    }

    private bool AllPlayersAnswered() {
        if (!IsServer)
        {
            return false;
        }
        int playerCount = 0;
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject.GetComponent<QuizPlayer>() != null && client.PlayerObject.GetComponent<QuizPlayer>().alreadyAnswered.Value)
            {

                playerCount++;
            }
        }

        return playerCount >= NetworkManager.Singleton.ConnectedClientsList.Count;
    }
    [Rpc(SendTo.Everyone)]
    public void StartQuizRpc()
    {
        //Event_PucQuiz.scene_actualy = "Quiz";
        LayoutManager.instance.ChangeMenu("Quiz","Quiz"); 
        OnQuizStarted?.Invoke(this, null);
        Timer.Value = timeToShowQuestion;
        CurrentGameState.Value = GameState.DisplayingQuestion;
    }

    private void PrintPlayersConnectedCommand(string[] args)
    {
        foreach(var player in players)
        {
            DEV.Instance.DevPrint($"Connected Player: ${player.playerName}");
        }
    }
}

public enum GameState
{
    WaitingToStart,
    DisplayingQuestion,
    CollectingAnswers,
    ShowingResults,
    RoundOver,
    GameOver
}