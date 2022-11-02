using GG.Core;
using UnityEngine;

namespace GG.Tests
{
    [CreateAssetMenu(
        fileName = "DAT_TestManifest", 
        menuName = "GG/Testing/Test Manifest")]
    public class DataConfigTestsManifest : DataConfig
    {
        [SerializeField] internal DataConfigTest[] OrderedTestsData;
    }
} 