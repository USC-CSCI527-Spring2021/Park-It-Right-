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
public class CarAgent : Agent
{

    private Rigidbody carRb;
    private Vector3 curPos, curRot;
    private GameObject target;
    private bool isColliding = false;
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

        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        walls = GameObject.FindGameObjectsWithTag("Wall");
    }

    public override void OnEpisodeBegin()
    {
        bestDistance = 30f;
    }

    public override void OnActionReceived(float[] actions)
    {
        if (Mathf.FloorToInt(actions[0]) == 0)
        {
            Vector3 forward = (transform.forward * carSpeed);
            carRb.AddForce(forward);
        }
        else if (Mathf.FloorToInt(actions[0]) == 1)
        {
            transform.Rotate(Vector3.up * carTurnSpeed);
        }
        else
        {
            transform.Rotate(Vector3.down * carTurnSpeed);
        }

        float[] distanceToWalls = { 0, 0, 0, 0 };
        for (int i = 0; i < 4; i++)
        {
            distanceToWalls[i] = Vector3.Distance(this.transform.position, walls[i].transform.position);
            if (distanceToWalls[i] < 2f)
            {
                AddReward(Constants.CLOSE_TO_WALL_REWARD);
            }
        }

        float[] distanceToObstacles = { 0, 0, 0 };
        for (int i = 0; i < 3; i++)
        {
            distanceToObstacles[i] = Vector3.Distance(this.transform.position, obstacles[i].transform.position);
            if (distanceToObstacles[i] < 2f)
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
        actionsOut[0] = 0f;
        if (Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow))
        {
            actionsOut[0] = 1f;
        }
        if (Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow))
        {
            actionsOut[0] = 2f;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.InverseTransformPoint(target.transform.position).normalized);
        sensor.AddObservation(this.transform.InverseTransformPoint(obstacles[0].transform.position).normalized);
        sensor.AddObservation(this.transform.InverseTransformPoint(obstacles[1].transform.position).normalized);
        sensor.AddObservation(this.transform.InverseTransformPoint(obstacles[2].transform.position).normalized);
        sensor.AddObservation(this.transform.InverseTransformPoint(walls[0].transform.position).normalized);
        sensor.AddObservation(this.transform.InverseTransformPoint(walls[1].transform.position).normalized);
        sensor.AddObservation(this.transform.InverseTransformPoint(walls[2].transform.position).normalized);
        sensor.AddObservation(this.transform.InverseTransformPoint(walls[3].transform.position).normalized);
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
            AddReward(5f);
            ScoreScript.rewardValue = GetCumulativeReward();
            EndEpisode();
        }
        if (other.gameObject.tag == "Obstacle" && !hasPowerUp)
        {
            ScoreScript.obstacleHitScoreValue++;
            AddReward(-0.5f);
            ScoreScript.rewardValue = GetCumulativeReward();
            EndEpisode();
            this.transform.position = curPos;
            this.transform.eulerAngles = curRot;
        }
        if (other.gameObject.tag == "Wall" && !hasPowerUp)
        {
            ScoreScript.wallHitScoreValue++;
            AddReward(-0.5f);
            ScoreScript.rewardValue = GetCumulativeReward();
            EndEpisode();
            this.transform.position = curPos;
            this.transform.eulerAngles = curRot;
        }
        if (other.gameObject.name == "Portal_1")
        {
            Debug.Log("Portal1");
            Vector3 offset = new Vector3(1, -1, 0.5f);
            this.transform.position = GameObject.Find("Portal_2").transform.position + offset;
            this.transform.eulerAngles = new Vector3(0, 45, 0);
        }
        if (other.gameObject.name == "Portal_2")
        {
            Debug.Log("Portal2");
            Vector3 offset = new Vector3(1, -0.5f, -1);
            this.transform.position = GameObject.Find("Portal_1").transform.position + offset;
            this.transform.eulerAngles = new Vector3(0, 135, 0);
        }
    }
}
