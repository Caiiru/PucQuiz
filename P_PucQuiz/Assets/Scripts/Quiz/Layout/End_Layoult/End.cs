using System;
using System.Collections;
using TMPro;
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

    public Timer time_rank;
    public Timer timer_end;

    [Header("Layouts")]
    public DictionaryThree<String, GameObject, VisualTreeAsset>[] layout;

    public void Awake(GameObject obj)
    {
        manager = LayoutManager.instance;
        doc = obj.GetComponent<UIDocument>();
        bar.Reset();
        time_rank.Reset();
        timer_end.Reset();
        if (GameManager.Instance.IsServer)
            GameManager.Instance.ChangeCurrentGameStateRPC(GameState.RoundOver, 3.5f);
    }

    public void Start(GameObject obj)
    {
        if (!manager.multiplayer_on) { return; }
        if (GameManager.Instance.IsServer)
            GameManager.Instance.ChangeCurrentGameStateRPC(GameState.RoundOver, 3.5f);

    }

    public void Update(GameObject obj)
    {
        if (!bar.finish() && Event_PucQuiz.layout_actualy == "Rank") 
        {
            bar.Run();
            SetBars();
            return;
        }

        if(!time_rank.End() && Event_PucQuiz.layout_actualy == "Rank")
        {
            time_rank.Run();
        }
        else if(!timer_end.End() && Event_PucQuiz.layout_actualy == "End")
        {
            timer_end.Run();
        }
        else
        {
            Return();
        }
    }

    private void Return()
    {
        manager.end_start = true;
        Modos quiz = manager.quiz;
        manager.sound_manager.Stop("Rank Sound");

        if (Event_PucQuiz.layout_actualy == "Rank")
        {
            manager.ChangeMenu("Quiz", quiz.attributes[quiz.question_actualy_index].question_type.ToString());
        }
        else if(Event_PucQuiz.layout_actualy == "End")
        {
            manager.ChangeMenu("Start", "Start");
        }
    }

    private void SetBars()
    {
        var players = Event_PucQuiz.players;
        float[] points = new float[players.Length];
        for(int i = 0; i < players.Length; i++)
        {
            if (i == 5) { break; }
            points[i] = players[i].points;
        }

        float[] porcents = new float[points.Length];
        for(int i = 0; i < players.Length; i++)
        {
            if (i >= 4) { break; }
            if (i == 0) { porcents[i] = 100; }
            else if (points[i] != 0 && points[0] != 0) { porcents[i] = ((100 * points[i]) / points[0]); }
            else { porcents[i] = 0; }
            porcents[i] *= bar.getSize();
        }

        for(int i = 0; i < porcents.Length; i++)
        {
            if (i >= 4) { break; }
            VisualElement player_bar = doc.rootVisualElement.Q("Progress"+(i+1));
            player_bar.style.width = new Length(porcents[i], LengthUnit.Percent);
        }
        for (int i =  players.Length; i < 4; i++)
        {
            var remove = doc.rootVisualElement.Q("Progress"+(i+1));
            remove.style.opacity = 0;
        }
    }
    IEnumerator SetEndBars(float time)
    {
        yield return new WaitForSeconds(time);

        VisualElement bar1 = doc.rootVisualElement.Q("Coluna_1Lugar");
        VisualElement bar2 = doc.rootVisualElement.Q("Coluna_2Lugar");
        VisualElement bar3 = doc.rootVisualElement.Q("Coluna_3Lugar");
        bar1.RemoveFromClassList("Coluna_1Lugar_Start");
        bar2.RemoveFromClassList("Coluna_2Lugar_Start");
        bar3.RemoveFromClassList("Coluna_3Lugar_Start");
    }
    private void SetLayout()
    {
        var players = Event_PucQuiz.players;
        string[] names;

        for (int i = 0; i < layout.Length; i++)
        {
            if (layout[i].getValue1() == Event_PucQuiz.layout_actualy)
            {
                switch (layout[i].getValue1())
                {
                    case "Rank":
                        Debug.Log("Rank Set Start");

                        manager.sound_manager.Play("Rank Sound", "Rank");

                        Debug.Log("Rank % = Start");

                        names = new string[players.Length];

                        for (int o = 0; o < players.Length; o++)
                        {
                            if (o >= 4) { break; }
                            doc.rootVisualElement.Q<Label>("PlayerName"+(o+1)).text =  players[o].playerName;
                        }
                        for(int o = players.Length; o < 4; o++)
                        {
                            var remove = doc.rootVisualElement.Q<Label>("PlayerName"+(o+1));
                            remove.style.opacity = 0;
                        }

                        Debug.Log("Players Count = " + players.Length);

                        SetBars();
                        break;
                    case "End":
                        Debug.Log("End Set Start");

                        manager.sound_manager.Play("Rank Sound", "End");

                        Debug.Log("End % = Start");

                        names = new string[players.Length];

                        for (int o = 0; o < players.Length; o++)
                        {
                            if (o >= 3) { break; }
                            if(o == 0) { doc.rootVisualElement.Q<Label>("win_name").text = players[o].playerName; }
                            doc.rootVisualElement.Q<Label>((o+1)+"Lugar_Name").text = players[o].playerName;
                        }
                        for (int o = players.Length; o < 3; o++)
                        {
                            var remove = doc.rootVisualElement.Q<Label>((o+1)+"Lugar_Name");
                            remove.style.opacity = 0;
                        }

                        Debug.Log("Players Count = " + players.Length);

                        manager.StartCoroutine(SetEndBars(1.5f));
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

    public void Reset() { time = 0; size = 0; }
    public bool finish() { return time >= time_max; }
    public float getSize()
    {
        return size;
    }
}