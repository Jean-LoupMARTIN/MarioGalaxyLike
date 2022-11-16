using System.Collections.Generic;
using UnityEngine;




public class Gravity : MonoBehaviour
{
    static public Gravity inst;

    [SerializeField] float acceleration = 40f;

    Dictionary<Vector3Int, (Vector3 dir, float dist, GravityField field)> gridStaticField = new Dictionary<Vector3Int, (Vector3, float, GravityField)>();
    [SerializeField] bool useGrid = false;
    [SerializeField] float gridCellSize = 1;

    [SerializeField] bool drawGrid = true;
    [SerializeField] float arrowLength = 0.5f;
    [SerializeField] Gradient gradient;
    [SerializeField] float distAtGradientMax = 10;


    public float Acceleration { get => acceleration; }


    void OnDrawGizmos()
    {
        if (drawGrid)
            DrawGrid();
    }

    void DrawGrid()
    {
        foreach (KeyValuePair<Vector3Int, (Vector3 dir, float dist, GravityField field)> cell in gridStaticField)
        {
            Vector3 worldPos = GridToWorld(cell.Key);
            Vector3 dir;
            float dist;
            (dir, dist, _) = cell.Value;
            Gizmos.color = gradient.Evaluate(Mathf.InverseLerp(0, distAtGradientMax, dist));
            GizmosExtension.DrawArrow(worldPos, worldPos + dir * arrowLength, arrowLength * 0.2f);
        }
    }

    void OnValidate()
    {
        gridStaticField.Clear();
    }

    void Awake()
    {
        inst = this;
        gridStaticField.Clear();
    }


    Vector3Int WorldToGrid(Vector3 worldPos) => new Vector3Int(Mathf.RoundToInt(worldPos.x / gridCellSize),
                                                               Mathf.RoundToInt(worldPos.y / gridCellSize),
                                                               Mathf.RoundToInt(worldPos.z / gridCellSize));

    Vector3 GridToWorld(Vector3Int gridPos) => (Vector3)gridPos * gridCellSize;


    (Vector3 dir, float dist, GravityField field) Grid(Vector3 worldPos) => Grid(WorldToGrid(worldPos));
    (Vector3 dir, float dist, GravityField field) Grid(Vector3Int gridPos)
    {
        // new cell
        if (!gridStaticField.ContainsKey(gridPos))
            gridStaticField[gridPos] = GravityField.AtPos(GridToWorld(gridPos), GravityField.SearchFilter.Static);

        return gridStaticField[gridPos];
    }


    static public (Vector3 dir, float dist, GravityField field) AtPos(Vector3 pos)
    {
        if (!inst.useGrid)
            return GravityField.AtPos(pos, GravityField.SearchFilter.Both);

        Vector3 dirToStaticSrf;
        float distToStaticSrf;
        GravityField fieldStaticSrf;
        (dirToStaticSrf, distToStaticSrf, fieldStaticSrf) = inst.Grid(pos);

        Vector3 dirToDynamicSrf;
        float distToDynamicSrf;
        GravityField fieldDynamicSrf;
        (dirToDynamicSrf, distToDynamicSrf, fieldDynamicSrf) = GravityField.AtPos(pos, GravityField.SearchFilter.Dynamic);

        return distToStaticSrf < distToDynamicSrf ? (dirToStaticSrf, distToStaticSrf, fieldStaticSrf) : (dirToDynamicSrf, distToDynamicSrf, fieldDynamicSrf);
    }

    static public Vector3 AcceAtPos(Vector3 pos) => AtPos(pos).dir * inst.acceleration;
}
