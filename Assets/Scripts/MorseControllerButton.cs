using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

[RequireComponent(typeof(Button))]
public class MorseOnClickLeftController : MonoBehaviour
{
    [Header("Morse 'R B G R'")]
    [Range(0f,1f)] public float amplitude = 0.85f;  // rumble strength
    [Tooltip("Dot length in seconds (0.07–0.12 feels good).")]
    public float unitSeconds = 0.09f;

    // optional: run on Awake if you want it pre-wired automatically
    [Tooltip("Automatically wire this Button.onClick on Awake.")]
    public bool autoWireOnClick = true;

    Button _btn;
    InputDevice _left;

    static readonly Dictionary<char,string> Morse = new() {
        { 'R', ".-." }, { 'B', "-..." }, { 'G', "--." }
    };

    void Awake()
    {
        _btn = GetComponent<Button>();
        if (autoWireOnClick) _btn.onClick.AddListener(PlayMorse);
        ResolveLeft();
        InputDevices.deviceConnected += _ => ResolveLeft();
        InputDevices.deviceDisconnected += _ => ResolveLeft();
    }

    void OnDestroy()
    {
        InputDevices.deviceConnected -= _ => ResolveLeft();
        InputDevices.deviceDisconnected -= _ => ResolveLeft();
        StopLeftHaptics();
    }

    public void PlayMorse()
    {
        StopAllCoroutines();
        StartCoroutine(PlayMorseOnce("R B G R"));
    }

    IEnumerator PlayMorseOnce(string msg)
    {
        if (!_left.isValid) ResolveLeft();
        msg = msg.ToUpperInvariant();

        for (int i = 0; i < msg.Length; i++)
        {
            char c = msg[i];

            if (c == ' ')
            {
                yield return new WaitForSeconds(7f * unitSeconds); // word gap
                continue;
            }

            if (!Morse.TryGetValue(c, out string code)) continue;

            // emit each dot/dash
            for (int k = 0; k < code.Length; k++)
            {
                float dur = (code[k] == '.') ? unitSeconds : 3f * unitSeconds;
                SendImpulse(amplitude, dur);
                yield return new WaitForSeconds(dur);
                StopLeftHaptics();

                // intra-symbol gap (except after last)
                if (k < code.Length - 1) yield return new WaitForSeconds(1f * unitSeconds);
            }

            // letter gap
            if (i < msg.Length - 1) yield return new WaitForSeconds(3f * unitSeconds);
        }
    }

    void ResolveLeft() => _left = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

    void SendImpulse(float amp, float seconds)
    {
        if (!_left.isValid) return;
        if (_left.TryGetHapticCapabilities(out var caps) && caps.supportsImpulse)
            _left.SendHapticImpulse(0, Mathf.Clamp01(amp), Mathf.Max(0.01f, seconds));
    }

    void StopLeftHaptics()
    {
        if (!_left.isValid) return;
        if (_left.TryGetHapticCapabilities(out var caps) && (caps.supportsImpulse || caps.supportsBuffer))
            _left.StopHaptics();
    }
}
