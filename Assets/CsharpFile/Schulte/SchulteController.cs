using System;
using UnityEngine;

public class SchulteController : MonoBehaviour
{
    public int currentTarget { get; private set; } = 1;
    public event Action<int> OnTargetChanged;
    public bool inputEnabled { get; private set; } = false;

    private int maxNumber;
    public event Action OnGameFinished;

    private GridCell[] cells;

    public int correctClicks { get; private set; } = 0;
    public int totalClicks { get; private set; } = 0;


    void Awake()
    {
        //Initialization if needed
    }

    void Update()
    {
        if (!inputEnabled) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleGlobalClick();
        }
    }

    public void EnableInput()
    {
        inputEnabled = true;
    }

    public void DisableInput()
    {
        inputEnabled = false;
    }


    bool IsCellDead(GridCell cell)
    {
        var health = cell.GetComponent<Ilumisoft.HealthSystem.Health>();
        return health == null || !health.IsAlive;
    }

    public void StartNewGame(int max)
    {
        //游戏开始锁鼠标
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        maxNumber = max;
        currentTarget = 1;
        inputEnabled = true;
        correctClicks = 0;
        totalClicks = 0;
        cells = FindObjectsOfType<GridCell>();

        ActivateCurrentCell();
        OnTargetChanged?.Invoke(currentTarget);
    }

    public void ResetTarget()
    {
        currentTarget = 1;
        correctClicks = 0;
        totalClicks = 0;
        OnTargetChanged?.Invoke(currentTarget);
    }

    void HandleGlobalClick()
    {
        GridCell target = GetCurrentTargetCell();
        if (target == null) return;

        totalClicks++;
        correctClicks++;

        target.RegisterCorrect();

        if (target.IsDead)
        {
            target.Deactivate();
            currentTarget++;

            if (currentTarget > maxNumber)
            {
                inputEnabled = false;

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                OnGameFinished?.Invoke();
            }
            else
            {
                ActivateCurrentCell();
                OnTargetChanged?.Invoke(currentTarget);
            }
        }
    }

    GridCell GetCurrentTargetCell()
    {
        foreach (var cell in cells)
        {
            if (cell.number == currentTarget)
                return cell;
        }
        return null;
    }


    void ActivateCurrentCell()
    {
        if (cells == null || cells.Length == 0)
        {
            Debug.LogWarning("ActivateCurrentCell: cells is empty!");
            return;
        }

        foreach (var cell in cells)
            cell.Deactivate();

        foreach (var cell in cells)
        {
            if (cell.number == currentTarget)
            {
                cell.Activate();
                Debug.Log($"Activated cell {cell.number}");
                break;
            }
        }
    }

}
