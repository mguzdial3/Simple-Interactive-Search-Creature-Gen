using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limb
{
    public Vector3 scale, offset, force, rotation;
    public float increment;
    public Limb parent;
    public LimbObject obj;
    public List<Limb> children;

    public Limb(Vector3 _scale, Vector3 _offset, Vector3 _force, Vector3 _rotation, float _increment, Limb _parent){
        this.scale = _scale;
        this.offset = _offset;
        this.force = _force;
        this.rotation = _rotation;
        this.increment = _increment;
        this.parent = _parent;
        children = new List<Limb>();
    }

    public void AddChild(Limb _child){
        children.Add(_child);
    }
}
