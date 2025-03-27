using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Pool
{
    public class MonoPool<T> : MonoSingleton<MonoPool<T>> where T : MonoBehaviour, IPoolable
    {
        [SerializeField] private int initialPoolSize;
        [SerializeField] private int maxPoolSize;
    
        [SerializeField] private T prefab;
        [SerializeField] private Transform parent;
        private Stack<T> _availablePool;
        private int _elementSpawned;
        private int _currentElement;

        public int GetElementSpawned() => _elementSpawned;
        public int GetCurrentElementNum() => _currentElement;

        public void SetElementSpawned(int elementNum)
        {
            _elementSpawned = elementNum;
        }

        public void SetCurrentElementNum(int elementNum)
        {
            _currentElement = elementNum;
        }
        private void Awake()
        {
            _availablePool = new Stack<T>();
            AddItemsToPool();
            _elementSpawned = 0;
            _currentElement = 0;
        }
    



        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public T Get()
        {
            if (_availablePool.Count <= 2)
            {
                AddItemsToPool();
            }
            var pooledObject = _availablePool.Pop();
            pooledObject.gameObject.SetActive(true);
            _currentElement++;
            _elementSpawned++;
        
            pooledObject.Reset();
            return pooledObject;
        }
        public void Return(T pooledObject)
        {
            pooledObject.gameObject.SetActive(false);
            _availablePool.Push(pooledObject);
            _currentElement--;
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        private void AddItemsToPool()
        {
            for (int i = 0; i < maxPoolSize; i++)
            {
                var obj = Instantiate(prefab, parent, true);
                obj.gameObject.SetActive(false);
                _availablePool.Push(obj);
            }
        }
    }
}
