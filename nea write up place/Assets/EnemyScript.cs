using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    public int health;

    void Update()
    {
        if (health <= 0)
            Death();
    }

    private void Death()
    {
        Destroy(gameObject);
    }
}

