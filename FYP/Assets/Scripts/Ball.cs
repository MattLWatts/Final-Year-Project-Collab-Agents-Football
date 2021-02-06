using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [HideInInspector]
    public Field soccerField;

    private void Start()
    {
        soccerField = new Field();
    }

    public void OnCollisionEnter(Collision collision)
    {
        // Blue team has score as ball is in red goal
        if (collision.gameObject.CompareTag("RedGoal"))
        {
            soccerField.GoalScored(FootballAgent.Team.Blue);
        }
        // Red team has scored as ball is in red goal
        if (collision.gameObject.CompareTag("BlueGoal"))
        {
            soccerField.GoalScored(FootballAgent.Team.Red);
        }
    }
}
