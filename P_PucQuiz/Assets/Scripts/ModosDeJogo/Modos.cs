using UnityEditor.Build.Content;
using UnityEngine;

public class Modos : MonoBehaviour
{
    [SerializeField] private Config_PucKahoot config;
    [SerializeField] public GameObject[] perguntas;
    public void Awake()
    {
        if(config == null) { config = Resources.Load<Config_PucKahoot>("Config/Kahoot"); }

        for (int i = 0; i < perguntas.Length; i++)
        {
            if (perguntas[i] == null) { return; }

            Perguntas pergunta = perguntas[i].GetComponent<Perguntas>();
            pergunta.Pre_Load(transform.gameObject);
        }
    }
    public void Start()
    {
        
    }
    public void Update()
    {
        for (int i = 0; i < perguntas.Length; i++)
        {
            if (perguntas[i] == null) { return; }

            Perguntas pergunta = perguntas[i].GetComponent<Perguntas>();
            pergunta.Update_Layout(transform.gameObject);
        }

    }
}
