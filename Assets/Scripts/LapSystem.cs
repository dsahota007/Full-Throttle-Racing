using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class LapSystem : MonoBehaviour
{
    private int lapCount = 1;
    private int checkpointCount = 0;
    public int maxLaps = 3;
    public Text lapText; 
    private PlacementSystem placementSystem; 

    private void Start()
    {
        UpdateLapPlacement();
        placementSystem = GetComponent<PlacementSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RaceBlockPoint"))
        {
            checkpointCount++;
        }
        else if (other.CompareTag("FinishLine"))
        {
            if (checkpointCount >= 15)
            {
                lapCount++;
                checkpointCount = 0;
                UpdateLapPlacement();

                if (lapCount > maxLaps)
                {
                    SaveLeaderboardData();
                    DisplayEndgameLeaderboard();
                }
            }
        }
    }

    private void UpdateLapPlacement()
    {
        if (lapText != null) // Null check to avoid errors
        {
            lapText.text = $"Lap {lapCount}/{maxLaps}"; 
        }
    }

    public void DisplayEndgameLeaderboard()
    {
        SceneManager.LoadScene("GameOverLeaderboardScene");
    }

    private void SaveLeaderboardData()
    {
        if (PlacementSystem.allCars != null && PlacementSystem.allCars.Count > 0)
        {
            string leaderboardData = "";
            for (int i = 0; i < PlacementSystem.allCars.Count; i++)
            {
                GameObject car = PlacementSystem.allCars[i];
                leaderboardData += $"{i + 1}. {car.name}\n"; 
            }

            PlayerPrefs.SetString("LeaderboardData", leaderboardData);
        }
    }
}
