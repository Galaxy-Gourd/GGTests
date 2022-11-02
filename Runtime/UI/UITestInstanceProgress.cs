using GG.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GG.Tests 
{
    public class UITestInstanceProgress : UIView<UITestInstanceProgress.Parameters>
    {
        #region PARAMETERS

        public struct Parameters
        {
            
        }

        #endregion PARAMETERS


        #region VARIABLES

        [Header("References")]
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private GameObject _objIndicatorRunning;
        [SerializeField] private GameObject _objIndicatorError;
        [SerializeField] private GameObject _objIndicatorSuccess;
        [SerializeField] private TMP_Text _textCurrentStep;

        #endregion VARIABLES


        #region VIEW SETUP

        protected override void SetParameters()
        {
            
        }

        protected override void SetupCallbacks()
        {
            
        }

        protected override void CleanupCallbacks()
        {
            
        }

        #endregion VIEW SETUP


        #region TEST UPDATE

        public void UpdateTestProgress(float pcntComplete, string nextStep)
        {
            _progressSlider.value = pcntComplete;
            _textCurrentStep.text = nextStep;
        }

        public void UpdateTestStatus(TestStatus status)
        {
            // Deactivate all indicators to reset view
            _objIndicatorRunning.SetActive(false);
            _objIndicatorError.SetActive(false);
            _objIndicatorSuccess.SetActive(false);
            
            switch (status)
            {
                case TestStatus.Running: 
                    _objIndicatorRunning.SetActive(true);
                    break;
                
                case TestStatus.CompleteSuccess: 
                    _objIndicatorSuccess.SetActive(true);
                    _progressSlider.value = 1;
                    break;
                
                case TestStatus.CompleteFailure: 
                    _objIndicatorError.SetActive(true);
                    break;
            }
        }

        internal void ResetUI()
        {
            _objIndicatorRunning.SetActive(false);
            _objIndicatorError.SetActive(false);
            _objIndicatorSuccess.SetActive(false);
            _progressSlider.value = 0;
            _textCurrentStep.text = "";
        }

        #endregion TEST UPDATE
    }
}