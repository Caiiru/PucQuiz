using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    public List<Cartas> AllCards = new();

    [Space]

    public GameObject[] LocalPlayerCards = new GameObject[4];

    [Header("Cards Stuff")]
    public Transform CardContainer;
    public GameObject CardPrefab;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region @ Cards Functions @

     

    public Cartas GetCardByID(int id)
    {
        foreach (var card in AllCards)
        {
            if (card.cardID == id) {
                //DEV.Instance.DevPrint($"Find by ID: {id}-{card.name}");
                return card;
            }
        }

        return null;
    }

    public void SpawnCard(Cartas card)
    {
        var _cardGO = Instantiate(CardPrefab, CardContainer);

        _cardGO.GetComponent<VisualCard>().CreateCard(card);
        
        //_cardGO.transform.localPosition = new Vector3(-5.5f+(LocalPlayerCards * 4) , -1.5f, 0);
    }
    public void UpdateCards()
    {
        string cards = GameManager.Instance.LocalPlayer.cartas.Value.ToString();
        foreach (var card in LocalPlayerCards)
        {
            if (card.GetComponent<VisualCard>().CardInfo == null)
            {
                card.SetActive(false);
            }
        }
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
