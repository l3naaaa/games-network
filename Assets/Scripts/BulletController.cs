using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviourPunCallbacks
{
Rigidbody rigidbody;
public float bulletSpeed = 15f;
public AudioClip bulletHitAud;
public int damage = 10;

   void Start()
   {
        if(!photonView.IsMine) { Destroy(this); return; }

     rigidbody= GetComponent<Rigidbody>();
   }
    int id;
    Vector3 m_bulletDir;
    public void StartBullet(Vector3 bulletDir, int _id)
    {
        id = _id;
      m_bulletDir = bulletDir;
    }

    void FixedUpdate(){
transform.forward = m_bulletDir;
     rigidbody.velocity = transform.forward * bulletSpeed;
    }
   private void OnCollissionEnter(Collision collision)
   {
       
     Debug.Log("bullet GOt Hit");
  

     Destroy(gameObject);
   }

   void OnTriggerEnter(Collider other){
        //if(other.tag == "Player"){return;}
        Debug.Log($"I hit {other.name}");
        if (other.transform.parent == null && other.transform == playerController.localPlayer.transform) {
            return;
        }

        if(other.transform.parent ==  playerController.localPlayer.transform)
        {
            return;
        }
        AudioManager.Instance.Play3D(bulletHitAud , transform.position);
        // VfxManager.vfxManager.PlayerVfx(bulletImpactvfx , transform.position);
        playerController.localPlayer.network.ShotFX(transform.position);
        if(other.tag == "Player")
        {

            playerNetwork controller = (other.transform.parent == null) ? other.GetComponent<playerNetwork>() : other.GetComponentInParent<playerNetwork>();
            if(controller.photonView.IsMine) { Debug.Log("Shot by myself"); return; }
            controller.TakeDamage(damage, id);
        }

        PhotonNetwork.Destroy(gameObject);
   }

   
}
