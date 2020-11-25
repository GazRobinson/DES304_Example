using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A very simple Prefab spawner that I've called GUN because those are popular things in Video Games
public class Gun : MonoBehaviour
{
    //Bullet prefab is the 'base' object that all of the bullets we shoot will be copied from
    [SerializeField] GameObject bulletPrefab;
    AudioSource m_Audio;
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip reloadSound;

    [SerializeField] AudioClip[] soundEffects;
              
    AudioClip[] soundFX = new AudioClip[5];
    [SerializeField] bool isProjectile = false;
    [SerializeField] LayerMask layerMask;
    Ray ray = new Ray();

    [SerializeField] int Max_Bullets = 10;

    [SerializeField] List<Bullet> m_BulletList = new List<Bullet>();

    private void Start()
    {
        m_Audio = GetComponent<AudioSource>();
    }
    private void Update()
    {
        ray.origin = transform.parent.position;
        ray.direction = transform.parent.forward;
    }
    //Shoot the gun
    public void Shoot()
    {
        if (isProjectile)
        {
            ShootProjectile();
        }
        else
        {
            ShootRay();
        }
    }

    void ShootRay()
    {
        ray.origin = transform.parent.position;
        ray.direction = transform.parent.forward;

        RaycastHit hitInfo;
        float splashDistance = 5.0f;

        if (Physics.Raycast(ray, out hitInfo, 2000.0f, layerMask))
        {
            Debug.Log("Ray hit! I hit " + hitInfo.transform.name);

            Collider[] colliders = Physics.OverlapSphere(hitInfo.point, splashDistance);
            if (colliders.Length > 0)
            {
                for(int i=0; i < colliders.Length; i++)
                {
                    if (colliders[i].GetComponent<Enemy>())
                    {
                        Destroy(colliders[i].transform.gameObject);
                        break;
                    }
                }
            }

            if (hitInfo.transform.GetComponent<Enemy>())
            {
                Destroy(hitInfo.transform.gameObject);
            }
        }
    }

    void ShootProjectile()
    {
        GameObject bullet = null;
        //make a bullet prefab
        if (m_BulletList.Count < Max_Bullets)
        {
            bullet = Instantiate<GameObject>(bulletPrefab, transform.position, Quaternion.identity);
            m_BulletList.Add(bullet.GetComponent<Bullet>());
        }
        else
        {
            /*for(int i = 0; i < Max_Bullets; i++)
            {

            }*/
            foreach(Bullet b in m_BulletList)
            {
                if (!b.gameObject.activeSelf)
                {
                    bullet = b.gameObject;
                    bullet.SetActive(true);
                    break;
                }
            }
        }
        if (bullet != null)
        {

            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;
            Vector3 forward = transform.forward;
            bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
            bullet.GetComponent<Rigidbody>().AddForce(forward * 10.0f, ForceMode.Impulse);
            float random = Random.value;
            bullet.transform.localScale = Vector3.one * random;


            if (m_Audio)
            {
                m_Audio.pitch = Mathf.Lerp(0.5f, 2.0f, random);  //Random.Range(0.5f, 2.0f);
                m_Audio.PlayOneShot(soundEffects[0]);
            }
        }
    }

    public void Reload()
    {
        if (m_Audio)
        {
            m_Audio.PlayOneShot(soundEffects[1]);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(ray.origin, ray.origin+ ray.direction * 5.0f);
    }
}
