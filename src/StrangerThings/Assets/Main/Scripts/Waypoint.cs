using UnityEngine;

namespace Assets.Main.Scripts
{
    public class Waypoint : MonoBehaviour
    {
        [SerializeField]
        private Waypoint[] connections;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDrawGizmos()
        {
            foreach (var waypoint in connections)
            {
                if (waypoint is null)
                {
                    continue;
                }
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, waypoint.transform.position);
            }
        }
    }
}