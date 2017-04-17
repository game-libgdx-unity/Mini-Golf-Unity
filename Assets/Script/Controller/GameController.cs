using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class GameController : MonoBehaviour
{

    #region Fields

    public static int Level = 2;

    [SerializeField]
    private CameraController cameraService;
    [SerializeField]
    private BallController ball;
    [SerializeField]
    private WallGenerator bigWall;
    [SerializeField]
    private PolygonCollider2D polygon2D;
    [SerializeField]
    private UIController gameUI;
    [SerializeField]
    private GoalController goal;

    private int strokes = 0;
    private AIController ai;
    private Vector3 ballPos;

    #endregion

    #region Initialization

    void Start()
    {
        InitializeAI();
        InitializeBallPosition();
        InitializeBall();
        InitializeGoal();
        InitializeUI();
    }

    private void InitializeAI()
    {
        ai = ball.GetComponent<AIController>();
        ai.Status_Changed += (s) =>
        {
            if (s == AIState.AIControlling)
            {
                gameUI.GameStatus = "Computer's Turn";
            }
            else if (s == AIState.PlayerControlling)
            {
                gameUI.GameStatus = "Your Turn";
            }
            else if (s == AIState.PlayerWin)
            {
                gameUI.GameStatus = "You Win!";
            }
            else if (s == AIState.AIWin)
            {
                gameUI.GameStatus = "Computer Win!";
            }
        };
    }

    private void InitializeBall()
    {
        ballPos.z = ballPos.y;
        ballPos.y = 5;
        ball.transform.localPosition = ballPos;
        if (ballPos.x > 0)
        {
            cameraService.Current = CameraType.Second;
        }
        else
        {
            cameraService.Current = CameraType.First;
        }
        ball.OnStopped = () =>
        {
            print("OnStopped");
            if (ai.Status == AIState.PlayerControlling)
                ai.Status = AIState.Thinking;

            if (ai.Status == AIState.AIControlling)
                ai.Status = AIState.PlayerControlling;
        };
        ball.OnHitStarted = () =>
        {
            ball.IsMoving = true;
            Strikes++;
        };
    }

    private void InitializeBallPosition()
    {
        polygon2D.points = bigWall.Points.ToVector2();
        ballPos = new Vector3(0f, 0f, 0f);
        polygon2D.transform.localRotation = Quaternion.identity;
        while ((!polygon2D.bounds.Contains(ballPos)) || Vector3.Distance(goal.transform.localPosition, ballPos) < 60 || Vector3.Distance(goal.transform.localPosition, ballPos) > 80)
        {
            print("calculate");
            ballPos.x = UnityEngine.Random.Range(-100, 100);
            ballPos.y = UnityEngine.Random.Range(-35, 35);
        }

        print(ballPos);
    }

    private void InitializeGoal()
    {
        goal.OnGameFinished = () =>
        {
            if (ai.Status == AIState.PlayerControlling)
            {
                ai.Status = AIState.PlayerWin;
                ball.gameObject.SetActive(false);
            }
        };
    }

    private void InitializeUI()
    {
        gameUI.GameStatus = "Welcome to mini golf";
        gameUI.Strokes = 0;
        if (Level == 1)
        {
            Level = 2;
        }
        else
        {
            Level = 1;
        }
        gameUI.Level = Level;
    }

    #endregion

    #region Public Members

    public void NextLevel()
    {
        SceneManager.LoadScene(0);
    }

    #endregion

    #region Private Members

    public int Strikes
    {
        get
        {
            return strokes;
        }

        set
        {
            strokes = value;
            gameUI.Strokes = strokes;
        }
    }

    #endregion
}
