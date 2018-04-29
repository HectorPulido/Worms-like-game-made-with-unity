using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wormy : MonoBehaviour {
    public Rigidbody2D bulletPrefab;
    public Transform currentGun;
    public float wormySpeed = 1;
    public int health;
    public bool gun;

    SpriteRenderer ren;

    private void Start()
    {
        ren = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        RotateGun();

        if (Input.GetKeyDown(KeyCode.W))
        {
            gun = !gun;
            currentGun.gameObject.SetActive(gun);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (gun)
            {
                var p = Instantiate(bulletPrefab,
                    currentGun.position - currentGun.right,
                    currentGun.rotation);
                
                p.AddForce(-currentGun.right * 3, ForceMode2D.Impulse);

            }
        }
        if (!gun)
        {
            transform.position += Vector3.right *
                                Input.GetAxis("Horizontal") *
                                Time.deltaTime *
                                wormySpeed;
            if (Input.GetAxis("Horizontal") != 0)
                ren.flipX = Input.GetAxis("Horizontal") > 0;
        }
        else
        {
            if (currentGun.eulerAngles.z > 180)
                ren.flipX = Input.GetAxis("Horizontal") > 0;
        }
    }


    void RotateGun()
    {
        Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        currentGun.rotation = Quaternion.Euler(0f, 0f, rot_z + 180
            );
    }
}
