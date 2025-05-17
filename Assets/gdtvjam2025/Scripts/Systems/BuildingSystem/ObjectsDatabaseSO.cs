using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ObjectsDatabase", menuName = "ScriptableObjects/ObjectsDatabase", order = 1)]
public class ObjectsDatabaseSO : ScriptableObject
{
    public List<ObjectData> objectsData = new List<ObjectData>();
}

[System.Serializable]
public class ObjectData
{
    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField]
    public int ID { get; private set; }

    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;

    [field: SerializeField]
    public GameObject prefab { get; private set; }

}