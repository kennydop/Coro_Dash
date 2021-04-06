using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public GameObject Cross1, Cross2, Cross3;

    public void RemoveHealth(int health)
    {
        if (health == 3)
            return;

        if (health == 2)
            Cross3.SetActive(false);
        if (health == 1)
            Cross2.SetActive(false);
        if (health == 0)
            Cross1.SetActive(false);
    }

    public void RemoveAll()
    {
        Cross1.SetActive(false); 
        Cross2.SetActive(false);
        Cross3.SetActive(false);
    }

    public void HealthReset()
    {
        Cross1.SetActive(true);
        Cross2.SetActive(true);
        Cross3.SetActive(true);
    }
}
