using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(SharedPropertiesTest))]
    public class Mover : MonoBehaviour
    {
        private void OnValidate()
        {
            _shared = GetComponent<SharedPropertiesTest>();
        }

        [SerializeField]
        private SharedPropertiesTest _shared;

        private static int moveSpeedID = SharedPropertiesTest.GetId("MoveSpeed");
        private ref float MoveSpeed => ref _shared.Get(moveSpeedID);
        private static int rotateSpeedID = SharedPropertiesTest.GetId("RotateSpeed");
        private ref float RotateSpeed => ref _shared.Get(rotateSpeedID);

        private void Update()
        {
            transform.position += transform.forward * MoveSpeed * Time.deltaTime;
            transform.Rotate(transform.up * RotateSpeed);
        }
    }
}