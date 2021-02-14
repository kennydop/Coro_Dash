using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDown : MonoBehaviour
{
    public TMPro.TextMeshProUGUI countDownText;
    private void OnEnable()
    {
        StartCoroutine(countDown());
    }
    IEnumerator countDown()
    {
        for (int i = 3; i >= 1; i--)
        {
            countDownText.text = i.ToString();
            if (i == 1)
            {
                yield return new WaitForSeconds(1);
                countDownText.text = "GO!";
                yield return new WaitForSeconds(1);
                yield break;
            }
            else
                yield return new WaitForSeconds(1);
        }
    }
}
