using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    // global variables
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    public float speed = 10;
    private Rigidbody2D marioBody;
    public float upSpeed = 10;
    public float maxSpeed = 20;
    private bool onGroundState = true;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    public Animator marioAnimator;

    // other variables
    public TextMeshProUGUI scoreText;
    public GameObject enemies;

    public AudioSource marioAudio;

    public AudioClip marioDeath;
    public float deathImpulse = 15;

    public Transform gameCamera;

    // state
    [System.NonSerialized]
    public bool alive = true;
    void PlayDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }
    void PlayJumpSound()
    {
        // play jump sound
        marioAudio.PlayOneShot(marioAudio.clip);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set to be 60 FPS
        Application.targetFrameRate = 60;
        marioBody = GetComponent<Rigidbody2D>();

        marioSprite = GetComponent<SpriteRenderer>();

        // Hide GameOver panel at start
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        marioAnimator.SetBool("onGround", onGroundState);

    }

    // Update is called once per frame
    void Update()
    {
        // toggle state
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;

            if (marioBody.linearVelocity.x > 0.1f)
                marioAnimator.SetTrigger("onSkid");

        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;

            if (marioBody.linearVelocity.x < -0.1f)
                marioAnimator.SetTrigger("onSkid");
        }

        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground") && !onGroundState)
        {
            onGroundState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collided with goomba!");
            // play death animation
            marioAnimator.Play("mario-die");
            marioAudio.PlayOneShot(marioDeath);
            alive = false;

            // GameOver();


        }
    }

    void GameOver()
    {
        // Stop game
        Time.timeScale = 0.0f;

        // Show Game Over UI
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Update score text
        if (finalScoreText != null && jumpOverGoomba != null)
            finalScoreText.text = "Score: " + jumpOverGoomba.score.ToString();
    }

    // FixedUpdate may be called once per frame. See documentation for details.
    void FixedUpdate()
    {

        if (!alive)
            return;

        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(moveHorizontal) > 0)
        {
            Vector2 movement = new Vector2(moveHorizontal, 0);
            // check if it doesn't go beyond maxSpeed
            if (marioBody.linearVelocity.magnitude < maxSpeed)
                marioBody.AddForce(movement * speed);
        }

        // stop
        if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
        {
            // stop
            marioBody.linearVelocity = Vector2.zero;
        }

        // other instructions
        if (Input.GetKeyDown("space") && onGroundState)
        {
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;

            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }
    }

    public void RestartButtonCallback(int input)
    {
        Debug.Log("Restart!");
        // reset everything
        ResetGame();
        // resume time
        Time.timeScale = 1.0f;

        EventSystem.current.SetSelectedGameObject(null);

        // hide panel again
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public JumpOverGoomba jumpOverGoomba;

    void GameOverScene()
    {

        GameOver(); // replace this with whichever way you triggered the game over screen for Checkoff 1
    }

    private void ResetGame()
    {
        // reset position
        marioBody.transform.position = new Vector3(-15f, 4.48f, 0.0f);
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;
        // reset score
        scoreText.text = "Score: 0";
        // reset Goomba
        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.transform.localPosition = eachChild.GetComponent<EnemyMovement>().startPosition;
        }
        // reset score
        if (jumpOverGoomba != null)
        {
            jumpOverGoomba.score = 0;
        }

        // reset animation
        marioAnimator.Rebind();
        alive = true;
        onGroundState = true;
        marioAnimator.SetBool("onGround", onGroundState);

        // reset camera position
        gameCamera.position = new Vector3(-15.06f, 7.02f, -10f);
    }
}

