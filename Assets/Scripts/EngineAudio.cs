using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(Rigidbody))]
public class EngineAudio : MonoBehaviour
{
    private Rigidbody rb;              // Rigidbody of the car
    private AudioSource audioSource;   // Single AudioSource for playback

    public AudioClip drivingClip;      // Assign the driving sound in Inspector
    public AudioClip idleClip;         // Assign the idle sound in Inspector

    public float minPitch = 1f;        // Minimum pitch of the driving sound
    public float maxPitch = 3f;        // Maximum pitch of the driving sound
    public float maxSpeed = 200f;      // Maximum speed to normalize pitch

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        audioSource.loop = true;       // Enable looping for both sounds
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        float speedOfTheCar = rb.velocity.magnitude * 3.6f;

        if (speedOfTheCar > 0.1f) // Threshold to check if the car is moving
        {
            if (audioSource.clip != drivingClip) // Switch to driving sound
            {
                audioSource.clip = drivingClip;
                audioSource.Play();
            }

            // Adjust the pitch based on the car's speed
            float normalizedSpeed = Mathf.Clamp(speedOfTheCar / maxSpeed, 0f, 1f);
            audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, normalizedSpeed);
        }
        else
        {
            if (audioSource.clip != idleClip) // Switch to idle sound
            {
                audioSource.clip = idleClip;
                audioSource.Play();
            }

            // Reset pitch to default for idle sound
            audioSource.pitch = 1f;
        }
    }
}
