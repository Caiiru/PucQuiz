using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    public List<Cartas> AllCards = new();

    [Space]

    public List<Cartas> LocalPlayerCards = new();


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
