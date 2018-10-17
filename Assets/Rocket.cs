using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Rocket : MonoBehaviour {

    Rigidbody rigidBody;
    AudioSource audioSource;
    [SerializeField] Text achievementText;

    enum State { Alive, Dying, Transcending };

    State state = State.Alive;
    bool isColliding = true;

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip success;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem successParticles;

    // Use this for initialization
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        if (SceneManager.GetActiveScene().buildIndex == 0) {
            achievementText.text = "";
        }

    }

    // Update is called once per frame
    void Update() {
        if (state == State.Alive) {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if (Debug.isDebugBuild) {
            RespondToCheatKeys();
        }
    }

    private void RespondToCheatKeys() {
        if (Input.GetKeyDown(KeyCode.L)) {
            LoadNextScene();
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            audioSource.PlayOneShot(success);
            isColliding = !isColliding;
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (state != State.Alive) { return; }

        switch (collision.gameObject.tag) {
            case "Friendly":
                //do nothing
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                if (isColliding) {
                    StartDeathSequence();
                }
                break;
        }
    }

    private void StartDeathSequence() {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        if (SceneManager.GetActiveScene().buildIndex == 0) {
            achievementText.text = "ACHIEVEMENT UNLOCKED!\nDied on the title screen";
        }
        Invoke("RestartGame", levelLoadDelay);
    }

    private void StartSuccessSequence() {
        state = State.Transcending;
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextScene() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = 0;
        if (currentSceneIndex < SceneManager.sceneCountInBuildSettings - 1) {
            nextSceneIndex = currentSceneIndex + 1;
        }
        SceneManager.LoadScene(nextSceneIndex); //todo allow for more than 2 levels
    }

    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) {
            ApplyThrust();
        } else {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust() {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying) {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput() {
        rigidBody.freezeRotation = true; //take manual control of the rotation
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false; //resume physics control
    }


}
