using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{
    // Start is called before the first frame update
    public static int parkingScoreValue = 0;
    public static int obstacleHitScoreValue = 0;
    public static int wallHitScoreValue = 0;
    public static float rewardValue = 0;

    Text parkingScore;
    Text obstacleHitScore;
    Text wallHitScore;
    Text reward;
    void Start()
    {
        parkingScore = GameObject.Find("ParkingScoreText").GetComponent<Text>();
        obstacleHitScore = GameObject.Find("ObstacleHitScoreText").GetComponent<Text>();
        wallHitScore = GameObject.Find("WallHitScoreText").GetComponent<Text>();
        reward = GameObject.Find("RewardText").GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        parkingScore.text = "Parking Score: " + parkingScoreValue;
        obstacleHitScore.text = "Obstacle Hit Score: " + obstacleHitScoreValue;
        wallHitScore.text = "Wall Hit Score: " + wallHitScoreValue;
        reward.text = "Cumulative Reward: " + rewardValue;

    }
}
