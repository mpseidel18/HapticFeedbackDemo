using UnityEngine;
using Oculus.Haptics;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // xri namespace for interactables

/// <summary>
/// attach to objects for haptics
/// needs trigger collider or xrinteractable
/// </summary>
public class HapticFeedbackPlayer : MonoBehaviour
{
    public enum TriggerMode
    {
        ProximityOnly,
        GrabOnly,
        ProximityAndGrab
    }

    [Header("Haptics Setup")]
    [Tooltip("assign haptics file")]
    public HapticClip hapticClip;

    [Tooltip("select trigger action")]
    public TriggerMode triggerMode = TriggerMode.ProximityOnly;

    [Tooltip("loop effect or play once")]
    public bool playContinuously = false;

    private HapticClipPlayer _hapticPlayer;
    private XRBaseInteractable _interactable;

    private void Awake()
    {
        // find interactable component
        _interactable = GetComponent<XRBaseInteractable>();
    }

    private void Start()
    {
        if (hapticClip != null)
        {
            _hapticPlayer = new HapticClipPlayer(hapticClip);
            // set looping state
            _hapticPlayer.isLooping = playContinuously;
        }
        else
        {
            Debug.LogWarning($"[HapticFeedbackPlayer] No HapticClip assigned on {gameObject.name}!");
        }
    }

    private void OnEnable()
    {
        if (_interactable != null)
        {
            _interactable.selectEntered.AddListener(OnGrabbed);
            _interactable.selectExited.AddListener(OnReleased);
        }
    }

    private void OnDisable()
    {
        if (_interactable != null)
        {
            _interactable.selectEntered.RemoveListener(OnGrabbed);
            _interactable.selectExited.RemoveListener(OnReleased);
        }
    }

    // grab logic
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (triggerMode == TriggerMode.ProximityOnly) return;

        // find haptic controller on interactor
        HapticController controller = args.interactorObject.transform.GetComponentInParent<HapticController>();
        if (controller != null && _hapticPlayer != null)
        {
            _hapticPlayer.Play(controller.controllerHand);
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (triggerMode == TriggerMode.ProximityOnly) return;

        if (playContinuously && _hapticPlayer != null)
        {
            _hapticPlayer.Stop();
        }
    }

    // proximity logic
    private void OnTriggerEnter(Collider other)
    {
        if (triggerMode == TriggerMode.GrabOnly) return;

        // check for haptic controller
        HapticController controller = other.GetComponentInParent<HapticController>();
        if (controller != null && _hapticPlayer != null)
        {
            _hapticPlayer.Play(controller.controllerHand);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerMode == TriggerMode.GrabOnly) return;

        // stop effect if leaving
        if (playContinuously)
        {
            HapticController controller = other.GetComponentInParent<HapticController>();
            if (controller != null && _hapticPlayer != null)
            {
                _hapticPlayer.Stop();
            }
        }
    }

    private void OnDestroy()
    {
        _hapticPlayer?.Dispose();
    }
}
