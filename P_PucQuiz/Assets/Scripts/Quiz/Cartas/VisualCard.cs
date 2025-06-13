using TMPro;
using UnityEngine;

public class VisualCard : MonoBehaviour
{
    [Header("Scriptable Object Data")]
    public Cartas CardInfo;


    [Header("Visual References")] 
    public TextMeshProUGUI textMeshPro;
    public GameObject visualPrefab;
     


    void Start()
    {
        if (CardInfo != null)
        {
            CreateCard(CardInfo);
        }
        if(GetComponent<SpriteRenderer>() != null)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    public void CreateCard(Cartas cardSO)
    {
        CardInfo = cardSO;
        if(cardSO.visualPrefab == null) { return; }
        visualPrefab = Instantiate(cardSO.visualPrefab,this.transform);
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        textMeshPro.text = CardInfo.cardName;
        //spriteRenderer.sprite = CardInfo.visualSprite;
         

    }
}
