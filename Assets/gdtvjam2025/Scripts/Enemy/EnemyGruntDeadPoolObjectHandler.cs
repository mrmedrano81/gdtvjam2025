using System.Collections;
using UnityEngine;

public class EnemyGruntDeadPoolObjectHandler : PoolObjectHandler
{
    [Header("Death Settings")]
    public float sinkRate = 0.1f;
    public float sinkDelay = 0.5f;

    private Coroutine sinkCoroutine;

    public override void OnObjectSpawned()
    {
        base.OnObjectSpawned();

        //Debug.Log($"OnObjectSpawned {gameObject.name} called");

        sinkCoroutine = StartCoroutine(Sink());
    }

    public override void OnReturnToPool()
    {
        StopCoroutine(sinkCoroutine);
        sinkCoroutine = null;

        //Debug.Log($"Returning {gameObject.name} to pool");

        base.OnReturnToPool();
    }

    private IEnumerator Sink()
    {
        yield return new WaitForSeconds(sinkDelay);

        while (transform.position.y > -10f)
        {
            //Debug.Log($"Sinking {gameObject.name} at {transform.position}");

            transform.position += Vector3.down * sinkRate * Time.deltaTime;
            yield return null;
        }
    }
}
