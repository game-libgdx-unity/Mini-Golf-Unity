using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;
using System.Collections.Generic;

public enum AIState
{
    Thinking,
    AIControlling,
    PlayerControlling,
    AIWin,
    PlayerWin
}

public class AIController : MonoBehaviour
{

    #region 

    private const int POWER_HIT = 40;
    private const int POWER_HIT_UP = 1500;

    [SerializeField]
    private Transform goal;
    [SerializeField]
    private WallGenerator[] walls;
    private List<Vector3> blankPoints;
    private AIState status;
    private BallController ball;
    [SerializeField]
    private float jump = 30;
    private Rigidbody rigidBody;
    
    #endregion

    #region Initialization

    void Start()
    {
        ball = GetComponent<BallController>();
        rigidBody = GetComponent<Rigidbody>();
        InitializeBlankPoints();
        Status = AIState.PlayerControlling; //start with Player turn
    }

    private void InitializeBlankPoints()
    {
        blankPoints = new List<Vector3>();
        foreach (WallGenerator wall in walls)
        {
            foreach (Vector3 blankPoint in wall.BlankPosition)
            {
                blankPoints.Add(blankPoint);
            }
        }
    }

    #endregion

    #region Public Members

    public Action<AIState> Status_Changed;

    public AIState Status
    {
        get
        {
            return status;
        }
        set
        {
            if (value != status)
            {
                status = value;
                print("Status :" + status);
                if (Status_Changed != null)
                    Status_Changed(status);

                if (status == AIState.Thinking)
                {
                    DOVirtual.DelayedCall(1f, () => //AI takes 1s to think the next move
                    {
                        Status = AIState.AIControlling;
                        if (CanDirectHitGoal)
                        {
                            print("Great");
                            PerformDirectHit();
                        }
                        else
                        {
                            print("Do AI");
                            MoveBallToNearestBlankPoint();
                        }
                        if (ball.OnHitStarted != null)
                        {
                            ball.OnHitStarted();
                        }
                    });
                }
            }
        }
    }


    #endregion

    #region Private Members

    private void MoveBallToNearestBlankPoint()
    {
        Vector3 target = FindNearestBlankPosition();
        MoveTheBall(target);
    }

    private void MoveTheBall(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        rigidBody.AddForce(direction.normalized * direction.magnitude * POWER_HIT + Vector3.up * POWER_HIT_UP);
    }

    private void PerformDirectHit()
    {
        if (GameController.Level == 2) // Perfect AI at level 2
        {
            transform.DOJump(Vector3.zero, 30, 1, 2).OnComplete(() => Status = AIState.AIWin);
        }
        else
        {
            MoveTheBall(Vector3.zero); //dumb AI at level 1
        }
    }

    private Vector3 FindNearestBlankPosition()
    {
        float dis = 10000f;
        Vector3 output = Vector3.zero;
        foreach (Vector3 pos in blankPoints)
        {
            float d2 = Vector3.Distance(pos, transform.localPosition);
            if (d2 < dis)
            {
                output = pos;
                dis = d2;
            }
        }
        blankPoints.Remove(output);
        return output;
    }

    private bool CanDirectHitGoal
    {
        get
        {
            Vector3 direction = (goal.transform.localPosition - transform.localPosition);
            float distance = Vector3.Distance(goal.transform.localPosition, transform.localPosition);
            Ray ray = new Ray(transform.localPosition, direction);
            Debug.DrawRay(transform.localPosition, direction, Color.red);
            RaycastHit info;
            if (Physics.Raycast(ray, out info, distance))
            {
                print(info.collider.tag);
                if (info.collider.tag.Equals("wall"))
                    return false;
            }
            return true;
        }

    }
    
    #endregion
}
