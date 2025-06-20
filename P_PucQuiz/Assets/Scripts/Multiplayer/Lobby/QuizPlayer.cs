using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Multiplayer.Lobby;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class QuizPlayer : NetworkBehaviour, IEquatable<QuizPlayer>, IComparable<QuizPlayer>
{

    [Header("Player Name")]

    public NetworkVariable<FixedString32Bytes> PlayerName = new("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<FixedString32Bytes> ClientId = new("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Space]
    [Header("Points")]

    public NetworkVariable<int> Score = new(0, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> Score_New = new(0, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
    [Space]
    [Header("Cards")]
    public NetworkVariable<int> slots = new(40, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<FixedString32Bytes> cartas = new("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);   // Cards uid separated by , 
    [SerializeField] private List<Cartas> playerCards = new List<Cartas>();
    [Header("Effects")]
    public NetworkVariable<bool> protetor = new(false, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> dobrar = new(false, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);


    CardsManager cardsManager;
    public void Start()
    {
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();


        if (IsOwner)
        {
            PlayerName.Value = LobbyManager.Instance.LocalPlayerName;
            ClientId.Value = LobbyManager.Instance.playerID;
            cardsManager = CardsManager.Instance;
            GameManager.Instance.LocalPlayer = this;
            slots.Value = 40;
        }

        if (IsServer)
        {
            GameManager.Instance.AddPlayer(this);
            Debug.Log("Is server and is Owner");
            SendAttributesToClientRPC(LayoutManager.instance.quiz.attributes);

        } 
        //LayoutManager.instance.AddQuizPlayer(this);

    }

    [Rpc(SendTo.Owner)]
    private void SendAttributesToClientRPC(Quiz_Attributes[] _attributes)
    {
        Debug.Log("Im owner and received message");

        if (_attributes == null || _attributes.Length == 0)
        {
            Debug.LogError("Received attributes are null or empty.");
            return;
        }
        Debug.Log(_attributes[0].question);
        LayoutManager.instance.quiz.attributes = _attributes;
    }

    #region @ CARD FUNCTIONS @ 
    public void AddCard(Cartas card)
    {
        Cartas card_values = card;
        DEV.Instance.DevPrint($"Trying to add {card.name} to {PlayerName.Value}");
        if (card_values == null) { Debug.Log("Carta n�o atribuida."); return; }

        playerCards.Add(card_values);
        CardsManager.Instance.SpawnCard(card);

        cartas.Value = "";
        int _index = 0;
        foreach (Cartas c in playerCards)
        {
            if (_index == 0)
            {
                cartas.Value = c.cardID.ToString();
            }
            else
            {
                cartas.Value = $"{cartas.Value},{c.cardID.ToString()}";
                Debug.Log(cartas.Value);
            }
            _index++;
        } 
    }
    public void AddCardByID(int id)
    {
        var card = CardsManager.Instance.GetCardByID(id);
        if (card != null)
        {
            AddCard(card);
        }
    }

    [Rpc(SendTo.Owner)]
    void SetCardsOnManagerRpc(int id)
    {

        var card = cardsManager.GetCardByID(id);
        if (card == null) return;
        Debug.Log($"Card found: {card.cardName}");
        //cardsManager.LocalPlayerCards.Add(card);
    }
    /*
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
            return card.name;
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
    */

    #endregion

    public bool Equals(QuizPlayer other)
    {
        if (other == null) return false;
        QuizPlayer objAsPart = other as QuizPlayer;
        if (objAsPart == null) return false;
        else return CompareTo(objAsPart) == 0;
    }

    public int CompareTo(QuizPlayer other)
    {
        if (other.Score == null)
            return 1;

        else return this.Score.Value.CompareTo(other.Score.Value);
    }

}
