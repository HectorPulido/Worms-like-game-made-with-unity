using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WormyManager : NetworkBehaviour
{
    public static WormyManager singleton;

    public Transform wormyCamera;

    [SyncVar(hook = "MoveCamera")]
    int currentWormy = -1;

    Wormy[] wormies;

    private void Awake()
    {
        if (singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;
        wormyCamera = Camera.main.transform;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        wormies = GameObject.FindObjectsOfType<Wormy>();
        if (isServer)
        {
            for (int i = 0; i < wormies.Length; i++)
            {
                wormies[i].wormId = i;
            }
            NextWorm();
        }
    }

    public void NextWorm()
    {
        if (!isServer)
            return;

        StartCoroutine(NextWormCoroutine());
    }

    public IEnumerator NextWormCoroutine()
    {
        var nextWorm = currentWormy + 1;
        currentWormy = -1;

        yield return new WaitForSeconds(2);

        currentWormy = nextWorm;
        if (currentWormy >= wormies.Length)
        {
            currentWormy = 0;
        }
    }

    void MoveCamera(int c)
    {
        currentWormy = c;

        if (c < 0 || c >= wormies.Length)
            return;

        wormyCamera.SetParent(wormies[currentWormy].transform);
        wormyCamera.localPosition = Vector3.zero + Vector3.back * 10;
    }

    public bool IsMyTurn(int i)
    {
        return i == currentWormy;
    }

}
