using UnityEngine;
using System.Collections;
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
    private const int POWER_HIT_UP = 1000;

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
    private bool hit;
    private float hitThreshold = 2f;

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
                    StartCoroutine(AIThinkingRoutine(.5f));
                }
            }
        }
    }

    IEnumerator AIThinkingRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
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
            StartCoroutine(ProjectileRoutine());
        }
        else
        {
            MoveTheBall(Vector3.zero); //dumb AI at level 1
        }
    }

    IEnumerator ProjectileRoutine()
    {
        //make sure the ball is facing the target
        Vector3 targetPos = Vector3.zero;
        ball.transform.LookAt(targetPos);
        //some properties for hitting the ball
        Transform ballT = ball.transform;
        float maxShootRange = 70;
        float maxShootAngle = 60;
        float speed = 20;
        float angle = Mathf.Min(1, Vector3.Distance(ballT.position, targetPos) / maxShootRange) * maxShootAngle;
        ballT.rotation = ballT.rotation * Quaternion.Euler(-angle, 0, 0);

        Vector3 startPos = ballT.position;
        float iniRotX = ballT.rotation.eulerAngles.x;

        float y = Mathf.Min(targetPos.y, startPos.y);
        float totalDist = Vector3.Distance(startPos, targetPos);

        //while the ball havent hit the target
        while (!hit)
        {
            //calculating distance to targetPos
            Vector3 curPos = ballT.position;
            //curPos.y = y;
            float currentDist = Vector3.Distance(curPos, targetPos);
            //if the target is close enough, trigger a hit
            if (currentDist < hitThreshold && !hit)
            {
                print(" Hit()");
                ball.gameObject.SetActive(false);
                hit = true;
                Status = AIState.AIWin;
                break;
            }

            //calculate ratio of distance covered to total distance
            float invR = 1 - Mathf.Min(1, currentDist / totalDist);

            //use the distance information to set the rotation, as the projectile approach target, it will aim straight at the target
            Vector3 wantedDir = targetPos - ballT.position;
            if (wantedDir != Vector3.zero)
            {
                Quaternion wantedRotation = Quaternion.LookRotation(wantedDir);
                float rotX = Mathf.LerpAngle(iniRotX, wantedRotation.eulerAngles.x, invR);

                //make y-rotation always face target
                ballT.rotation = Quaternion.Euler(rotX, wantedRotation.eulerAngles.y, wantedRotation.eulerAngles.z);
            }

            //move forward
            ballT.Translate(Vector3.forward * Mathf.Min(speed * Time.deltaTime, currentDist));

            yield return null;
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
