using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;

public class FootballAgent : Agent
{

    public enum Team
    {
        Blue = 0,
        Red = 1
    }

    [HideInInspector]
    public Team currentTeam;

    public GameObject ball;
    private Rigidbody ballRigidbody;
    private Rigidbody agentRigidbody;

    float kickPower;
    int playerIndex;
    public Field footballField;
    public float BallTouchReward = 0.5f;
    public float sidewaysSpeed = 0.3f;
    public float forwardSpeed = 1.3f;
    public Transform agentPosition;

    int timePenalty = 0;

    BehaviorParameters behaviourParameters;
    EnvironmentParameters resetParams;

    public override void Initialize()
    {
        ballRigidbody = ball.GetComponent<Rigidbody>();
        behaviourParameters = gameObject.GetComponent<BehaviorParameters>();
        if (behaviourParameters.TeamId == (int)Team.Blue)
        {
            currentTeam = Team.Blue;

        }
        else
        {
            currentTeam = Team.Red;
        }
        agentRigidbody = GetComponent<Rigidbody>();
        agentRigidbody.maxAngularVelocity = 500.0f;

        PlayerState playerState = new PlayerState
        {
            playerRigidbody = agentRigidbody,
            startingPos = transform.position,
            playerScript = this,
        };
        footballField.playerStates.Add(playerState);
        playerIndex = footballField.playerStates.IndexOf(playerState);
        playerState.playerIndex = playerIndex;

        resetParams = Academy.Instance.EnvironmentParameters;
    }

    public void Move(ActionSegment<int> action)
    {
        Vector3 moveDirection = Vector3.zero;
        Vector3 rotation = Vector3.zero;

        kickPower = 0.0f;

        int forwardAxis = action[0];
        int sidewaysAxis = action[1];
        int rotationAxis = action[2];

        switch(forwardAxis)
        {
            case 1:
                moveDirection = transform.forward * forwardSpeed;
                kickPower = 1f;
                break;
            case 2:
                moveDirection = transform.forward * -forwardSpeed;
                break;
        }

        switch (sidewaysAxis)
        {
            case 1:
                moveDirection = transform.right * sidewaysSpeed;
                break;
            case 2:
                moveDirection = transform.right * -sidewaysSpeed;
                break;
        }

        switch (rotationAxis)
        {
            case 1:
                rotation = transform.up * -1f;
                break;
            case 2:
                rotation = transform.up * 1f;
                break;
        }

        transform.Rotate(rotation, Time.deltaTime * 100.0f);
        agentRigidbody.AddForce(moveDirection * forwardSpeed, ForceMode.VelocityChange);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Own Positions
        sensor.AddObservation(this.gameObject.transform.position);
        sensor.AddObservation(this.gameObject.transform.rotation);

        // Relative position off ball to agent
        sensor.AddObservation(ball.transform.position - this.gameObject.transform.position);
        sensor.AddObservation(ballRigidbody.velocity);
    }


    //Add time penalties here
    public override void OnActionReceived(ActionBuffers actions)
    {
        Move(actions.DiscreteActions);
    }

    //public override void Heuristic(in ActionBuffers actionsOut)
    //{

    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            AddReward(BallTouchReward);
            Vector3 direction = collision.contacts[0].point - transform.position;
            direction = direction.normalized;
            collision.gameObject.GetComponent<Rigidbody>().AddForce(direction * kickPower);
        }
    }

    public override void OnEpisodeBegin()
    {
        timePenalty = 0;

        BallTouchReward = resetParams.GetWithDefault("BallTouch", 0);

        if (currentTeam == Team.Blue)
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        }
        transform.position = agentPosition.position;
        agentRigidbody.velocity = Vector3.zero;
        agentRigidbody.angularVelocity = Vector3.zero;
        SetResetParameters();
    }

    public void SetResetParameters()
    {
        footballField.RestartScenario();
    }
}
