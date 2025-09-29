using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionBox : MonoBehaviour
{
    public Sprite disabledSprite;
    public GameObject coin;
    public AudioClip spawnSound;
    public float bounceHeight = 0.5f;
    public float bounceSpeed = 4f;

    private bool isDisabled = false;
    private Vector3 originalPosition;
    private Animator animator;
    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDisabled)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if player is below the box
            ContactPoint2D contact = collision.contacts[0];
            if (contact.normal.y > 0.5f)
            {
                // Hit from below
                Hit();
            }
        }
    }

    private void Hit()
    {
        // This is where the logic for bouncing, spawning coin, and disabling the box will go.
        // For now, let's just log a message.
        Debug.Log("Question Box was hit from below!");
        isDisabled = true;

        // spawn the coin
        coin.SetActive(true);

        // animate the coin
        // coin.GetComponent<Animator>().SetTrigger("spawn");

        // set the animator to the disabled state
        animator.SetTrigger("isDisabled");

        // set the rigidbody to static to disable joint
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        // // set the sprite to the disabled sprite
        // GetComponent<SpriteRenderer>().sprite = disabledSprite;

        // play the spawn sound
        audioSource.PlayOneShot(spawnSound);

    }

}
