using UnityEngine;
using UnityEngine.Events;

namespace MSB
{
    public class PlayerTriggerEvents : MonoBehaviour
    {
        public UnityEvent TriggerEnter;
        public UnityEvent TriggerExit;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                TriggerEnter.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                TriggerEnter.Invoke();
            }
        }
    }
}