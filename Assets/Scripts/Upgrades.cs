using UnityEngine;

public class Upgrades : MonoBehaviour
{
    public static int freeAugmentSlots = 3;

    public static int minedDustTiles = 0; // spawns at the surface
    public static int minedStoneTiles = 0; //spawns all over
    public static int minedClayTiles = 0; //spawns in spread out clumps a bit deeper down
    public static int minedSiltTiles = 0; //spawns in flat deposits
    public static int minedBasaltTiles = 0; //spawns in thin veins
    public static int minedQuartzTiles = 0; //spawns in pillars
    public static int minedGraniteTiles = 0; //sprinkled in deeper layers

    public static int unlockLevelFuelTankLimit = 0; //dust
    public static int unlockStraightShooterLimit = 0; //stone
    public static int unlockVeinCrackerLimit = 0; //basalt
    public static int unlockDecisiveHitLimit = 0; //quartz
    public static int unlockAquiredGritLimit = 0; //silt
    public static int unlockGearboxOverdriveLimit = 0; //clay
    public static int unlockExtraAugmentSlotLimit = 0; //granite

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

    //data for specific augment functions --
    private int quartzMinedRecently = 0;
    private float quartzMinedTIme = 0;


    public static Upgrades Instance { get; private set; }
    private void Awake()
    {
        Instance = this;

        if (unlockAquiredGritLimit == 0)
        {
            unlockLevelFuelTankLimit = Random.Range(30, 50); //dust
            unlockStraightShooterLimit = Random.Range(40, 60); //stone
            unlockVeinCrackerLimit = Random.Range(10, 20); //basalt
            unlockDecisiveHitLimit = Random.Range(10, 20); //quartz
            unlockAquiredGritLimit = Random.Range(50, 70); //silt
            unlockGearboxOverdriveLimit = Random.Range(30, 50); //clay
            unlockExtraAugmentSlotLimit = Random.Range(30, 50); //granite
        }
    }

    public void OnDustMined()
    {
        minedDustTiles++;
    }

    public void OnStoneMined()
    {
        minedStoneTiles++;
    }
    public void OnSiltMined()
    {
        minedSiltTiles++;
        if (isAquiredGrit)
        {
            //apply aquired grit augmant actions
        }
    }
    public void OnQuartzMined(Vector2Int pos)
    {
        minedQuartzTiles++;
        if (isDecisiveHit)
        {
            //apply decisive hit augment actions
            if (Time.time - quartzMinedTIme > 10) quartzMinedRecently = 0;
            quartzMinedRecently++;

            if (quartzMinedRecently >= 10)
            {
                TileManager.instance.MineTiles(TileManager.instance.GetClumpedTiles(pos, 3));
                quartzMinedRecently = 0;
            }
        }
    }

    public void OnBasaltMined(Vector2Int pos)
    {
        minedBasaltTiles++;
        if (isVeinCracker)
        {
            //apply vein cracker augment actions
            TileManager.instance.MineTiles(TileManager.instance.NeighboringTiles(pos));
        }
    }

    public void OnGraniteMined()
    {
        minedGraniteTiles++;
    }

    public void OnClayMined()
    {
        minedClayTiles++;
    }

    public void DisableAllAugments()
    {
        isAquiredGrit = false;
        isDecisiveHit = false;
        isExtraAugmentSlot = false;
        isGearboxOverdrive = false;
        isLevelFuelTank = false;
        isStraightShooter = false;
        isVeinCracker = false;
    }
}
