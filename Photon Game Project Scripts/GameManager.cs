using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPun
{
    void Start()
    {
        GameObject player = PhotonNetwork.Instantiate("MaskGuy", new Vector3(-1.6f, -0.6f, 0f), Quaternion.identity);
        GameObject camera = Instantiate((GameObject)Resources.Load("Camera"), new Vector3(-1.6f, -0.6f, 0f), Quaternion.identity);

        camera.GetComponent<CameraFollow>().player = player.transform;

        InstantiateApples();
        InstantiatePineapples();
        InstantiateCherries();
    }
    private void Update()
    {
        LeaveRoom();
    }

    #region Instantiation on Fruits
    void InstantiateApples()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject[] fruitsToRespawn = GameObject.FindGameObjectsWithTag("Apples");

            foreach (GameObject g in fruitsToRespawn)
            {
                PhotonNetwork.InstantiateSceneObject("Apple", g.transform.position, g.transform.rotation);
                Destroy(g);
            }
        }
        else
        {
            GameObject[] fruitsToRespawn = GameObject.FindGameObjectsWithTag("Apples");

            foreach (GameObject g in fruitsToRespawn)
            {
                Destroy(g);
            }
        }
    }
    void InstantiatePineapples()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject[] fruitsToRespawn = GameObject.FindGameObjectsWithTag("Pineapples");

            foreach (GameObject g in fruitsToRespawn)
            {
                PhotonNetwork.InstantiateSceneObject("Pineapple", g.transform.position, g.transform.rotation);
                Destroy(g);
            }
        }
        else
        {
            GameObject[] fruitsToRespawn = GameObject.FindGameObjectsWithTag("Pineapples");

            foreach (GameObject g in fruitsToRespawn)
            {
                Destroy(g);
            }
        }
    }
    void InstantiateCherries()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject[] fruitsToRespawn = GameObject.FindGameObjectsWithTag("Cherries");

            foreach (GameObject g in fruitsToRespawn)
            {
                PhotonNetwork.InstantiateSceneObject("Cherries", g.transform.position, g.transform.rotation);
                Destroy(g);
            }
        }
        else
        {
            GameObject[] fruitsToRespawn = GameObject.FindGameObjectsWithTag("Cherries");

            foreach (GameObject g in fruitsToRespawn)
            {
                Destroy(g);
            }
        }
    }
    #endregion

    [PunRPC]
    public void LeaveRoom()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Time.timeScale = 1f;
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
            PhotonNetwork.LoadLevel("LobbyScene");
        }
    }
}
