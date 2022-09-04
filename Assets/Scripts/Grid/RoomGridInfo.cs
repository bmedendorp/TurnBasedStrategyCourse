using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGridInfo : MonoBehaviour
{
    [HorizontalGroup("Room Bounds", LabelWidth = 40, PaddingRight = 20, MaxWidth = 50)]
    [SerializeField] 
    private int minX;

    [HorizontalGroup("Room Bounds", PaddingRight = 20, MaxWidth = 50)]
    [SerializeField] 
    private int maxX;
    
    [HorizontalGroup("Room Bounds", PaddingRight = 20, MaxWidth = 50)]
    [SerializeField] 
    private int minZ;
    
    [HorizontalGroup("Room Bounds", PaddingRight = 20, MaxWidth = 50)]
    [SerializeField] 
    private int maxZ;
    [SerializeField]

    private void Start()
    {
        AddRoomToLevelGrid();
    }

    private void AddRoomToLevelGrid()
    {
        Vector3 v1 = transform.position + transform.rotation * new Vector3(this.minX, 0f, this.minZ);
        Vector3 v2 = transform.position + transform.rotation * new Vector3(this.maxX, 0f, this.maxZ);

        int x1 = Mathf.RoundToInt(v1.x);
        int x2 = Mathf.RoundToInt(v2.x);
        int z1 = Mathf.RoundToInt(v1.z);
        int z2 = Mathf.RoundToInt(v2.z);

        LevelGrid.Instance.AddRoom(Mathf.Min(x1, x2), 
                                   Mathf.Max(x1, x2), 
                                   Mathf.Min(z1, z2), 
                                   Mathf.Max(z1, z2));
    }
}
