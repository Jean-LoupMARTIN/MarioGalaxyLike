using System.Collections.Generic;
using UnityEngine;



public abstract class GravityField : MonoBehaviour
{
    static HashSet<GravityField> set = new HashSet<GravityField>();
    static HashSet<GravityField> setStatic = new HashSet<GravityField>();
    static HashSet<GravityField> setDynamic = new HashSet<GravityField>();

    protected abstract (Vector3 dir, float dist) AtPos(Vector3 pos);

    public bool IsStatic => gameObject.isStatic;

    public enum SearchFilter
    {
        Static,
        Dynamic,
        Both
    }

    void OnEnable()
    {
        set.Add(this);
        (IsStatic ? setStatic : setDynamic).Add(this);
    }

    void OnDisable()
    {
        set.Remove(this);
        (IsStatic ? setStatic : setDynamic).Remove(this);
    }




    public static (Vector3 dir, float dist, GravityField field) AtPos(Vector3 pos, SearchFilter filter)
    {
        Vector3 dir = Vector3.zero, dirCrt;
        float dist = float.MaxValue, distCrt;
        GravityField field = null;

        HashSet<GravityField> setPick;

        if      (filter == SearchFilter.Dynamic) setPick = setDynamic;
        else if (filter == SearchFilter.Static)  setPick = setStatic;
        else                                     setPick = set;

        foreach (GravityField crtField in setPick)
        {
            (dirCrt, distCrt) = crtField.AtPos(pos);

            if (distCrt < dist)
            {
                dir = dirCrt;
                dist = distCrt;
                field = crtField;
            }
        }

        return (dir, dist, field);
    }
}
