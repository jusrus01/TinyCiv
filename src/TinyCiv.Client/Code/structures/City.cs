using TinyCiv.Shared.Game;
using System.Collections.Generic;

namespace TinyCiv.Client.Code.Structures;

public class City : Structure
{
    public List<GameObject> HighlightedArea = new List<GameObject>();

    public City(GameObject go) : base(go) { }

    public void RemoveHighlight()
    {
        HighlightedArea.ForEach(x => x.RemoveEffects());
    }
}
