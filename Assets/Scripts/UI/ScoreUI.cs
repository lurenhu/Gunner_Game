using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    TextMeshProUGUI scoreTextMeshPro;

    private void Awake()
    {
        scoreTextMeshPro = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StaticEventHandler.OnScoreChange += StaticEventHandler_OnScoreChange;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnScoreChange -= StaticEventHandler_OnScoreChange;
    }

    private void StaticEventHandler_OnScoreChange(ScoreChangeArgs scoreChangeArgs)
    {
        scoreTextMeshPro.text = "SCORE " + scoreChangeArgs.score.ToString("###,###0") + "\nMUTIPLIER: x" + scoreChangeArgs.multiplier;
    }
}
