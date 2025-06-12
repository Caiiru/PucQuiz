using UnityEngine;
using System.Collections;

public class Draggable : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Camera cam;
    private Animator animator;
    private bool isDropped = false;
    private bool isOverDropZone = false;

    void Start()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
    }

    void OnMouseDown()
    {
        if (isDropped) return;

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z);
        isDragging = true;

        Debug.Log("Card is being dragged.");
    }

    void OnMouseDrag()
    {
        if (isDragging && !isDropped)
        {
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z) + offset;

            // Check if currently over a DropZone
            Collider2D hit = Physics2D.OverlapPoint(transform.position);
            bool nowOverDropZone = hit != null && hit.CompareTag("DropZone");

            if (nowOverDropZone && !isOverDropZone)
            {
                isOverDropZone = true;
                Debug.Log("Card is in drop zone.");
            }
            else if (!nowOverDropZone && isOverDropZone)
            {
                isOverDropZone = false;
                Debug.Log("Card is off drop zone.");
            }
        }
    }

    void OnMouseUp()
    {
        if (isDropped) return;

        isDragging = false;
        Debug.Log("Card stopped being dragged.");

        // Final drop check
        Collider2D hit = Physics2D.OverlapPoint(transform.position);
        if (hit != null && hit.CompareTag("DropZone"))
        {
            isDropped = true;
            animator.SetBool("isOnDropZone", true);
            Debug.Log("Card dropped in drop zone.");

            StartCoroutine(WaitForAnimation());
        }
        else
        {
            Debug.Log("Card dropped outside of drop zone.");
        }
    }

    private IEnumerator WaitForAnimation()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("CardActivated"))
        {
            yield return null;
        }

        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        Debug.Log("CardActivated animation finished.");
    }
}
