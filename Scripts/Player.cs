using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public float tapForce = 230;
    public int health = 3;
    public Health pHealth;
    public GameObject effect, mask;
    private Vector3 STARTPOS = new Vector2 (0, 7.5f);
    public bool onFloor, pb;
    AudioSource hitobs;

    private void Start()
    {
        rb.velocity = Vector2.zero;
        pHealth = GameManager.Instance.PlayerHealth;
        hitobs = GameManager.Instance.hit;
        mask = gameObject.transform.GetChild(0).gameObject;
        mask.SetActive(false);
    }
    void Update()
    {
        if (GameManager.Instance.playing == false)
        {
            isNotPlaying();
            return;
        }
        else
        {
            isPlaying();
        }

        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            if (onFloor || pb)
                return;

            rb.AddForce(Vector2.up * tapForce);
            rb.velocity = Vector2.zero;
            GameManager.Instance.jump.Play();
        }

        if (health == 0 && rb.simulated == true)
            GameManager.Instance.OnGameOver();

        
        transform.position = new Vector2 (Mathf.Clamp(transform.position.x, -0.5f, 0.5f), Mathf.Clamp(transform.position.y, -7.0f, 4.0f));
        
    }
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void isPlaying()
    {
        rb.simulated = true;
    }

    public void isNotPlaying()
    {
        rb.simulated = false;
    }

    void DeleteParticle()
    {
        Destroy(GameObject.FindGameObjectWithTag("Particle"));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Obstacle")
        {
            Destroy(other.gameObject);
            Instantiate(effect, other.gameObject.transform.position, Quaternion.identity);
            Camera.main.GetComponent<Animator>().Play("CameraShake");
            Invoke("DeleteParticle", 1);
            hitobs.Play();

            if (!mask.activeSelf)
            {
                health--;
                pHealth.RemoveHealth(health);
            }
        }

        if(other.tag == "LastKiller")
        {
            if (GameManager.Instance.firstTimer)
            {
                GameManager.Instance.TutorialContinue();
                GameManager.Instance.TutDeath = true;
                GameManager.Instance.StartObstacles = false;
            }
            else
            {
                GameManager.Instance.OnGameOver();
                pHealth.RemoveAll();
            }
        }

        if (other.tag == "Floor")
        { 
            rb.AddForce(Vector2.up * tapForce);
            rb.velocity = Vector2.zero;
            GameManager.Instance.jump.Play();
            onFloor = true;
            Invoke("NotOnFloor", 0.05f);
        }

    }
    public void PlayerReset()
    {
        transform.position = STARTPOS;
        health = 3;
        pHealth.HealthReset();
        rb.velocity =  Vector2.zero;
        mask.SetActive(false);
    }

    void NotOnFloor()
    {
        onFloor = false;
    }
}
