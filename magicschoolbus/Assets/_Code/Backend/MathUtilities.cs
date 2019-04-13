using UnityEngine;

namespace MSB
{
    public static class MathUtilities
    {
        /// <summary>
        /// Rotates the input values from the camera's perspective to the player's perspective.
        /// If the player presses left while the player's character is facing left, this function's output will be forward relative to the player character.
        /// </summary>
        /// <param name="input">The input to be remapped. x is horizontal. z is vertical.</param>
        /// <param name="camera">The active camera's transform.</param>
        /// <param name="player">The player character's transform.</param>
        /// <returns>The remapped input such that x is the player character's left and right and y is the player character's forward and backward.</returns>
        public static Vector3 TransformInputFromCameraToPlayer(Vector3 input, Transform camera, Transform player)
        {
            //Cannot use Transform.TransformDirection as it modifies y.
            Vector3 cameraForward = camera.forward;
            Vector3 playerForward = player.forward;
            cameraForward.y       = 0f;
            playerForward.y       = 0f;

            // I think I projected this backwards, but because Unity uses +clockwise, it somehow works out.
            float angle = Vector3.SignedAngle(cameraForward, playerForward, Vector3.up);

            angle            = Mathf.Deg2Rad * angle;
            Vector3 xRotated = input.x * new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            Vector3 zRotated = input.z * new Vector3(-Mathf.Sin(angle), 0f, Mathf.Cos(angle));

            return xRotated + zRotated;
        }

        /// <summary>
        /// Takes any angle, and returns the equivalent in the range of -180 to 180 degrees.
        /// </summary>
        /// <param name="angle">The angle to simplify in degrees</param>
        /// <returns>The simplified angle between -180 and 180 degrees</returns>
        public static float WrapAngle180(float angle)
        {
            if (angle > 180f)
            {
                float loops = Mathf.Floor(angle / 360f);
                angle -= loops * 360f;
                return angle > 180f ? angle - 360f : angle;
            }
            else if (angle < -180f)
            {
                float loops  = Mathf.Floor(-angle / 360f);
                angle       += loops * 360f;
                return angle < -180f ? angle + 360f : angle;
            }
            return angle;
        }
    }
}