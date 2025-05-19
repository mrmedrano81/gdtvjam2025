using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent OnStructurePlaced;
    public UnityEvent OnStructureRemoved;

    public void EventTriggered()
    {
        Debug.Log("Event Triggered");
    }
}
