using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Draggable : MonoBehaviour
{
    private Vector3 offset;
    private Camera cam;
    private Animator animator;
    [SerializeField]private bool isDragging = false;
    [SerializeField] private bool canBeDragged = true;

    CardContainer cardContainer;
    private Vector3 startPosition;


    public bool isDebug = false;

    void Start()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
        startPosition = transform.position;

        if (isDebug) return;

        cardContainer = CardsManager.Instance.CardContainer.GetComponent<CardContainer>();
    }


    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z) + offset;

        }
    }
    void OnMouseDown()
    {
        if (!canBeDragged) return;

        if (!isDebug)
        {
            if (!cardContainer.IsUp())
                return;

            cardContainer.DoMoveDown();
        }

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z);
        
        isDragging = true; 
        animator.SetBool("isDragging", isDragging);
        canBeDragged = false;

    }

    void OnMouseUp()
    {
        if (canBeDragged) return;
        isDragging = false;
        animator.SetBool("isDragging", isDragging); 

        // Final drop check
        Collider2D hit = Physics2D.OverlapPoint(transform.position);
        if (hit != null && hit.CompareTag("DropZone"))
        {
            animator.SetTrigger("CardActivated");
            StartCoroutine(WaitForAnimation());
        }
        else
        {
            Debug.Log("Card dropped outside of drop zone."); 
            
            transform.DOMove(startPosition, 0.5f).SetEase(Ease.OutBounce).OnComplete(() => 
            {
                //animator.SetBool("isDropped", false);
                cardContainer.UpdateCardsPosition();
                canBeDragged = true;
            });
        }
    }

    private IEnumerator WaitForAnimation()
    { 
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);
        transform.parent.gameObject.SetActive(false); 
    }

    //HOVER
    void OnMouseEnter()
    {
        animator.SetBool("isHovered", true);
    }

    void OnMouseExit()
    {
        animator.SetBool("isHovered", false);
    }
}
