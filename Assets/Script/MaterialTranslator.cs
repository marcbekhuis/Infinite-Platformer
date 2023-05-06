using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaterialTranslator : Singleton<MaterialTranslator>
{
    [SerializeField] private List<Material> Subscriptions;
    
    public void SetOffset(Vector3 offset)
    {
        Subscriptions.ForEach(x => x.SetVector("_offset", offset));
    }
}
