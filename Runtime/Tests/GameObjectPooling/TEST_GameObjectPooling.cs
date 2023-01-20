using System;
using System.Collections;
using System.Collections.Generic;
using GG.Core;
using GG.Core.Data;
using GG.Core.GameObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GG.Tests
{
    public class TEST_GameObjectPooling : TestController, ITickable
    {
        #region VARIABLES

        [Header("References")]
        [SerializeField] private Transform _spawnPosition;
        [SerializeField] private DataConfigPool _poolData;
        
        public TickGroup TickGroup => TickGroup.DefaultGroupTenthSecond;

        private PoolGameObject _pool;
        
        #endregion VARIABLES
        
        
        #region INITIALIZATION

        protected override void DoModulesLoadComplete()
        {
            base.DoModulesLoadComplete();

            PoolManager.GetAndSetPoolData(_poolData);
        }

        private void OnEnable()
        {
            _poolData.OnValidated += OnPoolDataValidated;
            TickRouter.Register(this);
        }

        private void OnDisable()
        {
            _poolData.OnValidated -= OnPoolDataValidated;
            TickRouter.Unregister(this);
        }

        #endregion INITIALIZATION


        #region TICK

        void ITickable.Tick(float delta)
        {
            Vector3 spawnPos = _spawnPosition.position + (Vector3.right * (0.1f * Random.value));
            PoolManager.Pooled(_poolData, spawnPos, Quaternion.identity);
        }

        #endregion TICK


        #region POOL

        private void OnPoolDataValidated()
        {
            PoolManager.GetAndSetPoolData(_poolData);
        }

        #endregion POOL
    }
}
