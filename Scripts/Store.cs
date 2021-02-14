using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    public List<Player> players;
    public List<int> LevelAcquiredForEachPlayer;
    public Button ActivateButton;
    public TextMeshProUGUI ActivateButtonText;
    public TextMeshProUGUI AbilityText;
    public GameObject CurrentPlayer;
    public List<Sprite> playerImgs;
    public List<string> Abilities;
    public List<GameObject> Store_Ads;
    public string SelectedPlayer;
    private void OnEnable()
    {
        StartStore();
    }

    public void StartStore()
    {
        SelectedPlayer = ES3.Load<string>("SelectedPlayer", defaultValue: "1");

        for (int p = 0; p < playerImgs.Count; p++)
        {
            if (playerImgs[p].name == SelectedPlayer)
            {
                CurrentPlayer.GetComponent<Image>().sprite = playerImgs[p];
                AbilityText.text = Abilities[p];
                ActivateButtonText.text = "Activated";
                ActivateButton.interactable = false;
                break;
            }
        }
    }
    public void OnCharacterTap()
    {
        GameManager.Instance.tap.Play();
        int q = int.Parse(EventSystem.current.currentSelectedGameObject.name);

        CurrentPlayer.GetComponent<Image>().sprite = playerImgs[q - 1];
        AbilityText.text = Abilities[q - 1];
        if (GameManager.Instance.CurrentLevel >= LevelAcquiredForEachPlayer[q - 1])
        {
            ActivateButton.interactable = true;
            ActivateButtonText.text = "Activate";
        }

        else
        {
            ActivateButton.interactable = false;
            ActivateButtonText.text = "Level " + LevelAcquiredForEachPlayer[q - 1];
        }

        if (EventSystem.current.currentSelectedGameObject.name == SelectedPlayer)
        {
            ActivateButtonText.text = "Activated";
            ActivateButton.interactable = false;
        }

    }

    public void OnCharacterActivated()
    {
        GameManager.Instance.PlayerActivated.Play();
        string CurrentPlayerName = CurrentPlayer.GetComponent<Image>().sprite.name;
        ES3.Save<string>("SelectedPlayer", CurrentPlayerName);
        SelectedPlayer = CurrentPlayerName;
        ActivateButton.interactable = false;
        ActivateButtonText.text = "Activated";
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        for (int p = 0; p < players.Count; p++)
        {
            if (players[p].name == ES3.Load<string>("SelectedPlayer", defaultValue: "1"))
            {
                int PlayerToSpawnIndex = int.Parse(SelectedPlayer) - 1;
                Instantiate(players[PlayerToSpawnIndex], new Vector3(0, 7.5f), Quaternion.identity);
                break;
            }
        }

    }

    public void DetermineWhichAdToShow()
    {
        if (GameManager.Instance.removedAds)
            return;

        int num = Random.Range(1, 4);

        foreach (GameObject go in Store_Ads)
        {
            if(int.Parse(go.name) == num)
            {
                go.SetActive(true);
            }
            else
            {
                go.SetActive(false);
            }
        }
    }
}
