using Unity.AI.Navigation;
using UnityEngine;

public class NavmeshManager : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;


    public void RebuildNavmesh()
    {
        Debug.Log("NavmeshRebuilt");
        navMeshSurface.BuildNavMesh();
    }
}
