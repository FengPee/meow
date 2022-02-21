using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handgun : MonoBehaviour
{

    public float distance = 15f; // range of our gun

    Camera camera;
    bool isFiring;

    float shotCounter;
    public float rateOfFire = 0.7f; // how fast the gun shoots

    public GameObject muzzleEffect; // holds the effect of the muzzle shot
    public Transform muzzleSpawn; // spawns the effect

    Animator anim;

    // take main camera in
    private void Start()
    {
        camera = Camera.main;
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // getting mouse input
        if (Input.GetButtonDown("Fire1"))
            isFiring = true;
        else if (Input.GetButtonUp("Fire1"))
        {
            isFiring = false;
            StopAnim();
        }

        // setting rate of fire
        if (isFiring)
        {
            shotCounter -= Time.deltaTime;

            if (shotCounter <= 0)
            {
                shotCounter = rateOfFire;
                Shoot();
            }
        }
        else
            shotCounter -= Time.deltaTime;
    }

    // method to shoot with raycast
    private void Shoot()
    {
        RaycastHit hit;
        EnemyScript enemyScript;

        GameObject muzzle = Instantiate(muzzleEffect, muzzleSpawn.position, Quaternion.Euler(Vector3.zero));
        muzzle.transform.parent = muzzleSpawn;

        //testing to detect shot
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, distance))
        {
            if (hit.transform.tag == "Enemy")
            {
                enemyScript = hit.transform.GetComponent<EnemyScript>();
                enemyScript.health--;
                Debug.Log(enemyScript.health);
            }
                Debug.Log("Hit");
        }
        else
            Debug.Log("Not Hit");

        anim.SetBool("isFiring", true);
    }

    private void StopAnim()
    {
        anim.SetBool("isFiring", false);
    }

}
