using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] Button endTurnButton;
    [SerializeField] TextMeshProUGUI turnNumberText;
    [SerializeField] GameObject enemyTurnVisual;

    private void Start()
    {
        endTurnButton.onClick.AddListener(() => {
            TurnSystem.Instance.NextTurn();
        });
        
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        UpdateTurnNumber();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void UpdateTurnNumber()
    {
        turnNumberText.text = "Turn Number: " + TurnSystem.Instance.GetTurnNumber();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnNumber();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void UpdateEnemyTurnVisual()
    {
        enemyTurnVisual.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }

    private void UpdateEndTurnButtonVisibility()
    {
        endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }
}
