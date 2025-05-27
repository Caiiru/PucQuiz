using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class GameHandlerUI : NetworkBehaviour
{

    public UIDocument document;

    public EventHandler OnStateChanged;

    void Start()
    {
        GameManager.Instance.CurrentGameState.OnValueChanged += ChangeStateUI;
    }

    public void ChangeStateUI(GameState previousState, GameState newState)
    {
        switch (newState)
        {
            case GameState.DisplayingQuestion:
                Event_PucQuiz.scene_actualy = "Quiz";
                ShowQuestion();
                break;
        }
    }

    private void ShowQuestion()
    {
        document = FindAnyObjectByType<UIDocument>();
        var textContainer = document.rootVisualElement.Q<VisualElement>("Container_Pergunta");
        var timerContainer = document.rootVisualElement.Q<VisualElement>("Container_Timer");
        var answersContainer = document.rootVisualElement.Q<VisualElement>("Container-Resposta1");
        textContainer.RemoveFromClassList("QuestionTextStart");
        answersContainer.RemoveFromClassList("ScaleUpStart"); 
        timerContainer.RemoveFromClassList("TimerStart"); 
    }
}
