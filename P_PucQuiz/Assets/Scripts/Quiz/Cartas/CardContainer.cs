using System;
using DG.Tweening;
using UnityEngine;

public class CardContainer : MonoBehaviour
{
    public GameObject container;
    private bool _isUp = false;
    [SerializeField] private bool isActive = false;
    SpriteRenderer spriteRenderer;

    GameManager _gameManager;
    public Vector3[] cardsStartPosition;


    public bool isDebug;
    void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnGameStateChanged += CheckState;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        if (isDebug)
        {
            spriteRenderer.enabled = true;
            isActive = true;
        }
        cardsStartPosition = new Vector3[transform.childCount];

        
        for (int i = 0; i < transform.childCount; i++)
        {
            cardsStartPosition[i] = transform.GetChild(i).position; // GET Cards start position
            transform.GetChild(i).gameObject.SetActive(false); // Hide all cards at start

            if (isDebug) {
                Debug.Log("DEBUG MODE ON CARD CONTAINER IS ACTIVE");
                continue;
            }
            
            transform.GetChild(i).GetComponent<VisualCard>().CardInfo = null;
            Destroy(transform.GetChild(i).GetChild(0).gameObject);
        }
         
    }


    public bool IsUp()
    {
        return _isUp;
    }

    public void DoMoveUp()
    {
        _isUp = true;
        container.transform.DOLocalMoveY(-3.5f, 1, false).SetEase(Ease.InOutBack);
    }
    public void DoMoveDown()
    {
        container.transform.DOMoveY(-6, 1, false).SetEase(Ease.InOutBack);
        _isUp = false;
    }
    private void CheckState(object sender, EventArgs e)
    {
        if (_gameManager.IsServer) return; // SERVER DONT NEED THIS
        if (!(_gameManager.CurrentGameState == GameState.CollectingAnswers)) // ONLY WHEN IS TO ANSWER 
        {
            isActive = false;
            spriteRenderer.enabled = false;
            return;
        }
        if (!isActive)
        {
            isActive = true;
            spriteRenderer.enabled = true;
        }

    }
    public void ActivateContainer()
    {
        if (isActive) return;
        isActive = true;
        spriteRenderer.enabled = true;
        UpdateCardsPosition(); 
    }

    public void UpdateCardsPosition()
    {
        if (_isUp) return;
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).GetChild(0);
            if (child == null) continue;
            transform.GetChild(i).gameObject.SetActive(true);
            //child.DOLocalMove(new Vector3(-5.5f + (i * 5), -1f, 0),0.5f).SetEase(Ease.InBack);
            child.DOMove(cardsStartPosition[i],0.5f).SetEase(Ease.InBack);

        }
    }
    private void OnMouseOver()
    {
        if (!isActive)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            if (_isUp)
            {
                DoMoveDown();
                return;
            }
            DoMoveUp();
        }
    }


}
