using UnityEngine;

public class CardHover : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnMouseEnter()
    {
        animator.SetBool("isHovered", true);
    }

    void OnMouseExit()
    {
        animator.SetBool("isHovered", false);
    }
}
