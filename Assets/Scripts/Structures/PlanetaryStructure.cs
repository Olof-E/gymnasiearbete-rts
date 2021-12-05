using System;
/*
Planetary structures:
- Fusion reactor
- Tritanium extractors
- Crystal synthesizers
- rare metals extractors
- Carbon extractors
- Research facility
- Agricultural hub

Space structures:
- Large shipyard
- Small Shipyard
- Spaceport
- Railgun cannon
- Torpedo/Missile laucnher
- Phased energy beam
- Deep space antenna array 
- 

*/
public class PlanetaryStructure
{
    public int playerId { get; set; }
    public int level { get; set; } = 0;
    public int maxLevel { get; set; } = 1;
    public Planet parentBody { get; set; }
    public ResourceConsumtion constructionCost { get; set; }
    public virtual void Execute()
    {

    }
    public virtual void LevelUp()
    {
        if (level < maxLevel)
        {
            level++;
        }
    }
}
