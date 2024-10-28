using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviourPunCallbacks
{
    public static playerController localPlayer;
    public playerNetwork network;
    public float pMovementSpeed = 10f;
    new Rigidbody rigidbody;
    public float fireRate = 0.75f;
    public GameObject bulletPrefab;
    public Transform bulletPos;
    float nextfire; 

    public AudioClip playerShootAud; 
    public GameObject bulletFirevfx;

    //[HideInInspector] 
    // public int Health = 100;

    private void Awake()
    {
        network = GetComponent<playerNetwork>();
    }

    
    void Start()
    {
        if(!photonView.IsMine){Destroy(this); return;}

        localPlayer = this;
        CameraFollowv2.instance.target = transform;
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
       Move();
       if(Input.GetKey(KeyCode.Space))
       {
            Fire();
       }
    }


    void PlayerDead()
    {
        gameObject.SetActive(false);
    }
    Vector3 lastPos;
    public float turningSmoothness = 0.1f;
    float turnSmoothVelocity;
    float horizontalInput => Input.GetAxisRaw("Horizontal");
    float verticalInput => Input.GetAxisRaw("Vertical") ;
    void Move()
    {
        if(horizontalInput== 0 && verticalInput== 0)
        return;
        
       
        
        
        Vector3 posDelta = transform.position - lastPos;
        Vector3 direction = posDelta.normalized;

        
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), turningSmoothness);
        lastPos = transform.position;

        Vector3 forwardMove = Camera.main.transform.forward * verticalInput;
        Vector3 verticalMove = Camera.main.transform.right * horizontalInput;
        forwardMove.y = 0; verticalMove.y = 0;

        rigidbody.MovePosition(rigidbody.position + ((forwardMove + verticalMove) * Time.deltaTime * pMovementSpeed));
       
    }

    void Fire()
    {
        if(Time.time > nextfire)
        {
            nextfire = Time.time + fireRate;
            GameObject bullet = PhotonNetwork.Instantiate("Bullet", bulletPos.position, Quaternion.identity);
            bullet.GetComponent<BulletController>()?.StartBullet(transform.rotation * Vector3.forward, photonView.ViewID);
            Debug.Log("fireed");
            AudioManager.Instance.Play3D(playerShootAud, transform.position);

          
            
        }
    }
}
