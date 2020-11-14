using System.Collections.Generic;

public class WinCondition : Card
{
    public int frog;
    public int dragon;
    public int gator;
    public int axolotl;
    public int lizard;
    public int dino;
    public int box;
    public int cat;

    public Dictionary<CreatureType, int> requirements;

    void Start()
    {
        requirements = new Dictionary<CreatureType, int>() {
            { CreatureType.Frog, frog },
            { CreatureType.Dragon, dragon },
            { CreatureType.Gator, gator },
            { CreatureType.Axolotl, axolotl },
            { CreatureType.Lizard, lizard },
            { CreatureType.Dino, dino },
            { CreatureType.Box, box },
            { CreatureType.Cat, cat }
        };
    }

}
