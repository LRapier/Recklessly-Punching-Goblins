using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public string playerPrefabPath;
    public Transform[] spawnPoints;
    public float respawnTime;
    public PlayerController[] players;

    [Header("Stage")]
    public GameObject outer;
    public GameObject middle;
    public GameObject inner;
    public GameObject outerWater;
    public GameObject middleWater;
    public GameObject innerWater;
    public float timeForShrink;

    public int playersInGame;
    public int playersAlive;
    private float timeLeft;
    private bool shrunk = false;

    public static GameManager instance;

    void Awake()
    {
        instance = this;
        timeLeft = timeForShrink;
    }

    void Update()
    {
        GameUI.instance.UpdateTime((int)timeLeft, shrunk);
        timeLeft -= Time.deltaTime;
        CheckWinCondition();
    }

    void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
        StartCoroutine(Shrinkage());
        IEnumerator Shrinkage()
        {
            yield return new WaitForSeconds(timeForShrink);
            Shrink(0);
            timeLeft = timeForShrink;
            yield return new WaitForSeconds(timeForShrink);
            Shrink(1);
            timeLeft = timeForShrink;
            yield return new WaitForSeconds(timeForShrink);
            Shrink(2);
            shrunk = true;
        };
        playersAlive = players.Length;
        GameUI.instance.UpdateGoldText(playersAlive);
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        if (playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
    }

    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabPath, spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber].position, Quaternion.identity);
        playerObj.GetComponent<PhotonView>().RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    void Shrink(int level)
    {
        switch(level)
        {
            case 0:
                outer.SetActive(false);
                outerWater.SetActive(true);
                break;
            case 1:
                middle.SetActive(false);
                middleWater.SetActive(true);
                break;
            case 2:
                inner.SetActive(false);
                innerWater.SetActive(true);
                break;
        }
    }
    public PlayerController GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId);
    }
    public PlayerController GetPlayer(GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }

    [PunRPC]
    void KillPlayer(int playerId)
    {
        PlayerController player = GetPlayer(playerId);
        player.dead = true;
        player.player.SetActive(false);
        playersAlive--;
        if (playersAlive < 0)
            playersAlive = 0;
        GameUI.instance.UpdateGoldText(playersAlive);
    }

    [PunRPC]
    void WinGame(int playerId)
    {
        PlayerController player = GetPlayer(playerId);
        GameUI.instance.SetWinText(player.photonPlayer.NickName);

        Invoke("GoBackToMenu", 3.0f);
    }

    void GoBackToMenu()
    {
        NetworkManager.instance.ChangeScene("Menu");
    }

    public void CheckWinCondition()
    {
        if (playersAlive == 1)
            photonView.RPC("WinGame", RpcTarget.All, players.First(x => !x.dead).id);
        else if (playersAlive == 0)
            photonView.RPC("TieGame", RpcTarget.All);
    }

    [PunRPC]
    public void TieGame()
    {
        GameUI.instance.SetTieText();

        Invoke("GoBackToMenu", 3.0f);
    }
}
