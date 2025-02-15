using UnityEngine;
using UnityEngine.UI; // for text
using UnityEngine.SceneManagement; // for detecting scene changes
using System.Collections.Generic;

public class PlacementSystem : MonoBehaviour
{
    private int checkpointIncrementPoint = 0;
    public Text positionText;
    public static List<GameObject> allCars = new List<GameObject>(); // List of all cars - static means all in list are stored in single list

    private void Awake()
    {
        // NEW CODE: Reset the list when the scene is loaded
        if (allCars == null)
        {
            allCars = new List<GameObject>(); // Initialize the list if null
        }
        else
        {
            allCars.Clear(); // Clear the list to avoid old data
        }
    }

    private void Start()
    {
        if (!allCars.Contains(gameObject))
        {
            allCars.Add(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("RaceBlockPoint")) // Check if we hit a RaceBlockPoint
        {
            checkpointIncrementPoint++;
            UpdateMyPosition();
        }
    }

    private void UpdateMyPosition()
    {
        allCars.Sort((CarA, CarB) =>
        {
            int CarA_Points = CarA.GetComponent<PlacementSystem>().checkpointIncrementPoint;  //gotta find script to all the cars adn acces the points in them for placement
            int CarB_Points = CarB.GetComponent<PlacementSystem>().checkpointIncrementPoint;
            return CarB_Points - CarA_Points; // Higher points come first -- If bPoints > aPoints: Put b before a (descending order).
        });

        // Find the position of your car
        int myPosition = allCars.IndexOf(gameObject) + 1; // Add 1 for 1-based ranking

        if (positionText != null)  // null checks if the box is empty in the Unity Inspector
        {
            if (myPosition == 1)
            {
                positionText.text = "1st";
            }
            else if (myPosition == 2)
            {
                positionText.text = "2nd";
            }
            else if (myPosition == 3)
            {
                positionText.text = "3rd";
            }
            else
            {
                positionText.text = $"{myPosition}th";
            }
        }
    }

    private void OnEnable()
    {
        // NEW CODE: Subscribe to the sceneLoaded event to handle scene transitions
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // NEW CODE: Unsubscribe from the sceneLoaded event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // NEW CODE: Clear allCars list when a new scene is loaded
        allCars.Clear();
    }
}
