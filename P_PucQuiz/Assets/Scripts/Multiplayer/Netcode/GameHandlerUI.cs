using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class GameHandlerUI : NetworkBehaviour
{
    [Header("Layouts")]
    [SerializeField] List<VisualTreeAsset> visualLayouts;
    [SerializeField] UIDocument currentDocument; 
    public EventHandler OnStateChanged;
    GameManager _gameManager;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnGameStateChanged += ChangeStateUI;
    }



    public void ChangeStateUI(object sender, EventArgs e)
    {
        switch (_gameManager.CurrentGameState)
        {
            case GameState.DisplayingQuestion:
                StartCoroutine(ShowQuestion(0.5f));
                break;
            case GameState.ShowingResults:
                 
                break;
        } 
    }

    IEnumerator ShowQuestion(float delayTime)
    {
        //DEV.Instance.DevPrint("Showing Questions");
        yield return new WaitForSeconds(delayTime);
        currentDocument = FindAnyObjectByType<UIDocument>();
        //document.visualTreeAsset = questionDocument;

        var textContainer = currentDocument.rootVisualElement.Q<VisualElement>("Container_Pergunta");
        var timerContainer = currentDocument.rootVisualElement.Q<VisualElement>("Container_Timer");
        var answersContainer = currentDocument.rootVisualElement.Q<VisualElement>("GridContainer");

        if (textContainer == null)
        {
            Debug.LogError($"Cant find question text container, current Document: {currentDocument.name}");
        }
        /*
        textContainer.AddToClassList("QuestionTextStart");
        answersContainer.AddToClassList("ScaleUpStart");
        timerContainer.AddToClassList("TimerStart");*/

        textContainer.RemoveFromClassList("QuestionText_Anim");
        answersContainer.RemoveFromClassList("Buttons_Anim"); 
        timerContainer.RemoveFromClassList("TimerText_Anim"); 
    }
 
}
