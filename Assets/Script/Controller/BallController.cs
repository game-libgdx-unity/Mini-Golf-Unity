using UnityEngine;
using System.Collections;
using System;

public class BallController : MonoBehaviour
{

    #region Fields
    [SerializeField]
    private int speed = 50;
    [SerializeField]
    private int jump = 100;
    [SerializeField]
    private float ball_hit_threshold = .5f;

    private Rigidbody rigidBody;
    private LineRenderer lineRenderer;
    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3[] positions = { -1000 * Vector3.one, -1000 * Vector3.one };
    private bool isMouseDown = false;
    private AIController ai;
    private bool isMoving;

    #endregion

    #region Initialization

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        ai = GetComponent<AIController>();
    }

    #endregion

    #region Public Members

    public Action OnStopped { get; set; }
    public Action OnHitStarted { get; set; }

    public bool IsMoving
    {
        get
        {
            return isMoving;
        }

        set
        {
            isMoving = value;
        }
    }

    public int JumpForce
    {
        get
        {
            return jump;
        }

        set
        {
            jump = value;
        }
    }

    #endregion

    #region Private Members

    void OnMouseDown()
    {
        isMouseDown = true;
    }

    void Update()
    {
        if (ai.Status == AIState.PlayerControlling && isMouseDown && !IsMoving)
        {
            if (Input.GetMouseButton(0))
            {
                positions[0] = transform.position;
                Vector3 curScreenPoint = Input.mousePosition;
                Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
                positions[1] = curPosition;
                lineRenderer.SetPositions(positions);
            }
            if (Input.GetMouseButtonUp(0))
            {
                isMouseDown = false;
                if (Vector2.Distance(new Vector2(positions[0].x, positions[0].z), new Vector2(positions[1].x, positions[1].z)) > ball_hit_threshold) //if player pull the ball powerful enough. 
                {
                    Vector3 power = (positions[0] - positions[1]);
                    power.y = JumpForce;
                    rigidBody.AddForce(power * speed); 
                    if (OnHitStarted != null)
                        OnHitStarted();
                }
                positions[0] = -1000 * Vector3.one;
                positions[1] = -1000 * Vector3.one;
                lineRenderer.SetPositions(positions);
            }
        }
    }

    private void FixedUpdate()
    {
        if (IsMoving)
        {
            if (rigidBody.velocity.magnitude > 0.005f && rigidBody.velocity.magnitude < .01f)
            {
                IsMoving = false;
                rigidBody.angularVelocity = Vector3.zero;
                rigidBody.velocity = Vector3.zero;
                if (OnStopped != null)
                    OnStopped();
            }
        }
    }

    #endregion
}
