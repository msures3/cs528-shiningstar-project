// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class StarPool : MonoBehaviour
// {
//     public GameObject starPrefab;
//     public int poolSize = 100;
//     private Queue<GameObject> starPool = new Queue<GameObject>();

//     private void Start()
//     {
//         // Populate the object pool with star objects
//         for (int i = 0; i < poolSize; i++)
//         {
//             GameObject star = Instantiate(starPrefab, Vector3.zero, Quaternion.identity);
//             star.SetActive(false);
//             starPool.Enqueue(star);
//         }
//     }

//     public GameObject GetStarFromPool()
//     {
//         // Retrieve a star object from the pool
//         if (starPool.Count > 0)
//         {
//             GameObject star = starPool.Dequeue();
//             star.SetActive(true);
//             return star;
//         }
//         else
//         {
//             // If the pool is empty, create a new star object
//             return Instantiate(starPrefab, Vector3.zero, Quaternion.identity);
//         }
//     }

//     public void ReturnStarToPool(GameObject star)
//     {
//         // Return a star object to the pool
//         star.SetActive(false);
//         starPool.Enqueue(star);
//     }
// }
