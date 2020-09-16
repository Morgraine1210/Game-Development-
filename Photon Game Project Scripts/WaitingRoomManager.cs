using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        GameObject player = PhotonNetwork.Instantiate("MaskGuy", new Vector3(-1.6f, -0.6f, 0f), Quaternion.identity);
        GameObject camera = Instantiate((GameObject)Resources.Load("Camera"), new Vector3(-1.6f, -0.6f, 0f), Quaternion.identity);
        camera.GetComponent<CameraFollow>().player = player.transform;
    }
    private void Update()
    {
        LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        CheckRoomFull();
    }

    public void CheckRoomFull()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            StartCoroutine(PrepareGameScene());
        }
    }

    public void LeaveRoom()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
            PhotonNetwork.LoadLevel("LobbyScene");
        }
    }

    IEnumerator PrepareGameScene()
    {
        yield return new WaitForSeconds(5.0f);
        PhotonNetwork.LoadLevel("GameScene");
    }
}
