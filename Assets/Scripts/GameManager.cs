using UnityEngine;

public class GameManager : MonoBehaviour
{

    #region Singleton
    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    #endregion

    public Fraction playerFraction;
    private UnitController[] allUnits = { };

    Camera cam;
    SquadManager squadManager;

    public float stoppingDistance = 0;

    void Start()
    {
        cam = Camera.main;
        squadManager = SquadManager.instance;
        allUnits = FindObjectsOfType<UnitController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                Transform transform = hit.collider.GetComponent<Transform>();
                if (transform != null)
                {
                    UnitController unit = transform.gameObject.GetComponent<UnitController>();
                    if (unit) SelectOne(unit);
                    else UnselectAll();
                }
            }
        }


        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                if (!squadManager.IsEmpty())
                {
                    UnitController targetUnit = hit.collider.GetComponent<UnitController>();

                    if (targetUnit)
                        squadManager.MoveSquadToPoint(stoppingDistance, hit.point, targetUnit);
                    else
                        squadManager.MoveSquadToPoint(stoppingDistance, hit.point);
                }
            }
        }
    }

    void SelectOne(UnitController targetUnit)
    {
        UnselectAll();

        if (targetUnit.fraction == playerFraction && !targetUnit.isSelected)
            squadManager.AddToSelected(targetUnit);

        else if (!targetUnit.isSelected)
            targetUnit.isSelected = true;
    }

    void UnselectAll()
    {
        foreach (UnitController unit in allUnits)
        {
            if (unit.fraction == playerFraction)
                squadManager.RemoveFromSelected(unit);

            else unit.isSelected = false;
        }
    }
}
