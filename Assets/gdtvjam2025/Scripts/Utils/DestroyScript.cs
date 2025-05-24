using UnityEngine;

public class DestroyScript : MonoBehaviour
{
    public void DestroyAfter(float duration)
    {
        Destroy(gameObject, duration);
    }
}
