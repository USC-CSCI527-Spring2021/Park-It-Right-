using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public GameObject[] parkPrefabs;
    public GameObject[] startPoints;
    public GameObject target;
    public GameObject car;
    public List<Vector3> goals;

    void Start()
    {
        
        // KM: We will stick to only BatMobil for our project 
        int randomCar = 0;
        int randomCarSpot = Random.Range(0, startPoints.Length);

        car = Instantiate(carPrefabs[randomCar], startPoints[randomCarSpot].transform.position, startPoints[randomCarSpot].transform.rotation);
        Instantiate(parkPrefabs[0], target.transform.position, target.transform.rotation);

        ParkControler.FindObjectOfType<ParkControler>().scene = SceneManager.GetActiveScene().name;

        goals = new List<Vector3>();
        if (SceneManager.GetActiveScene().name  == "Scene_1")
        {
            CarAgent.FindObjectOfType<CarAgent>().isStun = true;
            CarAgent.FindObjectOfType<CarAgent>().setTarget(target);
            goals.Add(new Vector3(-11f, 0.3f, -5f));
            goals.Add(new Vector3(-3.2f, 0.3f, 12f));
            goals.Add(new Vector3(8f, 0.3f, 12f));
        }
        else if (SceneManager.GetActiveScene().name == "Scene_2")
        {
            CarAgentForLevel2.FindObjectOfType<CarAgentForLevel2>().isStun = true;
            CarAgentForLevel2.FindObjectOfType<CarAgentForLevel2>().setTarget(target);
           // goals.Add(new Vector3(-3.2f, 0.3f, 12f));
            // Points on storey 1
            //goals.Add(new Vector3(8f, 0.3f, 12f));
            // Points on storey 2
            goals.Add(new Vector3(78.42f, 0f, 12.24f));

        }
    }


    Vector3 getGoalLocation()
    {
        return SceneManager.GetActiveScene().name == "Scene_1" ? goals[Random.Range(0, 3)] : goals[0];
    }

    void Update()
    {
        ParkControler.FindObjectOfType<ParkControler>().scene = SceneManager.GetActiveScene().name;
        if (ParkControler.FindObjectOfType<ParkControler>().hasPark == true)
        {
            Destroy(car);
            int randomCar = 0;
            int randomCarSpot = Random.Range(0, startPoints.Length);

            target.transform.position = getGoalLocation();
            car = Instantiate(carPrefabs[randomCar], startPoints[randomCarSpot].transform.position, startPoints[randomCarSpot].transform.rotation);
            Instantiate(parkPrefabs[0], target.transform.position, target.transform.rotation);

            if (SceneManager.GetActiveScene().name == "Scene_1")
            {
                CarAgent.FindObjectOfType<CarAgent>().isStun = true;
                CarAgent.FindObjectOfType<CarAgent>().setTarget(target);
            }
            else if (SceneManager.GetActiveScene().name == "Scene_2")
            {
                CarAgentForLevel2.FindObjectOfType<CarAgentForLevel2>().isStun = true;
                CarAgentForLevel2.FindObjectOfType<CarAgentForLevel2>().setTarget(target);
            }
            cameraFollow.FindObjectOfType<cameraFollow>().targets.RemoveAt(0);
        }
    }
}
