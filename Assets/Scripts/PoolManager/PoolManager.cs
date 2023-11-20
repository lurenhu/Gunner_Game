using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PoolManager : SingletonMonoBehaviours<PoolManager>
{
    [SerializeField] private Pool[] poolArray = null;
    private Transform objectPoolTransform;
    private Dictionary<int , Queue<Component>> poolDictionary = new Dictionary<int , Queue<Component>>();   

    [System.Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
        public string componentType;
    }

    private void Start()
    {
        objectPoolTransform = this.gameObject.transform;

        for (int i = 0; i < poolArray.Length; i++)
        {
            CreatPool(poolArray[i].prefab, poolArray[i].poolSize, poolArray[i].componentType);
        }
    }

    /// <summary>
    /// ����������еĶ��󣬲������ж���ʵ����
    /// </summary>
    private void CreatPool(GameObject prefab, int poolSize, string componentType)
    {
        int poolKey = prefab.GetInstanceID();

        string prefabName = prefab.name;

        GameObject parentGameObject = new GameObject(prefabName + "Anchor");

        parentGameObject.transform.SetParent(objectPoolTransform);

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<Component>());

            for (int i = 0; i < poolSize; i++)
            {
                GameObject newObject = Instantiate(prefab, parentGameObject.transform) as GameObject;

                newObject.SetActive(false);

                poolDictionary[poolKey].Enqueue(newObject.GetComponent(Type.GetType(componentType)));
            }
        }
    }

    /// <summary>
    /// ����Ԥ�裬��ȡԤ���Ӧ��ID�����Key���ڶ�����в������ã���ȡ��Ӧ��component�����
    /// ������λ������position,��ת�Ƕ�rotation,��СlocalScale���и�ֵ
    /// </summary>
    /// <returns>���ر���������component</returns>
    public Component ReuseComponent(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            Component componentToReuse = GetComponentFromPool(poolKey);

            ResetObject(position, rotation, componentToReuse, prefab);

            return componentToReuse;
        }else
        {
            Debug.Log("No object pool for " + prefab);
            return null;
        }
    }

    /// <summary>
    /// �Ӷ�����л�ȡ��Ӧkey�����component
    /// </summary>
    /// <returns>���ض�Ӧ�����component</returns>
    private Component GetComponentFromPool(int poolKey)
    {
        Component componentToReuse = poolDictionary[poolKey].Dequeue();
        poolDictionary[poolKey].Enqueue(componentToReuse);

        if (componentToReuse.gameObject.activeSelf == true)
        {
            componentToReuse.gameObject.SetActive(false);
        }

        return componentToReuse;
    }

    /// <summary>
    /// ����Ҫ�������������������ð���(position,rotation,localScale)
    /// </summary>
    private void ResetObject(Vector3 position, Quaternion rotation, Component componentToReuse, GameObject prefab)
    {
        componentToReuse.transform.position = position;
        componentToReuse.transform.rotation = rotation;
        componentToReuse.gameObject.transform.localScale = prefab.transform.localScale;
    }


    #region Validate
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckEnumerableValue(this,nameof(poolArray),poolArray);
    }
#endif
    #endregion
}