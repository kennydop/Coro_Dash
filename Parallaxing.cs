using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type { None, Obstacle, FrontCloud, BackCloud, LowerCloud, Floor }
public class Parallaxing : MonoBehaviour
{
    public Type type;
    public float moveSpeed;
    float minClampX;
    float maxClampX;
    Vector2 StartPos;

    private void Start()
    {
        StartPos = gameObject.transform.position;
        GetType(type);
    }
    private void OnEnable()
    {
        GetType(type);
    }
    void Update()
    {

        if (GameManager.Instance.playing == false)
            return;

        if (GameManager.Instance.GameOver)
            return;

        transform.Translate(((Vector3.left) * moveSpeed * Time.deltaTime));
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, minClampX, maxClampX), transform.position.y);
        switch (type)
        {
            case Type.FrontCloud:
                if (transform.position.x <= minClampX)
                {
                    transform.position = new Vector2(maxClampX, 3.6f);
                }
                break;

            case Type.BackCloud:
                if (transform.position.x <= minClampX)
                {
                    transform.position = new Vector2(maxClampX, 2.7f);
                }
                break;

            case Type.LowerCloud:
                if (transform.position.x <= minClampX)
                {
                    transform.position = new Vector2(maxClampX, -1.05f);
                }
                break;

            case Type.Floor:
                if (transform.position.x <= minClampX)
                {
                    transform.position = new Vector2(maxClampX, -5.25f);
                }
                break;

            case Type.Obstacle:
                if (transform.position.x <= minClampX)
                {
                    // gameObject.SetActive(false);
                    // gameObject.transform.position = StartPos;
                    // GameManager.Instance.prevs.Add(gameObject);
                    Destroy(gameObject);
                }
                break;
        }

        moveSpeed += 0.001f * Time.deltaTime;
    }

    void GetType(Type t)
    {
        switch (t)
        {

            case Type.FrontCloud:
                moveSpeed = 0.17f;
                minClampX = -22f;
                maxClampX = 22f;
                break;

            case Type.BackCloud:
                moveSpeed = 0.05f;
                minClampX = -22f;
                maxClampX = 22f;
                break;

            case Type.LowerCloud:
                moveSpeed = 0.01f;
                minClampX = -22f;
                maxClampX = 22f;
                break;

            case Type.Obstacle:
                moveSpeed = 3.0f;
                minClampX = -15.0f;
                maxClampX = 15.0f;
                break;

            case Type.Floor:
                moveSpeed = 2f;
                minClampX = -30f;
                maxClampX = 43f;
                break;

        }
    }

    public void ParallaxReset()
    {
        gameObject.transform.position = StartPos;
    }
}
