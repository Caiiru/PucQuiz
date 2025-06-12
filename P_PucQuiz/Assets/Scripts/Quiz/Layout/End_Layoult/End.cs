using System;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class End
{
    Config_PucQuiz config;
    public MyPlayer[] players => Event_PucQuiz.players;
    public LayoutManager manager;
    public UIDocument doc;
    public anim_bar bar;
    public int length => players.Length;

    public Timer time;

    [Header("Layouts")]
    public DictionaryThree<String, GameObject, VisualTreeAsset>[] layout;

    public void Awake(GameObject obj)
    {
        manager = LayoutManager.instance;
        doc = obj.GetComponent<UIDocument>();
    }

    public void Start(GameObject obj)
    {
        time.Reset();
        if (!manager.multiplayer_on) { return; }
        if (GameManager.Instance.IsServer)
            GameManager.Instance.ChangeCurrentGameStateRPC(GameState.RoundOver, 3.5f);

    }

    public void Update(GameObject obj)
    {
        if (!bar.finish()) 
        {
            bar.Run();
            SetBars();
            return;
        }

        if(!time.End())
        {
            time.Run();
        }
        else
        {
            Return(obj);
        }
    }

    private void Return(GameObject obj)
    {
        manager.end_start = true;
        Modos quiz = manager.quiz;

        if (Event_PucQuiz.layout_actualy == "Rank")
        {
            manager.ChangeMenu("Quiz", quiz.attributes[quiz.question_actualy_index].question_type.ToString());
        }
        else
        {
            manager.ChangeMenu("Start", "Start");
        }
    }

    private void SetBars()
    {
        float[] points = new float[Event_PucQuiz.players.Length];
        for(int i = 0; i < Event_PucQuiz.players.Length; i++)
        {
            if (i == 5) { break; }
            points[i] = Event_PucQuiz.players[i].points;
        }
        /*
        float point_1;
        float point_2;
        float point_3;
        float point_4;
        
        point_1 = Event_PucQuiz.players[0].points;
        point_2 = Event_PucQuiz.players[1].points;
        point_3 = Event_PucQuiz.players[2].points;
        point_4 = Event_PucQuiz.players[3].points;
         */

        float[] porcents = new float[points.Length];
        for(int i = 0; i < Event_PucQuiz.players.Length; i++)
        {
            if(i == 5) { break; }
            if (i == 0) { porcents[i] = 100; }
            else if (points[i] != 0 && points[0] != 0) { porcents[i] = ((100 * points[i]) / points[0]); }
            else { porcents[i] = 0; }
            porcents[i] *= bar.getSize();
        }
        /*
        float porcent_1 = 100;
        float porcent_2 = ((100 * point_2) / point_1);
        float porcent_3 = ((100 * point_3) / point_1);
        float porcent_4 = ((100 * point_4) / point_1);

        porcent_1 *= bar.getSize();
        porcent_2 *= bar.getSize();
        porcent_3 *= bar.getSize();
        porcent_4 *= bar.getSize();
        */

        for(int i = 0; i < porcents.Length; i++)
        {
            if(i == 5) { break; }
            VisualElement player_bar = doc.rootVisualElement.Q("Progress"+(i+1));
            player_bar.style.width = new Length(porcents[i], LengthUnit.Percent);
        }
        for (int i = Event_PucQuiz.players.Length; i < 4; i++)
        {
            var remove = doc.rootVisualElement.Q("Progress"+(i+1));
            //remove.parent.Remove(remove); 
            remove.style.opacity = 0;
        }
        /*
        VisualElement bar_1 = doc.rootVisualElement.Q("Progress1");
        bar_1.style.width = new Length(porcent_1, LengthUnit.Percent);

        VisualElement bar_2 = doc.rootVisualElement.Q("Progress2");
        bar_2.Q("Progress2").style.width = new Length(porcent_2, LengthUnit.Percent);

        VisualElement bar_3 = doc.rootVisualElement.Q("Progress3");
        bar_3.style.width = new Length(porcent_3, LengthUnit.Percent);

        VisualElement bar_4 = doc.rootVisualElement.Q("Progress4");
        bar_4.style.width = new Length(porcent_4, LengthUnit.Percent);
        */
    }
    private void SetLayout()
    {
        for (int i = 0; i < layout.Length; i++)
        {
            if (layout[i].getValue1() == Event_PucQuiz.layout_actualy)
            {
                switch (layout[i].getValue1())
                {
                    case "Rank":
                        Debug.Log("Rank Set Start");

                        manager.sound_manager.Play("Rank Music", "Rank");

                        Debug.Log("Rank % = Start");

                        string[] names = new string[Event_PucQuiz.players.Length];

                        for(int o = 0; o < Event_PucQuiz.players.Length; o++)
                        {
                            if (o == 5) { break; }
                            doc.rootVisualElement.Q<Label>("PlayerName"+(o+1)).text = Event_PucQuiz.players[o].playerName;
                        }
                        for(int o = Event_PucQuiz.players.Length; o < 4; o++)
                        {
                            var remove = doc.rootVisualElement.Q<Label>("PlayerName"+(o+1));
                            //remove.parent.Remove(remove);
                            remove.style.opacity = 0;
                        }
                        /*
                        string name_2 = Event_PucQuiz.players[1].playerName;
                        string name_3 = Event_PucQuiz.players[2].playerName;
                        string name_4 = Event_PucQuiz.players[3].playerName;*/

                        Debug.Log("Players Count = " + Event_PucQuiz.players.Length);
                        
                        /*
                        Debug.Log("Rank Name = Start");
                        doc.rootVisualElement.Q<Label>("PlayerName1").text = name_1;
                        doc.rootVisualElement.Q<Label>("PlayerName2").text = name_2;
                        doc.rootVisualElement.Q<Label>("PlayerName3").text = name_3;
                        doc.rootVisualElement.Q<Label>("PlayerName4").text = name_4;*/

                        SetBars();

                        Debug.Log("Rank Set End");
                        if (GameManager.Instance.IsServer)
                            GameManager.Instance.ChangeCurrentGameStateRPC(GameState.DisplayingQuestion, 3.5f);

                        break;
                    case "End":
                        manager.sound_manager.Play("Rank Music", "End");
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void ChangeMenu(string menu_new)
    {
        if (menu_new == null) { Debug.Log("N�o foi atribuido um valor ao novo menu buscado."); return; }

        Event_PucQuiz.scene_actualy = "End";
        Event_PucQuiz.layout_actualy = menu_new;

        GameObject background = null;

        try
        {
            for (int i = 0; i < layout.Length; i++)
            {
                if (layout[i].getValue1() == menu_new)
                {
                    background = layout[i].getValue2();
                    doc.visualTreeAsset = layout[i].getValue3();
                    Debug.Log(layout[i].getValue3().name);
                }

            }

            if (!background.activeSelf && background != null) { background.SetActive(true); }

            for (int i = 0; i < layout.Length; i++)
            {
                if (layout[i].getValue1() != menu_new && layout[i].getValue2() != background)
                {
                    layout[i].getValue2().SetActive(false);
                }
            }
        }
        catch (Exception error)
        {
            Debug.Log(error);
        }

        SetLayout();
    }
}

[Serializable]
public class anim_bar
{
    [SerializeField] private AnimationCurve anim;
    [SerializeField] private float time_max;
    [SerializeField] private float size = 0;
    [SerializeField] private float time = 0;

    public void Run() 
    {
        if (time <= time_max)
        { 
            time += Time.deltaTime; size = anim.Evaluate(time/time_max); 
        } 
        else 
        { 
            time = 1; size = anim.Evaluate(time);
        }
    }

    public void Reset() { time = 0; }
    public bool finish() { return time >= time_max; }
    public float getSize()
    {
        return size;
    }
}