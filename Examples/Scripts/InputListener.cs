using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(SharedPropertiesTest))]
    public class InputListener : MonoBehaviour
    {
        private SharedPropertiesTest _shared;

        private static int moveSpeedID = SharedPropertiesTest.GetId("MoveSpeed");
        private ref float MoveSpeed => ref _shared.Get(moveSpeedID);
        private static int rotateSpeedID = SharedPropertiesTest.GetId("RotateSpeed");
        private ref float RotateSpeed => ref _shared.Get(rotateSpeedID);

        private void Awake()
        {
            _shared = GetComponent<SharedPropertiesTest>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                MoveSpeed += 0.25f;
                RotateSpeed += 0.25f;
            }
        }
    }
}