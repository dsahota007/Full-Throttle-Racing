using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LeaderboardDisplay : MonoBehaviour
{
    public Text leaderboardText;

    private void Start()
    {
        string leaderboardData = PlayerPrefs.GetString("LeaderboardData", "No data available");   // Retrieve the saved leaderboard data from PlayerPrefs
        leaderboardText.text = leaderboardData;

    }

}
