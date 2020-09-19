using RPG.Movement;
using RPG.Combat;
using UnityEngine;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Controller
{
    public class PlayerController : MonoBehaviour
    {
        Mover move;
        Fighter fight;
        Health health;

        

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float maxNavPathLength = 40f;

        // Start is called before the first frame update
        void Awake()
        {
            move = GetComponent<Mover>();
            fight = GetComponent<Fighter>();
            health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            if (InteractWithUI()) return;

            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;//Checks to see if the player is dead
            }

            if (InteractWithComponent()) return;

            //if (InteractWithCombat()) return;//Checks to see if the object we are hovering can be interacted via combat
            if (InteractWithMovement()) return;//checks to see if the object we are hovering can be moved to
            else if (!InteractWithMovement())
            {
                SetCursor(CursorType.None);
            }

            SetCursor(CursorType.None);
        }

        

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();//Raycasts all objects in the direction clicked (the tree and behind the tree)
            foreach (RaycastHit hit in hits)
            {
                IRayCastable[] rayCastables = hit.transform.GetComponents<IRayCastable>();
                foreach (IRayCastable rayCastable in rayCastables)
                {
                    if(rayCastable.HandleRaycast(this))
                    {
                        SetCursor(rayCastable.GetCursorType());
                        return true;
                    }
                }    
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());


            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
           
            Array.Sort(distances, hits);

            return hits;
        }

        private bool InteractWithUI()
        {

            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

     

        private bool InteractWithMovement()
        {
           
            Vector3 target;
            bool hasHit = RayCastNavMesh(out target);
            if (hasHit)//If it hit something, then it will move to the point in which you clicked
            {
                if (Input.GetMouseButton(0))
                {
                    move.StartMoveAction(target, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;//Otherwise if nothing is being hit (edge of the world) returns false;
        }


        private bool RayCastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            RaycastHit hit;//creates a variable that returns information based on what you hit with the ray
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit); //verifies that the ray hits a point and returns the information;

            if (!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);

            if (!hasCastToNavMesh) return false;

            target = navMeshHit.position;

            NavMeshPath path =  new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxNavPathLength) return false;
            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2) return total;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return total;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);// returns information based on the mouse position
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type) return mapping;

                
            }
            return cursorMappings[0];
        }
    }
       
}

