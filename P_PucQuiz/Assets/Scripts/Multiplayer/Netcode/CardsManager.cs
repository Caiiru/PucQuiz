using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    public List<Cartas> AllCards = new();

    [Space]

    public GameObject[] LocalPlayerCards;

    [Header("Cards Stuff")]
    public Transform CardContainer;
    public GameObject CardPrefab;


    void Start()
    {
        CardPrefab = Resources.Load<GameObject>("Cartas/CardPrefab");
        CardContainer = FindAnyObjectByType<CardContainer>().gameObject.transform;
        LocalPlayerCards = new GameObject[4];
        for (int i = 0; i < LocalPlayerCards.Length; i++)
        {
            LocalPlayerCards[i] = CardContainer.GetChild(i).gameObject;
            LocalPlayerCards[i].SetActive(false);
        }
    }

    #region @ Cards Functions @



    public Cartas GetCardByID(int id)
    {
        foreach (var card in AllCards)
        {
            if (card.cardID == id)
            {
                //DEV.Instance.DevPrint($"Find by ID: {id}-{card.name}");
                return card;
            }
        }

        return null;
    }

    public void SpawnCard(Cartas card)
    {
        //var _cardGO = Instantiate(CardPrefab, CardContainer);

        //_cardGO.GetComponent<VisualCard>().CreateCard(card);

        //_cardGO.transform.localPosition = new Vector3(-5.5f+(LocalPlayerCards * 4) , -1.5f, 0);

        int freeCardSlot = GetFreeCardSlot();
        if (freeCardSlot == -1) return; // IS FULL

        LocalPlayerCards[freeCardSlot].GetComponent<VisualCard>().CardInfo = card;
        UpdateCards();
    }
    public void UpdateCards()
    {
        foreach (var card in LocalPlayerCards)
        {
            var visual = card.GetComponent<VisualCard>();
            if (visual.CardInfo == null)
            {
                card.SetActive(false);
            }
            visual.CreateCard(visual.CardInfo);
            card.SetActive(true);
        }
    }
    private int GetFreeCardSlot()
    {
        for (int i = 0; i < LocalPlayerCards.Length; i++)
        {
            if (LocalPlayerCards[i].GetComponent<VisualCard>().CardInfo == null)
            {
                return i;
            }
        }
        return -1;
    }




    #endregion

    #region @ Singleton @

    public static CardsManager Instance;
    public void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);

        Instance = this;
    }
    #endregion 
}
