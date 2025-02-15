using UnityEngine;
using UnityEngine.UI; // Required for Image component

public class Minimap : MonoBehaviour
{
    public Camera minimapCamera;         // The Minimap Camera
    public RectTransform minimapCanvas; // RectTransform of the Minimap RawImage
    public GameObject markerPrefab;     // Prefab for the minimap markers
    public Vector2 positionOffset;      // Manual offset for fine-tuning marker positions
    public float scaleFactor = 1f;      // Scale factor to fit the markers inside the minimap

    private GameObject[] cars;          // Array of cars
    private GameObject[] markers;       // Array of markers for each car
    public GameObject playerCar;        // Assign your player's car in the Inspector
    public Color playerColor = Color.red; // Color for your marker
    public Color aiColor = Color.blue;  // Color for AI markers

    private void Start()
    {
        // Find all cars (tagged as "Car")
        cars = GameObject.FindGameObjectsWithTag("Car");

        // Create markers for each car
        markers = new GameObject[cars.Length];
        for (int i = 0; i < cars.Length; i++)
        {
            markers[i] = Instantiate(markerPrefab, minimapCanvas); // Create marker inside minimap canvas

            // Check if this is the player's car
            if (cars[i] == playerCar)
            {
                // Set the player's marker color
                markers[i].GetComponent<Image>().color = playerColor;
            }
            else
            {
                // Set AI car marker color
                markers[i].GetComponent<Image>().color = aiColor;
            }
        }
    }

    private void Update()
    {
        RectTransform minimapRect = minimapCanvas.GetComponent<RectTransform>();

        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i] != null)
            {
                // Get car's position in world space
                Vector3 carPosition = cars[i].transform.position;

                // Convert world position to viewport position (values 0-1)
                Vector3 viewportPos = minimapCamera.WorldToViewportPoint(carPosition);

                // Map viewport position to minimap RectTransform
                Vector2 minimapPos = new Vector2(
                    (viewportPos.x - 0.5f) * minimapRect.rect.width * scaleFactor,
                    (viewportPos.y - 0.5f) * minimapRect.rect.height * scaleFactor
                );

                // Apply manual offset for fine-tuning
                minimapPos += positionOffset;

                // Update the marker's position
                markers[i].GetComponent<RectTransform>().anchoredPosition = minimapPos;
            }
        }
    }
}
