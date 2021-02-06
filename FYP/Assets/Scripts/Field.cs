using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerState
{
    public int playerIndex;
    public Rigidbody playerRigidbody;
    public Vector3 startingPos;
    public FootballAgent playerScript;
    public float ballPosReward;
}
public class Field : MonoBehaviour
{

    public List<PlayerState> playerStates = new List<PlayerState>();
    public GameObject ball;
    private Ball ballCont;
    public GameObject grass;
    public GameObject ballSpawnLocation;

    Ball ballTouched;

    private Rigidbody ballRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        ballRigidbody = ball.GetComponent<Rigidbody>();
    }

    public void GoalScored(FootballAgent.Team scoringTeam)
    {
        foreach (PlayerState playerState in playerStates)
        {
            if (playerState.playerScript.currentTeam == scoringTeam)
            {
                playerState.playerScript.AddReward(1f);
            }
            else
            {
                playerState.playerScript.AddReward(-0.7f);
            }
            playerState.playerScript.EndEpisode();
        }
    }

    public void RestartScenario()
    {
        ball.transform.position = ballSpawnLocation.transform.position;
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
    }
}
