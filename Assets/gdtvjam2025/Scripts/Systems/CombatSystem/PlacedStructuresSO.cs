using UnityEngine;
using System.Collections.Generic;

public class PlacedStructure
{
    public int ID;
    public GameObject StructureObject;
    public EStructureType StructureType;
}

[CreateAssetMenu(fileName = "PlacedStructures", menuName = "ScriptableObjects/PlacedStructuresSO")]
public class PlacedStructuresSO : ScriptableObject
{
    [SerializeField] private List<PlacedStructure> placedStructures = new List<PlacedStructure>();

    public void AddStructure(PlacedStructure structure)
    {
        placedStructures.Add(structure);
    }

    public void RemoveStructure(PlacedStructure structure)
    {
        placedStructures.Remove(structure);
    }
}
