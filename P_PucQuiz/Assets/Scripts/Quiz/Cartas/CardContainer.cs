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
    void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnGameStateChanged += CheckState;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        if (DEV.Instance.isDebug)
        {
            spriteRenderer.enabled = true;
            isActive = true;
        }
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
    private void OnMouseOver()
    {
        if (!isActive)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Inside");
            if (_isUp)
            {

                container.transform.DOMoveY(-6, 1, false);
                _isUp = false;
                return;
            }
            _isUp = true;
            container.transform.DOLocalMoveY(-3.5f, 1, false);
        }
    }
    void OnMouseEnter()
    {
       

    }
    void OnMouseExit()
    { 
    }


}
