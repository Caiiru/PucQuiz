using TMPro;
using UnityEngine;

public class VisualCard : MonoBehaviour
{
    [Header("Scriptable Object Data")]
    public Cartas CardInfo;


    [Header("Visual References")]
    public SpriteRenderer spriteRenderer;
    public TextMeshProUGUI textMeshPro;


    void Start()
    {
    }
    public void CreateCard(Cartas cardSO)
    {
        CardInfo = cardSO;
        textMeshPro.text = CardInfo.name;
        spriteRenderer.sprite = CardInfo.visualSprite;

    } 
}
