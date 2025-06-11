using TMPro;
using UnityEngine;

public class VisualCard : MonoBehaviour
{
    [Header("Scriptable Object Data")]
    public Cartas CardInfo;


    [Header("Visual References")]
    public SpriteRenderer spriteRenderer;
    public TextMeshProUGUI textMeshPro;

    [Header("Animation")]
    public Animator animator;
    public Animation defaultAnimation;
    public Animation hoverAnimation;
    


    void Start()
    {
        if (CardInfo != null)
        {
            CreateCard(CardInfo);
        }
    }
    public void CreateCard(Cartas cardSO)
    {
        CardInfo = cardSO;
        textMeshPro.text = CardInfo.name;
        spriteRenderer.sprite = CardInfo.visualSprite;

        animator = cardSO.cardAnimator;
        defaultAnimation = cardSO.DefaultAnimation;
        hoverAnimation = cardSO.HoverAnimation;

    }
}
