using UnityEngine;
using System.Collections;
using System;

public class GoalController : MonoBehaviour {

    #region Fields

    [SerializeField]
    private GameObject ball;
    private new Collider collider;

    #endregion

    #region Initialization

    private void Start()
    {
        collider = GetComponent<SphereCollider>();
    }

    #endregion

    #region Public Members

    public Action OnGameFinished;

    #endregion

    #region Private Members

    private void FixedUpdate()
    {
        if (collider.bounds.Contains(ball.transform.position))
        {
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            if (OnGameFinished != null)
                OnGameFinished();
        }
    }

    #endregion
}
