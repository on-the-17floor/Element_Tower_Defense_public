using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public class TowerSpecialData : ILoadableFromSnapshot
{
    public TowerTier tierType;
    public ElementType elementType;
    public float range;
    public float rate;

    public TowerSpecialData()
    {
        tierType = (TowerTier)0;
        elementType = (ElementType)0;
        range = 0f;
        rate = 0f;
    }
    public void LoadFromSnapshot(DataSnapshot snap)
    {
        tierType = Extension.StringToEnum<TowerTier>(snap.Child("tierType").Value.ToString());
        elementType = Extension.StringToEnum<ElementType>(snap.Child("elementType").Value.ToString());

        range = float.Parse(snap.Child("range").Value.ToString());
        rate = float.Parse(snap.Child("rate").Value.ToString());
    }
}
