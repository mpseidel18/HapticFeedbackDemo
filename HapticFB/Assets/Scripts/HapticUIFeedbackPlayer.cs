using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Oculus.Haptics;

/// <summary>
/// attach to ui elements
/// routes haptics to clicking hand
/// </summary>
public class HapticUIFeedbackPlayer : MonoBehaviour, IPointerDownHandler
{
    [Header("Haptics Setup")]
    [Tooltip("assign haptics file")]
    public HapticClip hapticClip;

    private HapticClipPlayer _hapticPlayer;

    private void Start()
    {
        if (hapticClip != null)
        {
            _hapticPlayer = new HapticClipPlayer(hapticClip);
        }
        else
        {
            Debug.LogWarning($"[HapticUIFeedbackPlayer] No HapticClip assigned on {gameObject.name}!");
        }
    }

    // interface method for ui press
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_hapticPlayer == null) return;

        // verify xr interactor click
        if (eventData is TrackedDeviceEventData trackedEventData)
        {
            if (trackedEventData.interactor != null)
            {
                // cast interface to component
                Component interactorComponent = trackedEventData.interactor as Component;
                
                if (interactorComponent != null)
                {
                    // find haptic controller script
                    HapticController controller = interactorComponent.GetComponentInParent<HapticController>();
                    
                    if (controller != null)
                    {
                        _hapticPlayer.Play(controller.controllerHand);
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        _hapticPlayer?.Dispose();
    }
}
