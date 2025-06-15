using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Netcode;
using UnityEditor; 
using UnityEngine;

public class LayoutManager : MonoBehaviour
{
    public static LayoutManager instance;

    [Header("Event Variables")]
    [SerializeField] public MyPlayer player;
    [SerializeField] private string scene_actualy;
    [SerializeField] private string layout_actualy;
    [SerializeField] private string question_result;

    [Header("Manager Variables")]
    public SoundsManager sound_manager;
    public Login menu;
    public Modos quiz;
    public End end;
    [Header("Manager Variables - Start Bools",order = 1)]
    [SerializeField] public bool quiz_start = true;
    [SerializeField] public bool menu_start, end_start = true;

    [Header("Multiplayer Variables")]
    [SerializeField] public bool multiplayer_on;
    QuizPlayerData[] host_players;

    [Header("Multiplayer Test Variables")]
    [SerializeField] public MyPlayer[] local_players = new MyPlayer[5];



    public LayoutManager()
    {
        instance = this;
    }

    public void Awake()
    {
        Event_PucQuiz.scene_actualy = "Menu";
        quiz.transform = transform;
        if(sound_manager == null) { sound_manager = new SoundsManager(); }
        sound_manager.manager = this;
        sound_manager.Awake();

        if (!multiplayer_on)
        {
            //player.AddCard((Cartas)Resources.Load<ScriptableObject>("Cartas/Comum/Retirar"));
        }
        
    }

    public void Update()
    {
        instance = this;

        scene_actualy = Event_PucQuiz.scene_actualy;
        layout_actualy = Event_PucQuiz.layout_actualy;
        question_result = Event_PucQuiz.question_result;


        switch (Event_PucQuiz.scene_actualy)
        {
            case "Quiz":
                Quiz_Run();
                break;
            case "Menu":
                Menu_Run();
                break;
            case "End":
                End_Run();
                break;
        }

        sound_manager.Update();

    }
     
    /// <summary>
    /// Tenta colocar um valor Y em um valor X de forma mais segura.
    /// </summary>
    /// <param name="x">Esta � a variavel que voce deseja alterar.</param>
    /// <param name="y">Esta � a variavel que representa o novo valor desejado.</param>
    /// <returns>
    /// Retorna o valor X se Y n�o puder ser atribuido ou retorna Y se ele puder ser atribuido.
    /// </returns>
    private T Set<T>(T x, object y)
    {
        if (x is T)
        {
            return (T)y;
        }
        //Ideia do Gepeto abaixo ksksks
        try
        {
            // Tenta converter se for poss�vel (ex.: string -> int, int -> double, etc.)
            return (T)Convert.ChangeType(y, typeof(T));
        }
        catch
        {
            // Se n�o conseguir converter, mant�m o valor anterior
            return x;
        }
    }

    #region @ Scene Functions @
    private void Menu_Run()
    {
        if (menu_start)
        {
            menu.Awake();
            menu.Start();
            menu_start = false;
        }
        menu.Update();
    }
    private void Quiz_Run()
    {
        if (quiz_start)
        {
            //  Debug.Log("Call to Start Quiz");
            quiz.Awake(gameObject);
            quiz.Start(gameObject);
            quiz_start = false;
        }
        //Debug.Log("Call to Update Quiz");
 
        quiz.Update(gameObject);
    }
    private void End_Run()
    {
        if (end_start)
        {
            end.Awake(gameObject);
            end.Start(gameObject);
            end_start = false;
        }
        end.Update(gameObject);
    }
    public void ChangeMenu(string scene, string layout)
    {
        if (multiplayer_on) { MultiplayerOn(); }
        else { MultiplayerOff(); }

        //Debug.Log("LayoutManager change menu is calling");

        switch (scene)
        {
            case "Start":
                menu.ChangeMenu(layout);
                break;
            case "Quiz":
                GameManager.Instance.ChangeCurrentGameStateRPC(GameState.DisplayingQuestion, 3.5f);
                if (GameManager.Instance.IsServer)
                    quiz.ChangeMenu("HostQuiz");
                else
                    quiz.ChangeMenu(layout);
                break;
            case "End":
                end.ChangeMenu(layout);
                break;
        }
    }

    public void StartQuiz()
    {

        scene_actualy = "Quiz";
        Event_PucQuiz.scene_actualy = "Quiz";
        quiz_start = true;
        Quiz_Run();
        if (GameManager.Instance.IsServer)
        {
            quiz.ChangeMenu("HostQuiz");

        }
        else
        {
            quiz.ChangeMenu("Quiz");
        }


    }
    #endregion

    #region @ Multiplayer Functions @
    private void MultiplayerOff()
    {
        if (Event_PucQuiz.players == null) { Event_PucQuiz.players = new MyPlayer[5]; }

        Event_PucQuiz.player = player;
        Event_PucQuiz.points = player.points;
        Event_PucQuiz.players = local_players;
    }
    private void MultiplayerOn()
    {

        host_players = GameManager.Instance.GetTop5Players();
        //Debug.Log("Checking players - multiplayer");
        if (host_players == null || host_players.Length == 0)
        {
            Debug.LogWarning("No players found in multiplayer mode.");
            MultiplayerOff();
            return;
        }
         

        if (host_players != null)
        {
            local_players = new MyPlayer[host_players.Length];
            Event_PucQuiz.players = new MyPlayer[local_players.Length];
            int _index = 0;
            foreach(var quizPlayerData in host_players)
            {
                MyPlayer _player = new MyPlayer();
                _player.playerName = quizPlayerData.PlayerName.Value.ToString();
                _player.points = quizPlayerData.Score;
                local_players[_index] = _player;
                Event_PucQuiz.players[_index] = _player;
                _index++;
            }

            /*
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].PlayerName == null) {
                    Debug.LogError($"{i} has a null player");
                    continue;
                }
                MyPlayer _player = new MyPlayer();
                _player.playerName = players[i].PlayerName.Value.ToString();
                _player.points = players[i].Score;
                local_players[i] = _player;
                Event_PucQuiz.players[i] = _player;
            }
            */
            
        }
        MultiplayerOff();
    }
    #endregion

    #region @ Card Functions @
    [ContextMenu("Cartas/GetRandomCard(Comum)")]
    public void Random_Card()
    {
        int rand = UnityEngine.Random.Range(1, 4);

        switch (rand)
        {
            case 1:
                player.AddCard(Cartas.Get_Card(Cartas.Card_Types.Retirar));
                break;
            case 2:
                player.AddCard(Cartas.Get_Card(Cartas.Card_Types.Proteger));
                break;
            case 3:
                player.AddCard(Cartas.Get_Card(Cartas.Card_Types.Dobrar));
                break;
        }
    }

    [ContextMenu("Cartas/Retirar")]
    public void Card_Retirar()
    {
        if (player.InCartas(Cartas.Card_Types.Retirar))
        {
            Cartas.Get_Card(Cartas.Card_Types.Retirar).Use();
        }
    }

    [ContextMenu("Cartas/Proteger")]
    public void Card_Proteger()
    {
        if (player.InCartas(Cartas.Card_Types.Proteger))
        {
            Cartas.Get_Card(Cartas.Card_Types.Proteger).Use();
        }
    }

    [ContextMenu("Cartas/Dobrar")]
    public void Card_Dobrar()
    {
        if (player.InCartas(Cartas.Card_Types.Dobrar))
        {
            Cartas.Get_Card(Cartas.Card_Types.Dobrar).Use();
        }
    }
    #endregion
}

[Serializable]
public class MyPlayer
{
    [Header("Atributos")]
    public string playerName = "";
    public int points = 0;
    public int slots;
    [SerializeField] private Cartas[] cartas = new Cartas[4];
    public int cartas_index = 0;

    [Header("Efeitos")]
    public bool protetor = false;
    public bool dobrar = false;

    #region @ Card Functions @
    public void AddCard(Cartas card)
    {
        Cartas card_values = card as Cartas;

        if (card_values == null) { Debug.Log("Carta não atribuida."); return; }
        if (slots - card_values.cust < 0) { Debug.Log("O custo desta carta é maior do que seus slots."); return; }
        ;

        for (int i = 0; i < cartas.Length; i++)
        {
            if (cartas[i] == null)
            {
                cartas[i] = card;
                cartas_index++;
                slots -= card.cust;
                //Debug.Log("Carta adicionada = " + card_values.name);
                //Debug.Log("Descrição : " + card_values.description);
                break;
            }
        }
    }
    public void RemoveCard(Cartas.Card_Types type)
    {
        for (int i = 0; i < cartas.Length; i++)
        {
            if (cartas[i] != null)
            {
                Cartas card = (Cartas)cartas[i];

                if (card.types == type)
                {
                    cartas_index--;
                    slots += card.cust;
                    cartas[i] = null;
                }
            }
        }
    }
    public bool InCartas(Cartas.Card_Types type)
    {
        for (int i = 0; i < cartas.Length; i++)
        {
            if (cartas[i].types == type)
            {
                return true;
            }
        }
        return false;
    }
    public string PrintCardName(int i)
    {
        if (cartas[i] != null)
        {
            Cartas card = (Cartas)cartas[i];
            return card.cardName;
        }
        return "";
    }
    public string PrintCardDescription(int i)
    {
        if (cartas[i] != null)
        {
            Cartas card = (Cartas)cartas[i];
            return card.description;
        }
        return "";
    }
    #endregion
}