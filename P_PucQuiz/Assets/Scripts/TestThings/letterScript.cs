using DG.Tweening;
using TMPro;
using UnityEngine;

public class letterScript : MonoBehaviour
{
    public TextMeshProUGUI textObject;
    public int myIndex = -1;

    private TextManager myManager;
    private float finalSize = 2f;

    void Start()
    {
        myManager = FindAnyObjectByType<TextManager>();
        textObject = GetComponent<TextMeshProUGUI>();
        GetComponent<BoxCollider2D>().size = new Vector2(textObject.fontSize,textObject.fontSize);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFinalSize(float finalSize)
    {
        this.finalSize = finalSize;
    }

    public void SetIndex(int newIndex)
    {
        myIndex = newIndex;
    }

    private void OnMouseEnter()
    { 
        //transform.DOScale(finalSize, 0.5f);
        myManager.ChangeColor(myIndex);


    }

    private void OnMouseExit()
    {
       //transform.DOScale(1, 0.5f);
       myManager.ResetColors();
    }

}
