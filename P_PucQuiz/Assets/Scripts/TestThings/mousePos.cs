using UnityEngine;

public class mousePos : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Camera.main.WorldToScreenPoint(Input.mousePosition);
        transform.position = Input.mousePosition;
    }
}
