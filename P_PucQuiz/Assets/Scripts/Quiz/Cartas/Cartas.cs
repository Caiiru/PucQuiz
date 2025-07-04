using System;
using UnityEditor;
using UnityEngine;
 
public abstract class Cartas : ScriptableObject
{
    [HideInInspector] public Cartas instance;
    public string cardName;
    public int cardID;
    public Card_Types types = Card_Types.NoN;
    public int cust;
    [Multiline] public string description;

    public Sprite visualSprite;

    [Header("Anim")]
    public GameObject visualPrefab; 

    public enum Card_Types
    {
        Retirar,
        Proteger,
        Dobrar,
        Troca_Tudo,
        Velocidade,
        Espelho,
        Congelamento,
        SomarOuDividir,
        NoN
    }

    protected Cartas()
    {
        Set();
    }
    private void OnEnable()
    {
        Set();
    }
    public abstract void Set();
    public abstract void Use();

    public static Cartas Get_Card(Card_Types type)
    {
        bool exist = false;
        switch (type)
        {
            case Card_Types.Retirar: exist = true; break;
            case Card_Types.Proteger: exist = true; break;
            case Card_Types.Dobrar: exist = true; break;
        }

        if( exist ) { return (Cartas)Resources.Load<ScriptableObject>("Cartas/Comum/" + type.ToString()); }
        return null;
    }
}
