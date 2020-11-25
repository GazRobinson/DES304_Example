using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A simple AI class that follows the player at a variable speed
public class Enemy : MonoBehaviour
{
    [SerializeField] Stats m_Stats;

    Vector3 lastKnownPostion;
    private bool m_IsFollowing = false;
    //What body should we follow
    [SerializeField] Transform followTarget;
    //How fast should we go in Units per Second

    //This enemy has a gun!
    private Gun myGun;
    //At that Time.time should we next shoot the gun
    float nextShootTime = 0.0f;
    float m_Health;
    void Start()
    {
        //Find the reference for our gun
        myGun = GetComponentInChildren<Gun>();
        lastKnownPostion = transform.position;
        m_Health = m_Stats.MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsFollowing)
        {
            //Work out which way we're going by subtracting our position from the position
            //of the thing we're following
            //We normalise it to ensure the values are correct later on when we multiply it by our speed
            Vector3 direction = followTarget.position - transform.position;
            direction.Normalize();

            //Move the enemy in the direction of the target at <movementSpeed> units per second.
            //It's the "* Time.deltaTime" that makes it happen per second, and not per frame!
            transform.Translate(direction * Time.deltaTime * m_Stats.MoveSpeed, Space.World);

            //Make the enemy look at the target
            transform.LookAt(followTarget, Vector3.up);
            lastKnownPostion = followTarget.position;
            //Compare the current time (in seconds since the game started) to the assigned "next shoot time"
            //If it's past that time, we shoot! And update the next shoot time.
            if (Time.time > nextShootTime)
            {
                myGun.Shoot();
                nextShootTime = Time.time + 2.0f;
            }
        }
        else
        {
            //Work out which way we're going by subtracting our position from the position
            //of the thing we're following
            //We normalise it to ensure the values are correct later on when we multiply it by our speed
            Vector3 direction = lastKnownPostion - transform.position;
            direction.Normalize();

            //Move the enemy in the direction of the target at <movementSpeed> units per second.
            //It's the "* Time.deltaTime" that makes it happen per second, and not per frame!
            transform.Translate(direction * Time.deltaTime * m_Stats.MoveSpeed, Space.World);

            //Make the enemy look at the target
            transform.LookAt(lastKnownPostion, Vector3.up);            
        }
        RaycastHit hitInfo;
        if(Physics.Raycast(transform.position, transform.forward,out hitInfo))
        {
            if(hitInfo.transform == followTarget)
            {
                m_IsFollowing = true;
            }
            else
            {
                m_IsFollowing = false;
            }
        }
        else
        {
            m_IsFollowing = false;
        }
    }

    public void Damage(float damageDealt)
    {
        Debug.Log("Enemy damaged");
        m_Health = m_Health - damageDealt; //OR m_Health -= damageDealt;

        if(m_Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
