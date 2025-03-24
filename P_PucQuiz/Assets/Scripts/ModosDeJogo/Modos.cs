using UnityEditor.Build.Content;
using UnityEngine;

public class Modos : MonoBehaviour
{
    [SerializeField] private Config_PucQuiz config;
    [SerializeField] private GameObject[] questions;
    [SerializeField] private GameObject question_actualy;
    public void Awake()
    {
        if(config == null) { config = Resources.Load<Config_PucQuiz>("Config/Kahoot"); }

        for (int i = 0; i < questions.Length; i++)
        {
            if (questions[i] == null) { return; }

            Perguntas pergunta = questions[i].GetComponent<Perguntas>();
            pergunta.Pre_Load(transform.gameObject);
        }
    }
    public void Start()
    {
        
    }
    public void Update()
    {
        Perguntas pergunta = question_actualy.GetComponent<Perguntas>();
        pergunta.Update_Layout(transform.gameObject);
    }
}
