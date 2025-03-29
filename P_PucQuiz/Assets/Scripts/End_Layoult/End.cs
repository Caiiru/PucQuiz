using TMPro;
using UnityEngine;

public class End : MonoBehaviour
{
    //public UnityEngine.UI.Text points;
    public TextMeshProUGUI points;
    public float local_points = 0;
    public float speed = 5;

    void Start()
    {
        points.text = "Points = " + local_points;
    }

    void Update()
    {
        if(Event_PucQuiz.points > 0)
        {
            local_points = local_points + (Time.deltaTime * speed);

            if (local_points > Event_PucQuiz.points)
            { 
                local_points = Event_PucQuiz.points;
                Event_PucQuiz.points = 0;
            }

            points.text = "Points = "+((int)local_points);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Event_PucQuiz.points = 0;
            Event_PucQuiz.Change_Scene("Start");
        }
    }
}
