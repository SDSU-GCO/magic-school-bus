using Cinemachine;
using UnityEngine;

// Written by Dreaming I'm Latios
// If this is not sufficient, please let me know. It can be improved.
// Also, the code is not fully optimized. It shouldn't be a big deal since there should only be a small number of these running.
// Usage: Leave the freelook's x-axis value range between -180 and 180. Also do not rotate the freelook's gameobject.

// Fascination = What the camera is tracking.

namespace MSB
{
    [RequireComponent(typeof(CinemachineFreeLook))]
    public class FreelookRubberBand : MonoBehaviour
    {
        [Tooltip("Higher values will cause the camera to auto-orbit more aggressively.")]
        public float                m_followBias         = 1f;
        public float                m_autoOrbitMaxSpeed  = 45f;
        public float                m_inputOrbitSpeed    = 60f;
        public string               m_bindingXAxis       = "";
        public string               m_bindingResetButton = "";
        private CinemachineFreeLook m_Freelook;
        private Vector3             m_prevTargetPosition;

        private void Awake()
        {
            m_Freelook                          = GetComponent<CinemachineFreeLook>();
            m_prevTargetPosition                = m_Freelook.m_Follow.position;
            m_Freelook.m_BindingMode            = CinemachineTransposer.BindingMode.WorldSpace;
            m_Freelook.m_XAxis.m_InputAxisName  = "";
            m_Freelook.m_XAxis.m_InputAxisValue = 0f;
        }

        private void LateUpdate()
        {
            if (m_bindingXAxis.Length > 0)
                m_Freelook.m_XAxis.Value += Input.GetAxis(m_bindingXAxis) * m_inputOrbitSpeed * Time.deltaTime;
            if (m_bindingResetButton.Length > 0)
            {
                if (Input.GetButtonDown(m_bindingResetButton))
                {
                    m_Freelook.m_YAxis.Value = 0.5f;
                    Vector3 targetForward    = m_Freelook.m_Follow.forward;
                    targetForward.y          = 0f;
                    m_Freelook.m_XAxis.Value = Vector3.SignedAngle(transform.forward, targetForward, Vector3.up);
                }
            }

            // Unity API uses positive angles in clockwise direction. This makes the trig confusing.
            // So I reverse the sign immediately and then reverse it back on writeback.
            Vector3 currTargetPosition = m_Freelook.m_Follow.position;
            Vector3 deltaPosition      = currTargetPosition - m_prevTargetPosition;
            deltaPosition.y            = 0f;
            // If our fascination isn't moving, neither should the camera.
            // Note that the soft damping will still apply with an idle fascination. That's fine.
            if (deltaPosition.sqrMagnitude < 0.0001f)
                return;

            // We need the radius and the angle of our camera relative to the fascination's forward direction.
            Vector3 planarCameraOffset = currTargetPosition - m_Freelook.State.RawPosition;
            planarCameraOffset.y       = 0f;
            float radius               = planarCameraOffset.magnitude;
            float currAngle            = -m_Freelook.m_XAxis.Value; //This also uses +clockwise
            float movedAngle           = -Vector3.SignedAngle(transform.forward, deltaPosition, Vector3.up); //SignedAngle returns +clockwise
            float currRelativeAngle    = MathUtilities.WrapAngle180(currAngle - movedAngle);
            float currRelativeAngleCos = Mathf.Cos(currRelativeAngle * Mathf.Deg2Rad);
            float targetRelativeAngle;

            // Since we are now relative to the fascination's direction, the deltaPosition is always the forward position.
            // This means that the cosine of our relativeAngle is how far behind the fascination's back the camera is.
            // What we are doing is adding a nudge to this offset and then calculating a new angle.
            // radius / m_followBias is our reference circle. The smaller the circle, the more a bias in world-units affects the angle.
            // We then remap this onto a unit circle.
            if (currRelativeAngleCos + deltaPosition.magnitude * m_followBias / radius >= 1f)
            {
                // Our offset moved us too far behind our fascination that our camera's radius is smaller.
                // So we just snap the camera to the angle behind our target.
                targetRelativeAngle = 0f;
            }
            else
            {
                // Arc cosine always returns an angle between 0 and 180.
                // This means that if our original angle was negative, our camera will "jump" to the other side (left side to right side).
                // We correct this by multiplying by the sign of our original angle.
                targetRelativeAngle = Mathf.Acos(currRelativeAngleCos + deltaPosition.magnitude * m_followBias / radius) * Mathf.Rad2Deg * Mathf.Sign(currRelativeAngle);
            }
            // Now we know our goal angle, we need to remap it back to world space.
            // We also limit how much we can rotate towards our goal angle.
            // That helps stop the sudden jerk when our camera get close to right behind the fascination.
            float deltaAngle         = targetRelativeAngle - currRelativeAngle;
            deltaAngle               = Mathf.Clamp(deltaAngle, -m_autoOrbitMaxSpeed * Time.deltaTime, m_autoOrbitMaxSpeed * Time.deltaTime);
            m_Freelook.m_XAxis.Value = -MathUtilities.WrapAngle180(currAngle + deltaAngle);
            m_prevTargetPosition     = currTargetPosition;
        }
    }
}