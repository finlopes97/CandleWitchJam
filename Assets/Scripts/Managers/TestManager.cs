using UnityEngine;
using UnityEngine.Rendering;

namespace Managers
{
    public class TestManager : MonoBehaviour
    {
        void Awake()
        {
            DebugManager.instance.enableRuntimeUI = false;
        }
    }
}
