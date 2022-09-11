using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    private static MouseWorld Instance;

    [SerializeField] private LayerMask mousePlaneLayerMask;

    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;
    }

    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        Physics.Raycast(ray, out RaycastHit rayCast, float.MaxValue, Instance.mousePlaneLayerMask);
        return rayCast.point;
    }
}
