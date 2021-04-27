using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System;
using UnityEngine.SceneManagement;

/*
 * Agent Actions - Turn Right, Turn Left, Do Nothing
 * Total Actions - 3
 */
public class CarAgentForLevel2 : Agent
{

    private Rigidbody carRb;
    private Vector3 curPos, curRot;
    private GameObject target;
    private Vector3 portal1Position;
    private Vector3 portal2Position;
    private bool isColliding = false;
    public float accelaration = 50f;
    public float carSpeed = 800f;
    public float carTurnSpeed = 3.0f;
    public bool isStun = true;
    public bool hasPowerUp = false;
    private float bestDistance = 30f;
    private float previousDistance = 0f;
    private GameObject[] obstacles;
    private GameObject[] walls;

    public void setTarget(GameObject target)
    {
        this.target = target;
    }
    public override void Initialize()
    {
        carRb = GetComponent<Rigidbody>();
        cameraFollow.Instance.AddTarget(transform);

        curPos = this.transform.position;
        curRot = this.transform.eulerAngles;

        portal1Position = GameObject.Find("Portal_1").transform.position;
        portal2Position = GameObject.Find("Portal_2").transform.position;
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        walls = GameObject.FindGameObjectsWithTag("Wall");
    }

    public override void OnEpisodeBegin()
    {
        bestDistance = 30f;
    }

    public override void OnActionReceived(float[] actions)
    {
        drive(actions[1]);
        steer(actions[0]);

        for (int i = 0; i < Constants.NUMBER_OF_WALLS_FOR_LEVEL_2; i++)
        {
            float distance = Vector3.Distance(this.transform.position, walls[i].transform.position);
            if (distance < 2.5f)

            {
                AddReward(Constants.CLOSE_TO_WALL_REWARD);
            }
        }

        for (int i = 0; i < Constants.NUMBER_OF_OBSTACLES_FOR_LEVEL_2; i++)
        {
            float distance = Vector3.Distance(this.transform.position, obstacles[i].transform.position);
            if (distance < 2.5f)
            {
                AddReward(Constants.CLOSE_TO_OBSTACLE_REWARD);
            }
        }

        float distanceToTarget = Vector3.Distance(this.transform.position, target.transform.position);
        if (distanceToTarget < 2.5f)
        {
            updateReward(Constants.CLOSEST_TO_TARGET_REWARD, distanceToTarget, false);
        }
        if (distanceToTarget < bestDistance)
        {
            updateReward(Constants.BEST_DISTANCE_TO_TARGET_REWARD, distanceToTarget, true);
        }
        else if (distanceToTarget < previousDistance)
        {
            updateReward(Constants.MOVING_TO_TARGET_REWARD, distanceToTarget, false);
        }
        else
        {
            updateReward(Constants.MOVING_AWAY_TARGET_REWARD, distanceToTarget, false);
        }
        ScoreScript.rewardValue = GetCumulativeReward();
    }

    private void drive(float value)
    {
        int action = Mathf.FloorToInt(value);
        if (action == 1)
        {
            Vector3 forward = transform.forward * carSpeed;
            carRb.AddForce(forward);
        }
        
    }

    private void steer(float value)
    {
        int action = Mathf.FloorToInt(value);
        if (action == 1)
        {
            transform.Rotate(Vector3.up * carTurnSpeed);
        }
        else if (action == 2)
        {
            transform.Rotate(Vector3.down * carTurnSpeed);
        }
    }

    private void updateReward(float reward, float distanceToTarget, bool updateBestDistance)
    {
        AddReward(reward);
        if (updateBestDistance)
        {
            bestDistance = distanceToTarget;
        }
        previousDistance = distanceToTarget;
    }
    
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[1] = 0f;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            actionsOut[1] = 1f;
        }

        actionsOut[0] = 0f;
        if (Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow))
        {
            actionsOut[0] = 1f;
        }
        else if (Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow))
        {
            actionsOut[0] = 2f;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Portal 1
        sensor.AddObservation((portal1Position - this.transform.position).normalized);
        sensor.AddObservation(this.transform.InverseTransformPoint(portal1Position).normalized);
        
        // Portal 2
        sensor.AddObservation((portal2Position - this.transform.position).normalized);
        sensor.AddObservation(this.transform.InverseTransformPoint(portal2Position).normalized);
       
        // Direction 
        sensor.AddObservation((target.transform.position - this.transform.position).normalized);

        // Target
        sensor.AddObservation(this.transform.InverseTransformPoint(target.transform.position).normalized);

        // Obstacles
        for(int i = 0; i < Constants.NUMBER_OF_OBSTACLES_FOR_LEVEL_2; i++) {
            sensor.AddObservation(this.transform.InverseTransformPoint(obstacles[i].transform.position).normalized);
        }

        // Walls
        for(int i = 0; i < Constants.NUMBER_OF_WALLS_FOR_LEVEL_2; i++) {
             sensor.AddObservation(this.transform.InverseTransformPoint(walls[i].transform.position).normalized);
        }

        // Velocity
        sensor.AddObservation(carRb.velocity);
    }

    void Update()
    {
        isColliding = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isColliding)
        {
            return;
        }
        isColliding = true;

        if (other.gameObject.tag == "Park")
        {
            ScoreScript.parkingScoreValue++;
            AddReward(Constants.PARKING_REWARD);
            ScoreScript.rewardValue = GetCumulativeReward();
            EndEpisode();
        }
        if (other.gameObject.tag == "Obstacle" && !hasPowerUp)
        {
            ScoreScript.obstacleHitScoreValue++;
            AddReward(Constants.OBSTACLE_HIT_REWARD);
            ScoreScript.rewardValue = GetCumulativeReward();
            EndEpisode();
            this.transform.position = curPos;
            this.transform.eulerAngles = curRot;
        }
        if (other.gameObject.tag == "Wall" && !hasPowerUp)
        {
            ScoreScript.wallHitScoreValue++;
            AddReward(Constants.WALL_HIT_REWARD);
            ScoreScript.rewardValue = GetCumulativeReward();
            EndEpisode();
            this.transform.position = curPos;
            this.transform.eulerAngles = curRot;
        }
        if (other.gameObject.name == "Portal_1")
        {
            Vector3 offset = new Vector3(1, -1, 0.5f);
            this.transform.position = GameObject.Find("Portal_2").transform.position + offset;
            this.transform.eulerAngles = new Vector3(0, 45, 0);
            if(this.target.transform.position.x > 15f) {
                Debug.Log("Target is on storey 2, moving to storey 2");
                AddReward(Constants.MOVE_TO_CORRECT_STOREY);
            } else {
                Debug.Log("Moving to storey 2 when not needed");
                AddReward(Constants.MOVE_TO_INCORRECT_STOREY);
            }
            ScoreScript.rewardValue = GetCumulativeReward();
        }
        if (other.gameObject.name == "Portal_2")
        {
            Vector3 offset = new Vector3(1, -0.5f, -1);
            this.transform.position = GameObject.Find("Portal_1").transform.position + offset;
            this.transform.eulerAngles = new Vector3(0, 135, 0);
            Debug.Log("Moving from storey 2 to storey 1");
            if(this.target.transform.position.x > 15f) {
                AddReward(Constants.MOVE_TO_INCORRECT_STOREY);
            } else {
                AddReward(Constants.MOVE_TO_CORRECT_STOREY);
            }
            ScoreScript.rewardValue = GetCumulativeReward();
        }
    }
}
