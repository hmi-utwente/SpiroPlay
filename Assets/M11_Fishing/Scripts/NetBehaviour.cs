using System;
using System.Collections;
using System.Collections.Generic;
using Spirometry.SpiroController;
using UnityEngine;
using Random = UnityEngine.Random;

namespace M11_Vissen
{


    public class NetBehaviour : MonoBehaviour
    {
        #region variables
        #pragma warning disable 649

        [SerializeField] private M11_Manager manager = null;
        [SerializeField] private GameObject[] regularFish;
        [SerializeField] private GameObject tropicalFish = null;

        [SerializeField] private Transform startRange = null;
        [SerializeField] private Transform endRange = null;
        
        [SerializeField] private string fishTag = "";
        [SerializeField] private int maxFishes = 20;
        [SerializeField] private float fishSpawnFrequency = 1.0f;

        [SerializeField] private Transform netParent = null;
        [SerializeField] private float squareSize = 3.0f;
        [SerializeField] private Vector2 netY;
        [SerializeField] private Vector2 netX;

        [SerializeField] private float horizontalSpeed;
        [SerializeField] private float verticalSpeed;
        [SerializeField] private float targetX;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float rotationSpeedUp;
        [SerializeField] private float stopPointXOffset;

        [SerializeField] private Sprite[] nets;

        private SpriteRenderer render;
        private Animator animator;

        private int fishesCaught = 0;
        private float fishSpawnTime = 0.0f;
        private bool transition = true;
        private float progressOld = 0f;
        private int fishSpawnedAmount;
        private int maxSpawnAmount;
        private bool stopCalculated;
        private Vector3 stopPoint;

#pragma warning restore 649
        #endregion
        
        private void Start()
        {
            render = GetComponent<SpriteRenderer>();
            render.sprite = nets[0];

            animator = GetComponent<Animator>();
        }

        public void Inhaling(float progress)
        {
            float progressNormalized = progress / 100f;

            float downProgress = Mathf.Lerp(progressOld, progressNormalized, Time.deltaTime * 2f);
            animator.SetFloat("DownProgress", downProgress);
            
            progressOld = downProgress;
        }

        public void Exhaling()
        {
            if (transition)
            {
                animator.enabled = false;
                Vector3 newpos = transform.position;
                newpos.x += horizontalSpeed;
                transform.position = newpos;
                transform.Rotate(0, 0, -rotationSpeed);

                if (newpos.x > targetX)
                {
                    transition = false;
                    render.sprite = nets[1];
                }
            }
        }

        public void Finished()
        {
            if (!stopCalculated)
            {
                stopPoint = transform.position + new Vector3(stopPointXOffset, 0, 0);
                stopCalculated = true;
            }
            
            if (transform.rotation.z < 0f)
            {
                transform.Rotate(0, 0, rotationSpeedUp);
                transform.position = Vector3.Lerp(transform.position, stopPoint, 0.01f);
            }

            if (transform.position.y < 45f)
            {
                Vector3 newpos = transform.position;
                newpos.y += verticalSpeed;
                transform.position = newpos;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(fishTag))
            {
                if (fishesCaught < manager.expirationProgressLong / 100.0f * maxFishes)
                {
                    fishesCaught++;
                    collision.gameObject.GetComponent<FishBehaviour>().enabled = false;
                    collision.gameObject.transform.parent = netParent;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(netParent.position, new Vector3(squareSize, squareSize, squareSize));
        }

        public void SpawnFish(float progress)
        {
            if (!transition)
            {
                if (fishSpawnedAmount < maxSpawnAmount)
                {
                    Vector3 position = new Vector3(Random.Range(netX.x, netX.y), Random.Range(netY.x, netY.y), 0);
                    Vector3 offset = new Vector3(50,Random.Range(10, -10),0);
                    position = transform.localPosition + position;
                    int randomFish = Random.Range(0, regularFish.Length);
                    GameObject spawnedObject = null;
                    spawnedObject = Instantiate(regularFish[randomFish], position + offset, regularFish[randomFish].transform.rotation);

                    spawnedObject.GetComponent<FishBehaviour>().SetManager(manager);
                    spawnedObject.GetComponent<FishBehaviour>().SetNet(transform);
                    spawnedObject.GetComponent<FishBehaviour>().Move(position);
                    fishSpawnedAmount += 1;
                }
            }

            maxSpawnAmount = (int)(progress / 10f);
        }

        public void SpawnTropicalFish()
        {
            Vector3 position = new Vector3(Random.Range(netX.x, netX.y),Random.Range(netY.x, netY.y),0);
            position = transform.localPosition + position;
            Vector3 offset = new Vector3(50,Random.Range(10, -10),0);
            var fish = Instantiate(tropicalFish, position + offset, tropicalFish.transform.rotation);
            fish.GetComponent<FishBehaviour>().SetManager(manager);
            fish.GetComponent<FishBehaviour>().SetNet(transform);
            fish.GetComponent<FishBehaviour>().Move(position);
        }
    }
}
