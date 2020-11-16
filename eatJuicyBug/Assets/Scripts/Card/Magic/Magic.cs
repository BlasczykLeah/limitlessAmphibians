public abstract class Magic : Card
{
    public MagicTarget target;
    public CreatureType creatureType;
    
    //does magic stuffs
    public virtual void DoMagic(int playerIndex, int targetIndex, CreatureType creatureType) {

    }

}
