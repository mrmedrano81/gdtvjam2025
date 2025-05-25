using UnityEngine;

public class HQScript : MonoBehaviour
{
    public void OnDefeat()
    {
        GameState.Instance.EndGame();
    }
}
