 
using UnityEngine;

public class LayoutManager : MonoBehaviour
{
    public Login menu;
    public Modos quiz;
    [SerializeField] private bool quiz_start, menu_start = true;

    public void Awake()
    {
        Event_PucQuiz.scene_actualy = "Menu";

        if(Modos.get != null)
        {
            quiz = Modos.get;
        }
        quiz.Set_Transform(transform);
    }

    public void Update()
    {
        switch(Event_PucQuiz.scene_actualy)
        {
            case "Quiz":
                Quiz_Run();
                break;
            case "Menu":
                Menu_Run();
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

    private void Menu_Run()
    {
        if (menu_start)
        {
            menu.Awake();
            menu.Start();
            menu_start = false;
        }
        menu.Update();
    }
}
