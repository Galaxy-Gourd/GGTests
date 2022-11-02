using System;
using GG.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GG.Tests
{
    /// <summary>
    /// Controls UI behavior of prefab 'PF_UITestInstance'
    /// </summary>
    internal class UITestInstance : UIView<UITestInstance.Parameters>
    {
        #region PARAMETERS

        internal struct Parameters
        {
            public DataConfigTest TestData;
            
            public Action<DataConfigTest> DoTestAutoRunButtonSelected;
            public Action<DataConfigTest> DoTestManualRunButtonSelected;
        }

        #endregion PARAMETERS
        
        
        #region VARIABLES

        [Header("References")]
        [SerializeField] private TMP_Text _testTestTitle;
        [SerializeField] private Button _buttonRunTestAuto;
        [SerializeField] private Button _buttonRunTestManual;

        #endregion VARIABLES


        #region VIEW SETUP

        protected override void SetParameters()
        {
            _testTestTitle.text = _parameters.TestData.TestName;
            _buttonRunTestAuto.interactable = _parameters.TestData.HasAutoTest;
            _buttonRunTestManual.interactable = _parameters.TestData.HasManualTest;
        }

        protected override void SetupCallbacks()
        {
            _buttonRunTestAuto.onClick.AddListener(OnTestAutoRunButtonSelected);
            _buttonRunTestManual.onClick.AddListener(OnTestManualRunButtonSelected);
        }

        protected override void CleanupCallbacks()
        {
            _buttonRunTestAuto.onClick.RemoveListener(OnTestAutoRunButtonSelected);
            _buttonRunTestManual.onClick.RemoveListener(OnTestManualRunButtonSelected);
        }

        #endregion VIEW SETUP


        #region ACTIONS

        private void OnTestAutoRunButtonSelected()
        {
            _parameters.DoTestAutoRunButtonSelected?.Invoke(_parameters.TestData);
        }
        
        private void OnTestManualRunButtonSelected()
        {
            _parameters.DoTestManualRunButtonSelected?.Invoke(_parameters.TestData);
        }

        #endregion ACTIONS
    }
}

