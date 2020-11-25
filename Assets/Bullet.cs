using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float m_Damage = 10.0f;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            collision.transform.GetComponent<Enemy>().Damage(m_Damage);
            //collision.transform.gameObject.SendMessage("Damage", m_Damage); //Alternative
        }
        gameObject.SetActive(false);
    }
}
