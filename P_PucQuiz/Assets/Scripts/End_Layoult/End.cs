using TMPro;
using UnityEngine;

public class End : MonoBehaviour
{
    //public UnityEngine.UI.Text points;
    public TextMeshProUGUI points;
    public static float local_points = 0;
    public float speed = 5;

    public Timer time;

    void Start()
    {
        points.text = "Points = " + local_points;
        time.Reset();
    }

    void Update()
    {

        if (Event_PucQuiz.points > local_points)
        {
            local_points = local_points + (Time.deltaTime * speed);

            if (local_points > Event_PucQuiz.points)
            {
                local_points = Event_PucQuiz.points;
            }

            points.text = "Points = " + ((int)local_points);
        }
        else
        {
            time.Run();
        }

        if(time.End())
        {
            if (Modos.get.Final())
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Event_PucQuiz.Change_Scene(Config_PucQuiz.Get_Config().Layout_Start);
                }
            }
            else
            {
                Event_PucQuiz.Change_Scene(Config_PucQuiz.Get_Config().Layout_Game);
            }
        }
    }
}
