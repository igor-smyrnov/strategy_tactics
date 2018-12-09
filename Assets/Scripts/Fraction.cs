using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Other", menuName = "Fraction")]
public class Fraction : ScriptableObject
{
    new public string name = "Other";
    public Color color = new Color(1,1,1);
    public List<Fraction> allyFractions = new List<Fraction>();
    public List<Fraction> enemyFractions = new List<Fraction>();
}
