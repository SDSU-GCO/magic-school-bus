using UnityEngine;

namespace MSB
{
    [RequireComponent(typeof(CharacterController))]
    public class TestMovement : MonoBehaviour
    {
        public float m_moveSpeed = 3f;
        public Transform CameraTransform;

        private CharacterController m_CharacterController;

        private void Awake()
        {
            m_CharacterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            // Todo: Rename axis once actual input scheme is decided.
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            input         = MathUtilities.TransformInputFromCameraToPlayer(input, CameraTransform, transform);

            //Normally the corrected input would drive Animation Blending parameters, but for now just manipulate the model directly.
            if (input.sqrMagnitude > 0.0001f)
            {
                float angle = Vector3.SignedAngle(Vector3.forward, input.normalized, Vector3.up);
                transform.Rotate(0f, angle, 0f);
                m_CharacterController.SimpleMove(transform.forward * m_moveSpeed * input.magnitude);
            }
        }
    }
}