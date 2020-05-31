using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Wormy : NetworkBehaviour
{
    public Rigidbody2D bulletPrefab;
    public Transform currentGun;

    public float wormySpeed = 1;
    public float maxRelativeVelocity;
    public float misileForce = 5;

    public bool IsTurn { get { return WormyManager.singleton.IsMyTurn(wormId); } }

    public int wormId;
    WormyHealth wormyHealth;
    SpriteRenderer ren;

    private void Start()
    {
        wormyHealth = GetComponent<WormyHealth>();
        ren = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;
       
        if (!IsTurn)
            return;

        RotateGun();

        var hor = Input.GetAxis("Horizontal");
        if (hor == 0)
        {
            currentGun.gameObject.SetActive(true);

            ren.flipX = currentGun.eulerAngles.z < 180;

            if (Input.GetKeyDown(KeyCode.Q))
            {
                var tempPos = currentGun.position - currentGun.right;
                CmdShoot(tempPos, currentGun.rotation, -currentGun.right);                
            }
        }
        else
        {
            currentGun.gameObject.SetActive(false);
            transform.position += Vector3.right *
                                hor *
                                Time.deltaTime *
                                wormySpeed;            
             ren.flipX = Input.GetAxis("Horizontal") > 0;
        }
    }

    [Command]
    void CmdShoot(Vector3 pos, Quaternion rot, Vector3 left)
    {
        var p = Instantiate(bulletPrefab,
                            pos,
                            rot);

        NetworkServer.Spawn(p.gameObject);
        p.AddForce(left * misileForce, ForceMode2D.Impulse);

        if (IsTurn)
            WormyManager.singleton.NextWorm();
    }


    void RotateGun()
    {
        if (!isLocalPlayer)
            return;
        var diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        currentGun.rotation = Quaternion.Euler(0f, 0f, rot_z + 180);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isServer)
            return;

        if (collision.relativeVelocity.magnitude > maxRelativeVelocity)
        {
            wormyHealth.ChangeHealth(-3);
            if (IsTurn)
                WormyManager.singleton.NextWorm();
        }  
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer)
            return;

        if (collision.CompareTag("Explosion"))
        {
            wormyHealth.ChangeHealth(-10);
            if (IsTurn)
                WormyManager.singleton.NextWorm();
        }
            
    }
}
