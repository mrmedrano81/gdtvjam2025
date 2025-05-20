using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionHandler : MonoBehaviour
{
    [Header("Settings")]
    public bool HandleCollision = false;
    public bool HandleTriggers = false;

    public List<string> collisionTags = new List<string>();
    public List<string> triggerTags = new List<string>();

    [Header("Events")]
    public UnityEvent OnTriggerEnterEvent;
    public UnityEvent OnTriggerStayEvent;

    public UnityEvent OnCollisonEnterEvent;
    public UnityEvent OnCollisonStayEvent;

    private void OnCollisionEnter(Collision collision)
    {
        if (!HandleCollision) return;

        foreach (string tag in collisionTags)
        {
            if (collision.gameObject.CompareTag(tag))
            {
                OnCollisonEnterEvent?.Invoke();
                HandleOnCollisionEnter(collision);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!HandleCollision) return;

        foreach (string tag in collisionTags)
        {
            if (collision.gameObject.CompareTag(tag))
            {
                OnCollisonStayEvent?.Invoke();
                HandleOnCollisionStay(collision);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!HandleTriggers) return;

        foreach (string tag in triggerTags)
        {
            if (other.gameObject.CompareTag(tag))
            {
                OnTriggerEnterEvent?.Invoke();
                HandleOnTriggerEnter(other);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!HandleTriggers) return;

        foreach (string tag in triggerTags)
        {
            if (other.gameObject.CompareTag(tag))
            {
                OnTriggerStayEvent?.Invoke();
                HandleOnTriggerStay(other);
            }
        }
    }


    /// <summary>
    /// Implement this method to handle trigger stay events in child class.
    /// </summary>
    /// <param name="collision"></param>
    public virtual void HandleOnTriggerStay(Collider other)
    {

    }

    /// <summary>
    /// Implement this method to handle trigger enter events in child class.
    /// </summary>
    /// <param name="collision"></param>
    public virtual void HandleOnTriggerEnter(Collider other)
    {

    }

    /// <summary>
    /// Implement this method to handle collision enter events in child class.
    /// </summary>
    /// <param name="collision"></param>
    public virtual void HandleOnCollisionEnter(Collision collision)
    {
        
    }

    /// <summary>
    /// Implement this method to handle collision stay events in child class.
    /// </summary>
    /// <param name="collision"></param>
    public virtual void HandleOnCollisionStay(Collision collision)
    {

    }
}
