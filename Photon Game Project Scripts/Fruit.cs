using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Fruit : MonoBehaviourPun
{
    /// <summary>
    /// The number of points a player receives when the fruit is picked up.
    /// </summary>
    [SerializeField] private int _pointsOnPickup;

    public int PointsOnPickup
    {
        get => _pointsOnPickup;
    }

    [SerializeField] private GameObject _animationPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            photonView.RPC("CheckDestroy", RpcTarget.All, photonView.ViewID);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    public void CheckDestroy(int viewID)
    {
        if (photonView.ViewID == viewID)
        {
            PhotonNetwork.InstantiateSceneObject(_animationPrefab.name, transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(photonView);
        }
        else
        {
            Debug.Log("Not mine, not destroying it! My ID: " + photonView.ViewID);
        }
    }

}
