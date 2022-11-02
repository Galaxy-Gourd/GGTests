using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GG.Tests
{
    public class TEST_TestInstance : TestController
    {
        #region VARIABLES

        

        #endregion VARIABLES
    
    
        #region INITIALIZATION

        protected override void DoModulesLoadComplete()
        {
            base.DoModulesLoadComplete();

            
        }

        #endregion INITIALIZATION

        
        #region AUTOMATED TESTING

        protected override IEnumerator CR_AutoTest()
        {
            // TEST MOVE LEFT
            _testProgressUI.UpdateTestProgress(0f, "Testing timing...");
            yield return new WaitForSeconds(1f);
            _testProgressUI.UpdateTestProgress(0.25f, "Testing timing... [2]");
            yield return new WaitForSeconds(0.25f);
            _testProgressUI.UpdateTestProgress(0.5f, "Testing timing... [3]");
            yield return new WaitForSeconds(0.25f);
            _testProgressUI.UpdateTestProgress(0.75f, "Testing timing... [4]");
            yield return new WaitForSeconds(0.25f);
            _testProgressUI.UpdateTestProgress(1f, "Testing complete");
            yield return new WaitForSeconds(0.5f);

            StartCoroutine(AutoTestFinished());
        }

        #endregion AUTOMATED TESTING


        #region MANUAL TESTING

        

        #endregion MANUAL TESTING
    }
}
