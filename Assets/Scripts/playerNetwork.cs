using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class playerNetwork : MonoBehaviourPunCallbacks, IPunObservable
{
    public List<Material> materials;
    public MeshRenderer[] renderers;

    [Range(0,100f)]
    public float health = 100;

    public int score = 0;
    public Slider healthBar;
    public int deathCount;
    public string username;

    public GameObject explosionVFX;
    void Start()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RpcSyncInit", RpcTarget.AllBuffered, Random.Range(0, materials.Count), PhotonNetwork.LocalPlayer.NickName);
        }
    }
    public void Reset()
    {
        deathCount = score = 0;
        health = 100;
    }


    [PunRPC]
    void RpcSyncInit(int id, string _username)
    {
        Material selectedMaterial = materials[id];

        foreach (MeshRenderer meshRenderer in renderers)
        {
            meshRenderer.material = materials[id];
        }
        materials.Remove(materials[id]);

        username = _username;
    }

    // Update is called once per frame
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
           stream.SendNext(health);
            stream.SendNext(score);
            stream.SendNext(deathCount);

            SceneDataHolder.instance.txtScore.text = score.ToString();
            SceneDataHolder.instance.txtDeathCount.text = deathCount.ToString();
        }
        else
        {
           health = (float)stream.ReceiveNext();
            score = (int)stream.ReceiveNext();
            deathCount= (int)stream.ReceiveNext();
        }

        healthBar.value = health;
    }


    public void TakeDamage(float damage, int id)
    {
        photonView.RPC("RpcTakeDamage", RpcTarget.All, damage,id);
    }


    [PunRPC]
    void RpcTakeDamage(float damage,int id)
    {
        health-= damage;

        if(photonView.IsMine && health <= 0)
        {
            PhotonView.Find(id).gameObject.GetComponent<playerNetwork>().Score();
            GameManager.Respawn();

            deathCount++;
        }
    }


    public void Score()
    {
        photonView.RPC("RpcScore", RpcTarget.All);
    }

    [PunRPC]
    void RpcScore()
    {
        if (photonView.IsMine)
        {
            score+= 10;
        }
    }


    public void ShotFX(Vector3 position)
    {
        if(photonView.IsMine)
        {
            photonView.RPC("RpcFX", RpcTarget.All, position);
        }
    }

    [PunRPC]
    void RpcFX(Vector3 position)
    {
        StartCoroutine(AutoDisableAfterSpawn(explosionVFX,position, 3));
    }

    IEnumerator AutoDisableAfterSpawn(GameObject bulletObj,Vector3 position, float timer)
    {
        GameObject go = Instantiate(bulletObj, position, Quaternion.identity);
        yield return new WaitForSeconds(timer);

        Destroy(go);
    }
}
