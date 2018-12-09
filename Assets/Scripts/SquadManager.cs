using UnityEngine;
using System.Collections.Generic;

public class SquadManager : MonoBehaviour {

    #region Singleton

    public static SquadManager instance;

    void Awake()
    {
        instance = this;
    }

    #endregion

    private List<UnitController> selectedUnits = new List<UnitController>();
    
    void Start () {
		
	}
	
	void Update () {
		
	}

    public void AddToSelected(UnitController unit)
    {
        if (!selectedUnits.Contains(unit))
        {
            selectedUnits.Add(unit);
            unit.isSelected = true;
        }
    }

    public void RemoveFromSelected(UnitController unit)
    {
        if (selectedUnits.Contains(unit))
        {
            selectedUnits.Remove(unit);
            unit.isSelected = false;
        }
    }

    public bool IsEmpty()
    {
        return selectedUnits.Count == 0;
    }

    public void MoveSquadToPoint(float stoppingDistance, Vector3 point, UnitController targetUnit = null)
    {
        foreach (UnitController unit in selectedUnits)
        {
            if (targetUnit)
                unit.FollowTarget(targetUnit, stoppingDistance);
            else
                unit.MoveToPoint(point);
        }
    }
}
