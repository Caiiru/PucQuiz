 
using UnityEngine;

public class Basico : MonoBehaviour
{
    public Modos modo;

    public void Awake()
    {
        if(Modos.get != null)
        {
            modo = Modos.get;
        }
        modo.Set_Transform(transform);
    }

    public void Start()
    {
        modo.Awake();
        modo.Start();
    }

    public void Update()
    {
        modo.Update();
    }
}
