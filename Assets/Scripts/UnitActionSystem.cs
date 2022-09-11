using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStart;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitMaskLayer;

    private BaseAction selectedAction;

    private bool isBusy = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one UnitActionController! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() 
    {
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
        SetSelectedUnit(selectedUnit);
    }

    private void Update()
    {
        if (isBusy)
        {
            return;
        }

        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (TryHandleMouseSelection())
        {
            return;
        }

        HandleSelectedAction();
    }

    private void HandleSelectedAction()
    {
        if (!InputManager.Instance.IsMouseButtonDown())
        {
            return;
        }

        GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
        if (!selectedAction.IsValidActionPosition(mouseGridPosition))
        {
            return;
        }

        if (!selectedUnit.TrySpendActionPointsOnAction(selectedAction))
        {
            return;
        }

        SetBusy();
        selectedAction.TakeAction(mouseGridPosition, ClearBusy);
        OnActionStart?.Invoke(this, EventArgs.Empty);
    }

    private bool TryHandleMouseSelection()
    {
        if (InputManager.Instance.IsMouseButtonDown())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            if (Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, unitMaskLayer))
            {
                if (hitInfo.transform.gameObject.TryGetComponent<Unit>(out Unit unit))
                {
                    if (unit == selectedUnit)
                    {
                        // Unit is aleady selected
                        return false;
                    }

                    if (unit.IsEnemy())
                    {
                        // Clicked on an ememy unit
                        return false;
                    }

                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }
        return false;
    }

    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.GetAction<MoveAction>());

        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public void SetSelectedAction(BaseAction baseAction) 
    {
        selectedAction = baseAction;

        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);    
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }

    private void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit deadUnit = sender as Unit;
        if (deadUnit == selectedUnit)
        {
            // Our selected unit died, grab the first available friendly unit instead
            SetSelectedUnit(UnitManager.Instance.GetFirstFriendlyUnitNotEqual(deadUnit));
        }
    }
}
