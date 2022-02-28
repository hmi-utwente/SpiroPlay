using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M11_Vissen;
using Spirometry.SpiroController;
public class FishBehaviour : MonoBehaviour
{
    [SerializeField] private float delay = 3.0f;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float fishSpeed = 0.5f;

    [SerializeField] private M11_Manager manager = null;

    private AudioSource sound;
    private bool hasSound = false;

    private Transform net;
    private bool shouldMove = true;
    private Vector3 targetPosition;

    private void Awake()
    {
        if (GetComponent<AudioSource>())
        {
            sound = GetComponent<AudioSource>();
            hasSound = true;
        }
    }

    private IEnumerator StartMoving()
    {
        yield return new WaitForSeconds(delay);

        if (manager.gameState != SpiroManager.State.Done && transform.parent == null)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (targetPosition != Vector3.zero)
        {
            if (shouldMove)
            {
                if (manager.gameState == SpiroManager.State.Exhaling)
                {
                    transform.position = Vector3.Lerp(transform.position, targetPosition, fishSpeed);
                }
                else if (manager.gameState == SpiroManager.State.Done)
                {
                    Vector3 netPosition = net.position + new Vector3(0, -8, 0);
                    transform.position = Vector3.Lerp(transform.position, netPosition, 0.1f);
                    if (Math.Abs(transform.position.x - netPosition.x) < 3f && Math.Abs(transform.position.y - netPosition.y) < 3f)
                    {
                        transform.SetParent(net);
                        shouldMove = false;
                    }
                }

                if (Math.Abs(transform.position.x - targetPosition.x) < 4f)
                {
                    transform.SetParent(net);
                    shouldMove = false;
                }
            }
        }
        else if(manager.gameState == SpiroManager.State.Exhaling)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        
        if (hasSound)
        {
            sound.Play();
            hasSound = false;
        }
    }

    public void Move(Vector3 target)
    {
        targetPosition = target;
        StartCoroutine(StartMoving());
    }

    public void SetManager(M11_Manager newManager)
    {
        manager = newManager;
    }
    
    public void SetNet(Transform parent)
    {
        net = parent;
    }
}
