using System;
using System.Collections;
using System.Collections.Generic;
using GG.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace GG.Tests
{
    /// <summary>
    /// Controls the execution of tests from the TestRunner scene.
    /// </summary>
    public class TestRunner : MonoBehaviour, ITickable
    {
        #region VARIABLES

        [Header("Tests")]
        [SerializeField] private DataConfigTestsManifest _systemTests;
        [SerializeField] private DataConfigTestsManifest _projectTests;
        [SerializeField] private DataConfigTest _defaultTest;
        [SerializeField] private bool _autoTestDefault;
        
        [Header("References")]
        [SerializeField] private UITestSelection _uiSelector;
        [SerializeField] private UITestInstanceProgress _uiTestProgress;
        [SerializeField] private EventSystem _runnerEventSystem;
        [SerializeField] private Camera _runnerCamera;

        public TickGroup TickGroup => TickGroup.Input;

        private TestController _currentTest;
        private DataConfigTest _currentTestData;
        private bool _isRunningAutomated;
        private int _currentAutomatedTestIndex = -1;
        private readonly List<Tuple<DataConfigTest, UITestInstance>> _uiTestsInstances = new();

        #endregion VARIABLES


        #region INITIALIZATION

        private void Awake()
        {
            // Initialize UI selector
            _uiSelector.SetParameters(new UITestSelection.Parameters
            {
                DoRunAutomatedTestsSelected = DoBeginAutomatedTestingButtonSelected,
                OnBackToRunnerButtonSelected = DoSelectReturnToRunnerButton
            });
            
            // Setup lists of tests
            SpawnTestInstancesUI(true);
            SpawnTestInstancesUI(false);
        }

        private void SpawnTestInstancesUI(bool trueSystemFalseProject)
        {
            DataConfigTestsManifest manifest = trueSystemFalseProject ? _systemTests : _projectTests;
            
            // Setup list of tests
            for (int i = 0; i < manifest.OrderedTestsData.Length; i++)
            {
                // Create new test UI prefab
                Tuple <DataConfigTest, UITestInstance>thisTest = new Tuple<DataConfigTest, UITestInstance>(
                    manifest.OrderedTestsData[i], 
                    _uiSelector.AddTestInstance(manifest.OrderedTestsData[i], trueSystemFalseProject));
                
                // Setup UI instance with parameters
                thisTest.Item2.SetParameters(new UITestInstance.Parameters()
                {
                    TestData = thisTest.Item1,
                    
                    DoTestAutoRunButtonSelected = OnRunTestAutoSelected,
                    DoTestManualRunButtonSelected = OnRunTestManualSelected
                });
                
                // Add to dictionary for future access
                _uiTestsInstances.Add(thisTest);
            }
        }

        private void OnEnable()
        {
            ConfigureRunnerForTestSelection();
            
            // Spawn into default test if needed
            if (_autoTestDefault && _defaultTest)
            {
                OnRunTestManualSelected(_defaultTest);
            }
        }

        #endregion INITIALIZATION


        #region RUNNER INPUT
        
        void ITickable.Tick(float delta)
        {
            InputSystem.Update();
        }
        
        #endregion RUNNER INPUT


        #region NAVIGATION

        private void DoSelectReturnToRunnerButton()
        {
            StartCoroutine(CR_UnloadCurrentTest());
        }

        private void DoBeginAutomatedTestingButtonSelected()
        {
            _isRunningAutomated = true;
            _currentAutomatedTestIndex = -1;
            OnRunTestAutoSelected(GetNextAutoTest());
        }
 
        #endregion NAVIGATION


        #region TESTING

        private void OnRunTestAutoSelected(DataConfigTest test)
        {
            if (!test)
                return;
            
            TestController controller = LoadTest(test);
            controller.BeginAutomatedTest(this, _isRunningAutomated);
        }
        
        private void OnRunTestManualSelected(DataConfigTest test)
        {
            LoadTest(test);
        }

        private TestController LoadTest(DataConfigTest test)
        {
            // Instantiate modules for this test
            Modules.LoadModules(test.Modules);
            
            // Instantiate test controller
            _currentTest = Instantiate(test.ControllerPrefab.gameObject).GetComponent<TestController>();
            _currentTest.Init(test, this, _uiTestProgress);
            _currentTestData = test;
            
            // Turn off test runner menu things
            ConfigureRunnerForTestExecution();

            return _currentTest;
        }

        private IEnumerator CR_UnloadCurrentTest()
        {
            // Destroy test
            Destroy(_currentTest.gameObject);
            yield return null;
            Modules.DestroyModules();
            
            //
            ConfigureRunnerForTestSelection();

            // If we're in the middle of an automated sequence, we want to run the next test
            if (_isRunningAutomated)
            {
                yield return new WaitForSeconds(0.25f);

                if (_uiTestsInstances.Count > _currentAutomatedTestIndex)
                {
                    OnRunTestAutoSelected(GetNextAutoTest());
                }
                else
                {
                    OnAutomatedTestsFinished();
                }
            }
        }
        
        private void ConfigureRunnerForTestSelection()
        { 
            // We need to tick the input system in the test runner to be able to select the buttons
            TickRouter.Register(this);
            _runnerCamera.enabled = true;
            _runnerEventSystem.enabled = true;
            _uiSelector.gameObject.SetActive(true);
            _uiTestProgress.gameObject.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void ConfigureRunnerForTestExecution()
        {
            // We want to unregister so that the test being run has full control
            TickRouter.Unregister(this);
            _runnerCamera.enabled = !_currentTestData.HideRunnerCamera;
            _runnerEventSystem.enabled = _currentTestData.ForceRunnerInputModule;
            _uiSelector.gameObject.SetActive(false);
            _uiTestProgress.gameObject.SetActive(true);
            _uiTestProgress.ResetUI();
        }

        #endregion TESTING


        #region RESULTS

        /// <summary>
        /// Called from a test controller when the test has finished
        /// </summary>
        /// <param name="success"></param>
        internal void TestCompleted(bool success = true)
        {
            
        }

        /// <summary>
        /// Called from a test controller when it has finished, and we are running automated tests in sequence
        /// </summary>
        internal void OnAutoReturnFromTest()
        {
            StartCoroutine(CR_UnloadCurrentTest());
        }
        
        private void OnAutomatedTestsFinished()
        {
            Debug.Log("All automated tests completed");
            _isRunningAutomated = false;
        }

        #endregion RESULTS


        #region UTILITY

        private DataConfigTest GetNextAutoTest()
        {
            DataConfigTest data = null;
            for (int i = _currentAutomatedTestIndex; i < _uiTestsInstances.Count - 1; i++)
            {
                int adder = (i + 1) - _currentAutomatedTestIndex;
                if (_uiTestsInstances[_currentAutomatedTestIndex + adder].Item1.HasAutoTest)
                {
                    data = _uiTestsInstances[_currentAutomatedTestIndex + adder].Item1;
                    _currentAutomatedTestIndex += adder;
                    break;
                }
            }

            // Finish auto-tests 
            if (!data)
            {
                _currentAutomatedTestIndex = _uiTestsInstances.Count - 1;
                OnAutomatedTestsFinished();
            }
            
            return data;
        }

        #endregion UTILITY
    }
}
