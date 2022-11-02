using System;
using GG.Core;
using UnityEngine;
using UnityEngine.UI;

namespace GG.Tests
{
    internal class UITestSelection : UIView<UITestSelection.Parameters>
    {
        #region PARAMETERS

        internal struct Parameters
        {
            internal Action DoRunAutomatedTestsSelected;
            internal Action OnBackToRunnerButtonSelected;
        }

        #endregion PARAMETERS
        
        
        #region VARIABLES

        [Header("References")]
        [SerializeField] private Transform _testInstanceParentSystem; 
        [SerializeField] private Transform _testInstanceParentProject; 
        [SerializeField] private GameObject _prefabUITestInstance;
        [SerializeField] private Button _buttonRunAutomatedTests;
        [SerializeField] private GameObject _panelRunnerUI;
        [SerializeField] private GameObject _panelSceneTestBackButton;
        [SerializeField] private Button _buttonSceneTestBackButton;

        #endregion VARIABLES


        #region VIEW SETUP

        protected override void SetParameters()
        {
            
        }
        
        protected override void SetupCallbacks()
        {
            _buttonRunAutomatedTests.onClick.AddListener(OnRunAutomatedTestsSelected);
            _buttonSceneTestBackButton.onClick.AddListener(OnBackToTestRunnerButtonSelected);
        }

        protected override void CleanupCallbacks()
        {
            _buttonRunAutomatedTests.onClick.RemoveListener(OnRunAutomatedTestsSelected);
            _buttonSceneTestBackButton.onClick.RemoveListener(OnBackToTestRunnerButtonSelected);
        }

        #endregion VIEW SETUP


        #region LOAD

        internal UITestInstance AddTestInstance(DataConfigTest testinstanceData, bool trueSystemFalseProject)
        {
            Transform parent = trueSystemFalseProject ? _testInstanceParentSystem : _testInstanceParentProject;
            UITestInstance thisTest = Instantiate(_prefabUITestInstance, parent).GetComponent<UITestInstance>();

            return thisTest;
        }

        internal void OnRunnerSceneActivated(bool activated)
        {
            _panelRunnerUI.SetActive(activated);
            _panelSceneTestBackButton.SetActive(!activated);
        }

        #endregion LOAD


        #region ACTIONS

        private void OnRunAutomatedTestsSelected()
        {
            _parameters.DoRunAutomatedTestsSelected?.Invoke();
        }

        private void OnBackToTestRunnerButtonSelected()
        {
            _parameters.OnBackToRunnerButtonSelected?.Invoke();
        }

        #endregion ACTIONS
    }
}
