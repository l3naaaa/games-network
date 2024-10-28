using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    // 
    public float fireRate = 0.75f;
    public GameObject bulletPrefab;
    public Transform bulletPos;
    float nextfire; 
    public AudioClip playerShootAud; 
    public GameObject bulletFirevfx;
    
    [HideInInspector] 
    public int Health = 100;
    public Slider healthBar;
    
   
   private void OnTriggerStay(Collider othercollision)
   {
        if(othercollision.gameObject.tag.Equals("Player"))
        {
            transform.LookAt(othercollision.transform);
            Fire();
        }
   }

    private void OnCollissionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bullet"))
        {
            BulletController bullet = collision.gameObject.GetComponent<BulletController>();
            TakeDamage(bullet.damage);
        }
    }

    void TakeDamage(int damage)
    {
        Health -= damage;
        healthBar.value = Health;
        if(Health<=0)
        {
            EnemyDead();
        }
    }

    void EnemyDead()
    {
        gameObject.SetActive(false);
    }
   void Fire()
    {
        if(Time.time > nextfire)
        {
            nextfire = Time.time + fireRate;
            GameObject bullet = Instantiate( bulletPrefab , bulletPos.position , Quaternion.identity);
            bullet.GetComponent<BulletController>()?.StartBullet(transform.rotation * Vector3.forward,0);
            Debug.Log("fireed");
            AudioManager.Instance.Play3D(playerShootAud , transform.position);
            VfxManager.vfxManager.PlayerVfx(bulletFirevfx , bulletPos.position);
            
        }
    }
}
