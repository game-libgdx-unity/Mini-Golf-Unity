using UnityEngine;
using System.Collections;

public class Mud : MonoBehaviour
{

    #region Fields

    Rigidbody rigidBody;

    #endregion

    #region Initialization

    void Start()
    {
        rigidBody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
    }

    #endregion

    #region Public Members


    #endregion

    #region Private Members

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            print("OnCollisionEnter");
            if (rigidBody)
                rigidBody.angularDrag = 20;
        }


    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            if (rigidBody)
                rigidBody.angularDrag = 3;
        }
    }


    #endregion
}
