using System.Collections;
using System.Net;
using UnityEngine;

public class MissileAttack : MonoBehaviour
{
    public float damage;
    public float duration;
    public float lineHeight;
    public float maxLineWidthMult;

    public Transform followTransform;
    public Vector3 targetPosition;
    public LineRenderer targetingBeamPrefab;
    public GameObject explosionPrefab;

    private Coroutine trackCoroutine;

    public void StartTracking()
    {
        LineRenderer targetingBeam = Instantiate(targetingBeamPrefab, transform.position, Quaternion.identity);

        trackCoroutine = StartCoroutine(TrackTarget(targetingBeam));
    }

    private IEnumerator TrackTarget(LineRenderer targetingBeam)
    {
        float time = 0;

        Vector3 aimDirection = Vector3.up;

        Vector3 lineEnd = targetPosition + Vector3.up * lineHeight;

        Vector3 lastAimPosition = targetPosition;

        Vector3 midPoint = (lineEnd + lastAimPosition) / 2;

        if (followTransform != null)
        {
            lineEnd = followTransform.position + Vector3.up * lineHeight;
            midPoint = (followTransform.position + lineEnd) / 2;
            lastAimPosition = followTransform.position;
        }

        targetingBeam.SetPosition(0, lastAimPosition);
        targetingBeam.SetPosition(1, midPoint);
        targetingBeam.SetPosition(2, lineEnd);

        targetingBeam.enabled = true;

        Gradient originalGradient = targetingBeam.colorGradient;

        GradientColorKey[] colorKeys = originalGradient.colorKeys;
        GradientAlphaKey[] alphaKeys = originalGradient.alphaKeys;

        float[] originalAlphas = new float[alphaKeys.Length];

        for (int i = 0; i < alphaKeys.Length; i++)
        {
            originalAlphas[i] = alphaKeys[i].alpha;
        }

        Gradient fadeGradient = new Gradient();
        fadeGradient.SetKeys(colorKeys, alphaKeys);
        targetingBeam.colorGradient = fadeGradient;

        while (time < 1f)
        {
            float t = time;

            for (int i = 0; i < alphaKeys.Length; i++)
            {
                alphaKeys[i].alpha = Mathf.Lerp(0, originalAlphas[i], t);
            }

            fadeGradient.SetKeys(colorKeys, alphaKeys);
            targetingBeam.colorGradient = fadeGradient;

            targetingBeam.widthMultiplier = Mathf.Lerp(maxLineWidthMult, 0, t);

            if (followTransform != null && followTransform.gameObject.activeInHierarchy)
            {
                lastAimPosition = followTransform.position;
                lineEnd = followTransform.position + Vector3.up * lineHeight;
                midPoint = (lastAimPosition + lineEnd) / 2;
            }
            else
            {
                followTransform = null;
            }

            targetingBeam.SetPosition(0, lastAimPosition);
            targetingBeam.SetPosition(1, midPoint);
            targetingBeam.SetPosition(2, lineEnd);

            time += Time.deltaTime / duration;

            yield return null;
        }

        Instantiate(explosionPrefab, lastAimPosition, Quaternion.identity);

        ExplosionScript explosionScript = explosionPrefab.GetComponent<ExplosionScript>();

        explosionScript.damage = damage;

        targetingBeam.enabled = false;

        Destroy(gameObject); // Destroy the explosion effect after a short delay
    }

    private void OnDrawGizmos()
    {
        if (followTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(followTransform.position, followTransform.position + followTransform.up * lineHeight);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(targetPosition, targetPosition + Vector3.up * lineHeight);
    }
}
