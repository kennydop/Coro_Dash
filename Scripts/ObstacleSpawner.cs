using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] PossibleSpawns; 
    public float timeToSpawn = 2f;
    public float actualWaitTime = 1f;
    public float sTime = 5;

    private void Update()
    {
        if (GameManager.Instance.GameOver && GameManager.Instance.continued > 2)
        {
            ResetDifficulty();
            return;
        }

        if (GameManager.Instance.GameOver || GameManager.Instance.playing == false)
            return;

        if (!GameManager.Instance.StartObstacles)
            return;

        sTime += 1.1f * Time.deltaTime;

        if (sTime >= timeToSpawn)
        {
            int s = Random.Range(0, PossibleSpawns.Length);
            Instantiate(PossibleSpawns[s], transform.position, Quaternion.identity, gameObject.transform);          
            timeToSpawn = sTime + actualWaitTime;
            actualWaitTime -= (0.03f * Time.deltaTime);
            actualWaitTime = Mathf.Clamp(actualWaitTime, 0.7f, 1f);
        }

    }

    void ResetDifficulty()
    {
        sTime = 6;
        actualWaitTime = 1f;
        timeToSpawn = 2f;
    }
}
