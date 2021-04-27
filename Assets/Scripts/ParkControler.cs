using UnityEngine;

public class ParkControler : MonoBehaviour
{
    private Light lt;
    public GameObject parkPrefab;
    public bool hasPark = false;
    public string scene;

    void Update()
    {
        lt = GameObject.Find("Spot Light").GetComponent<Light>();
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Park score = " + ScoreScript.parkingScoreValue + 
            " Obstacle score = " + ScoreScript.obstacleHitScoreValue +
            " Wall score = " + ScoreScript.wallHitScoreValue);
        if (scene == "Scene_1" && other && other.gameObject && other.GetComponent<CarAgent>())
        {
                other.GetComponent<CarAgent>().enabled = false;
                lt.color = Color.green;
                hasPark = true;
                Destroy(parkPrefab, 1);
        }

        if (scene == "Scene_2" && other && other.gameObject && other.GetComponent<CarAgentForLevel2>())
        {
            other.GetComponent<CarAgentForLevel2>().enabled = false;
            lt.color = Color.green;
            hasPark = true;
            Destroy(parkPrefab, 1);
        }
    }
}
