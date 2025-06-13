using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Draggable : MonoBehaviour
{
    private Vector3 offset;
    private Camera cam;
    private Animator animator;
    [SerializeField] private bool isDragging = false;
    [SerializeField] private bool canBeDragged = true;

    [SerializeField]CardContainer cardContainer;
    private Vector3 startPosition;


    public bool isDebug = false;

    void OnEnable()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
        startPosition = transform.position;

        isDebug = DEV.Instance == null ? true : DEV.Instance.isDebug; // Check if DEV instance exists and get isDebug value 
        cardContainer = CardsManager.Instance.CardContainer.GetComponent<CardContainer>();
        if (isDebug) return;

        //cardContainer = FindAnyObjectByType<CardContainer>(); 
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

        if (!cardContainer.IsUp())
            return;

        cardContainer.DoMoveDown();


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
                if (!isDebug)
                    cardContainer.UpdateCardsPosition();
                canBeDragged = true;
            });
        }
    }

    private IEnumerator WaitForAnimation()
    {
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);
        CardsManager.Instance.UseCard(transform.parent.GetComponent<VisualCard>().CardInfo.cardID); // Use the card
        Destroy(gameObject);

        transform.parent.gameObject.SetActive(false);
        transform.parent.GetComponent<VisualCard>().CardInfo = null; // Clear the card info
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
