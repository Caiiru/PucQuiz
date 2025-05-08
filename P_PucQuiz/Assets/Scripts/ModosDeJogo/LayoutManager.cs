 
using UnityEngine;

public class LayoutManager : MonoBehaviour
{
    public Login menu;
    public Modos quiz;
    [SerializeField] private bool quiz_start = true;

    public void Awake()
    {
        if(Modos.get != null)
        {
            quiz = Modos.get;
        }
        quiz.Set_Transform(transform);
    }

    public void Update()
    {
        switch(Event_PucQuiz.layout_actualy)
        {
            case "Quiz":
                Quiz_Run();
                break;
        }
        
    }

    private void Quiz_Run()
    {
        if (quiz_start)
        {
            quiz.Awake();
            quiz.Start();
            quiz_start = false;
        }
        quiz.Update();
    }
}
