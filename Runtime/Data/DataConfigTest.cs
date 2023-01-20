using System.Collections;
using GG.Core;
using UnityEngine;

namespace GG.Tests
{
    [CreateAssetMenu(
        fileName = "DAT_Test_", 
        menuName = "GG/Testing/New Test Data")]
    public class DataConfigTest : ScriptableObject
    {
        #region DATA

        [Header("Meta")]
        [SerializeField] internal string TestName;
        [SerializeField] internal bool HasAutoTest;
        [SerializeField] internal bool HasManualTest;
        [SerializeField] internal bool HideRunnerCamera = true;
        [SerializeField] internal bool ForceRunnerInputModule;
        
        [Header("References")]
        [SerializeField] internal TestController ControllerPrefab;
        [Tooltip("The modules to load for this test's execution")]
        [SerializeField] internal Module[] Modules;

        [Header("Timing")]
        [SerializeField] internal float TimeDelayBeforeTestBegin = 0.1f;
        [SerializeField] internal float TimeDelayAfterTestEnd = 0.5f;

        #endregion DATA
    }
}