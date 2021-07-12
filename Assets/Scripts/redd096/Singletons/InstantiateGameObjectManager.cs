namespace redd096
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("redd096/Singletons/Instantiate GameObject Manager")]
    public class InstantiateGameObjectManager : Singleton<InstantiateGameObjectManager>
    {
        [Header("Time Autodestruction")]
        [SerializeField] float timeAutodestruction = 5;

        private Transform parent;
        Transform Parent
        {
            get
            {
                if (parent == null)
                    parent = new GameObject("Instantiated Game Objects Parent").transform;

                return parent;
            }
        }
        Dictionary<GameObject, Pooling<GameObject>> pooling = new Dictionary<GameObject, Pooling<GameObject>>();

        /// <summary>
        /// Spawn at point and rotation. Use specific pooling
        /// </summary>
        public void Play(Pooling<GameObject> pool, GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
                return;

            //instantiate (if didn't find deactivated, take first one in the pool)
            GameObject element = pool.Instantiate(prefab);
            if (element == null && pool.PooledObjects.Count > 0)
                element = pool.PooledObjects[0];

            //if still null, return
            if (element == null)
                return;

            //set position, rotation and parent
            element.transform.position = position;
            element.transform.rotation = rotation;
            element.transform.SetParent(Parent);

            //start coroutine to deactivate
            StartCoroutine(DeactiveAfterSeconds(element));
        }

        IEnumerator DeactiveAfterSeconds(GameObject gameObjectToDeactivate)
        {
            //wait
            yield return new WaitForSeconds(timeAutodestruction);

            //and deactive
            if (gameObjectToDeactivate)
                gameObjectToDeactivate.gameObject.SetActive(false);
        }

        /// <summary>
        /// Spawn at point and rotation
        /// </summary>
        public void Play(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
                return;

            //if this pooling is not in the dictionary, add it
            if (pooling.ContainsKey(prefab) == false)
                pooling.Add(prefab, new Pooling<GameObject>());

            //use this manager's pooling, instead of a specific one
            Play(pooling[prefab], prefab, position, rotation);
        }

        /// <summary>
        /// Spawn at point and rotation. Get one random from the array
        /// </summary>
        public void Play(GameObject[] prefabs, Vector3 position, Quaternion rotation)
        {
            //do only if there are elements in the array
            if (prefabs.Length > 0)
            {
                Play(prefabs[Random.Range(0, prefabs.Length)], position, rotation);
            }
        }
    }
}