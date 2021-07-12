namespace redd096
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

#if UNITY_EDITOR

    using UnityEditor;

    [CustomEditor(typeof(FieldOfView2D))]
    public class FieldOfViewEditor2D : Editor
    {
        void OnSceneGUI()
        {
            //draw circle
            FieldOfView2D fov = (FieldOfView2D)target;
            Handles.color = Color.white;
            Handles.DrawWireArc(fov.transform.position, Vector3.back, fov.StartDirection, 360, fov.viewRadius);

            //draw 2 line to see cone vision
            Vector3 ViewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
            Vector3 ViewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);
            Handles.DrawLine(fov.transform.position, fov.transform.position + ViewAngleA * fov.viewRadius);
            Handles.DrawLine(fov.transform.position, fov.transform.position + ViewAngleB * fov.viewRadius);

            //show targets
            Handles.color = Color.red;
            foreach (Transform visibleTarget in fov.VisibleTargets)
            {
                Handles.DrawLine(fov.transform.position, visibleTarget.position);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            //button to show if hit targets
            if (GUILayout.Button("Update Targets"))
            {
                FieldOfView2D fov = (FieldOfView2D)target;
                fov.FindVisibleTargets();

                //repaint scene
                SceneView.RepaintAll();
            }
        }
    }

#endif

    [AddComponentMenu("redd096/Field Of View/Field Of View 2D")]
    public class FieldOfView2D : MonoBehaviour
    {
        [Header("Field of View")]
        public float viewRadius = 10;
        [Range(0, 360)] public float viewAngle = 30;
        [SerializeField] Transform pointDirection = default;

        [Header("Layers")]
        [SerializeField] LayerMask targetMask = -1;
        [SerializeField] LayerMask obstacleMask = default;

        List<Transform> visibleTargets = new List<Transform>();

        public List<Transform> VisibleTargets => visibleTargets;
        public Vector3 StartDirection => pointDirection ? (pointDirection.position - transform.position).normalized : Vector3.up;   //direction from our to pointDirection - else look up

        void OnEnable()
        {
            //start look every tot seconds
            StartCoroutine("FindTargetsWithDelay", 0.2f);
        }

        IEnumerator FindTargetsWithDelay(float delay)
        {
            while (true)
            {
                //wait then find targets
                yield return new WaitForSeconds(delay);
                FindVisibleTargets();
            }
        }

        #region public API

        public void FindVisibleTargets()
        {
            visibleTargets.Clear();

            //find targets
            Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

            //foreach target
            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                //check is in angle
                Transform target = targetsInViewRadius[i].transform;
                Vector2 dirToTarget = (target.position - transform.position).normalized;
                if (Vector2.Angle(StartDirection, dirToTarget) < viewAngle / 2)
                {
                    //throw raycast to see is not hide by an obstacle
                    float distToTarget = Vector2.Distance(transform.position, target.position);
                    if (Physics2D.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask).collider == null)
                    {
                        //add to list
                        visibleTargets.Add(target);
                    }
                }
            }
        }

        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            //direction cone vision
            if (angleIsGlobal == false)
            {
                //rotate to point direction
                float angle = Vector2.SignedAngle(Vector2.up, StartDirection);
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                angleInDegrees -= rotation.eulerAngles.z;
            }

            //direction from start direction (used to create one of two line of cone vision)
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
        }

        #endregion
    }
}