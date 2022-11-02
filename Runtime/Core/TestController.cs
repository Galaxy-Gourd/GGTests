using System.Collections;
using GG.Core;
using UnityEngine;

namespace GG.Tests
{
    /// <summary>
    /// Controls an individual test
    /// </summary>
    public abstract class TestController : MonoBehaviour
    {
        #region VARIABLES

        protected bool _modulesLoaded;
        protected UITestInstanceProgress _testProgressUI;
        protected TestRunner _runner;
        private bool _testIsRunning;
        private bool _shouldAutoExitTest;
        private DataConfigTest _testData;
        
        #endregion VARIABLES
        
        
        #region INITIALIZATION

        internal void Init(DataConfigTest testData, TestRunner runner, UITestInstanceProgress progress)
        {
            _runner = runner;
            _testProgressUI = progress;
            _testData = testData;
        }

        private void Start()
        {
            Modules.SubscribeToLoadComplete(DoModulesLoadComplete);
            StartCoroutine(CR_StartTestWhenLoadIsReady());
        }

        protected virtual void DoModulesLoadComplete()
        {
            Modules.OnModulesLoadComplete -= DoModulesLoadComplete;
            _modulesLoaded = true;
        }

        private IEnumerator CR_StartTestWhenLoadIsReady()
        {
            // Can;t start test until modules have loaded
            while (!_modulesLoaded)
            {
                yield return null;
            }

            if (!_testIsRunning)
            {
                _testIsRunning = true;
                if (_testData.HasManualTest)
                {
                    BeginManualTest();
                }
                else
                {
                    StartCoroutine(CR_AutoTestSetup());
                }
            }
        }

        private void OnDestroy()
        {
            CleanupTest();
        }

        #endregion INITIALIZATION
        

        #region AUTOMATED TESTING

        /// <summary>
        /// Called from TestRunner when executing automated testing
        /// </summary>
        internal void BeginAutomatedTest(TestRunner runner, bool doAutoExit)
        {
            _runner = runner;
            _shouldAutoExitTest = doAutoExit;
            if (!_testIsRunning)
            {
                _testIsRunning = true;
                StartCoroutine(CR_AutoTestSetup());
            }
        }

        private IEnumerator CR_AutoTestSetup()
        {
            // Wait for modules to be loaded
            while (!_modulesLoaded)
            {
                yield return null;
            }
            
            // Flip over to auto testing
            StopCoroutine(CR_ManualTest());
            yield return new WaitForSeconds(_testData.TimeDelayBeforeTestBegin);
            StartCoroutine(CR_AutoTest());
        }

        // We override this method to implement specific automatic tests
        protected virtual IEnumerator CR_AutoTest()
        {
            yield return null;
        }

        protected IEnumerator AutoTestFinished(bool success = true)
        {
            StopCoroutine(CR_AutoTest());
            _testProgressUI.UpdateTestStatus(success ? TestStatus.CompleteSuccess : TestStatus.CompleteFailure);

            yield return new WaitForSeconds(_testData.TimeDelayAfterTestEnd);

            // Return control to the runner if it exists
            if (_runner)
            {
                _runner.OnAutoReturnFromTest();
            }

            yield return null;
        }

        #endregion AUTOMATED TESTING


        #region MANUAL TESTING

        protected virtual void BeginManualTest()
        {
            StartCoroutine(CR_ManualTest());
        }

        protected virtual IEnumerator CR_ManualTest()
        {
            while (true)
            {
                yield return null;
            }
        }

        #endregion MANUAL TESTING


        #region CLEANUP

        protected virtual void CleanupTest()
        {
            
        }

        #endregion CLEANUP
    }
}

