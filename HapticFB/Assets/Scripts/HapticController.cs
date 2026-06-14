using UnityEngine;
using Oculus.Haptics;

/// <summary>
/// attach to xr controller
/// identifies hand for haptics
/// </summary>
public class HapticController : MonoBehaviour
{
    [Tooltip("select controller hand")]
    public Controller controllerHand = Controller.Right;
}
