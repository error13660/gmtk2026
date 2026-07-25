using UnityEngine;

public class Upgrades
{
    public static int freeAugmentSlots = 3;

    public static int minedDustTiles = 0; // spawns at the surface
    public static int minedStoneTiles = 0; //spawns all over
    public static int minedClayTiles = 0; //spawns in spread out clumps a bit deeper down
    public static int minedSiltTiles = 0; //spawns in flat deposits
    public static int minedBasaltTiles = 0; //spawns in thin veins
    public static int minedQuartzTiles = 0; //spawns in pillars
    public static int minedGraniteTiles = 0; //sprinkled in deeper layers

    public static int unlockLevelFuelTankLimit = Random.Range(30, 50); //dust
    public static int unlockStraightShooterLimit = Random.Range(40, 60); //stone
    public static int unlockVeinCrackerLimit = Random.Range(10, 20); //basalt
    public static int unlockDecisiveHitLimit = Random.Range(10, 20); //quartz
    public static int unlockAquiredGritLimit = Random.Range(50,70); //silt
    public static int unlockGearboxOverdriveLimit = Random.Range(30, 50); //clay
    public static int unlockExtraAugmentSlotLimit= Random.Range(30, 50); //granite

    /// <summary>
    /// Moving relatively level slightly increases your mining power
    /// </summary>
    public static bool isLevelFuelTank;
    /// <summary>
    /// Moving perfectly downwards slightly increases your mining power
    /// </summary>
    public static bool isStraightShooter;
    /// <summary>
    /// When mining (spagetti veins) veins the surrounging rock also gets mined
    /// </summary>
    public static bool isVeinCracker;
    /// <summary>
    /// Mining 10 tiles of (pillar deposits) the whole pillar cracks and gets mined
    /// </summary>
    public static bool isDecisiveHit;
    /// <summary>
    /// Mining silt increases your mining power for a limited time
    /// </summary>
    public static bool isAquiredGrit;
    /// <summary>
    /// Moving fast allows you to accelerate and increases your mining power until you slow back down
    /// </summary>
    public static bool isGearboxOverdrive;
    /// <summary>
    /// Equipping this augment gives 2 augment slots (1 extra)
    /// </summary>
    public static bool isExtraAugmentSlot;
}
