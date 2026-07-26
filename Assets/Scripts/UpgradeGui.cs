using TMPro;
using UnityEngine;

public class UpgradeGui : MonoBehaviour
{
    private enum AugmentType
    {
        LEVELFUELTANK,
        STRAIGHTSHOOTER,
        VEINCRACKER,
        DECISIVEHIT,
        AQUIREDGRIT,
        GEARBOXOVERDRIVE,
        EXTRAAUGMENTSLOT
    }

    [SerializeField] private AugmentType augmentType;
    [SerializeField] private TextMeshProUGUI descriptionDisplay;
    [SerializeField] private TextMeshProUGUI remainingSlotsDisplay;
    private Vector3 basePosition;

    private void Awake()
    {
        basePosition = transform.position;
        Upgrades.Instance.DisableAllAugments();

        switch (augmentType)
        {
            case AugmentType.LEVELFUELTANK:
                if (Upgrades.minedDustTiles > Upgrades.unlockLevelFuelTankLimit) gameObject.SetActive(true);
                else gameObject.SetActive(false);
                break;
            case AugmentType.STRAIGHTSHOOTER:
                if (Upgrades.minedStoneTiles > Upgrades.unlockStraightShooterLimit) gameObject.SetActive(true);
                else gameObject.SetActive(false);
                break;
            case AugmentType.VEINCRACKER:
                if (Upgrades.minedBasaltTiles > Upgrades.unlockVeinCrackerLimit) gameObject.SetActive(true);
                else gameObject.SetActive(false);
                break;
            case AugmentType.DECISIVEHIT:
                if (Upgrades.minedQuartzTiles > Upgrades.unlockDecisiveHitLimit) gameObject.SetActive(true);
                else gameObject.SetActive(false);
                break;
            case AugmentType.AQUIREDGRIT:
                if (Upgrades.minedSiltTiles > Upgrades.unlockAquiredGritLimit) gameObject.SetActive(true);
                else gameObject.SetActive(false);
                break;
            case AugmentType.GEARBOXOVERDRIVE:
                if (Upgrades.minedClayTiles > Upgrades.unlockGearboxOverdriveLimit) gameObject.SetActive(true);
                else gameObject.SetActive(false);
                break;
            case AugmentType.EXTRAAUGMENTSLOT:
                if (Upgrades.minedGraniteTiles > Upgrades.unlockExtraAugmentSlotLimit) gameObject.SetActive(true);
                else gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void OnHover()
    {
        switch (augmentType)
        {
            case AugmentType.LEVELFUELTANK:
                descriptionDisplay.SetText("Level fuel tank: Keeping your vehicle relatively level slightly increases your mining and movement speed.");
                break;
            case AugmentType.STRAIGHTSHOOTER:
                descriptionDisplay.SetText("Straight shooter: Drilling straight down slightly increases your mining and movement speed.");
                break;
            case AugmentType.VEINCRACKER:
                descriptionDisplay.SetText("Vein cracker: Basalt veins become significantly easier to mine. Mining basalt also cracks and destroys surrounding rock.");
                break;
            case AugmentType.DECISIVEHIT:
                descriptionDisplay.SetText("Decisive hit: Quickly mining a section of quartz pillars cracks and destroys the whole deposit instantly.");
                break;
            case AugmentType.AQUIREDGRIT:
                descriptionDisplay.SetText("Aquired grit: Mining silt coats your drill in abresive grit, greatly increasing your mining and movement speed until the grit wears off.");
                break;
            case AugmentType.GEARBOXOVERDRIVE:
                descriptionDisplay.SetText("Gearbox overdrive: Moving fast allows you to accelerate further.");
                break;
            case AugmentType.EXTRAAUGMENTSLOT:
                descriptionDisplay.SetText("Extra augment slot: Unlock an extra augment slot.");
                break;
            default:
                break;
        }
    }

    public void OnClick()
    {
        switch (augmentType)
        {
            case AugmentType.LEVELFUELTANK:
                if (!Upgrades.isLevelFuelTank && Upgrades.freeAugmentSlots > 0)
                {
                    Upgrades.isLevelFuelTank = true;
                    Upgrades.freeAugmentSlots--;
                }
                else
                {
                    Upgrades.isLevelFuelTank = false;
                    Upgrades.freeAugmentSlots++;
                }
                break;
            case AugmentType.STRAIGHTSHOOTER:
                if (!Upgrades.isStraightShooter && Upgrades.freeAugmentSlots > 0)
                {
                    Upgrades.isStraightShooter = true;
                    Upgrades.freeAugmentSlots--;
                }
                else
                {
                    Upgrades.isStraightShooter = false;
                    Upgrades.freeAugmentSlots++;
                }
                break;
            case AugmentType.VEINCRACKER:
                if (!Upgrades.isVeinCracker && Upgrades.freeAugmentSlots > 0)
                {
                    Upgrades.isVeinCracker = true;
                    Upgrades.freeAugmentSlots--;
                }
                else
                {
                    Upgrades.isVeinCracker = false;
                    Upgrades.freeAugmentSlots++;
                }
                break;
            case AugmentType.DECISIVEHIT:
                if (!Upgrades.isDecisiveHit && Upgrades.freeAugmentSlots > 0)
                {
                    Upgrades.isDecisiveHit = true;
                    Upgrades.freeAugmentSlots--;
                }
                else
                {
                    Upgrades.isDecisiveHit = false;
                    Upgrades.freeAugmentSlots++;
                }
                break;
            case AugmentType.AQUIREDGRIT:
                if (!Upgrades.isAquiredGrit && Upgrades.freeAugmentSlots > 0)
                {
                    Upgrades.isAquiredGrit = true;
                    Upgrades.freeAugmentSlots--;
                }
                else
                {
                    Upgrades.isAquiredGrit = false;
                    Upgrades.freeAugmentSlots++;
                }
                break;
            case AugmentType.GEARBOXOVERDRIVE:
                if (!Upgrades.isGearboxOverdrive && Upgrades.freeAugmentSlots > 0)
                {
                    Upgrades.isGearboxOverdrive = true;
                    Upgrades.freeAugmentSlots--;
                }
                else
                {
                    Upgrades.isGearboxOverdrive = false;
                    Upgrades.freeAugmentSlots++;
                }
                break;
            case AugmentType.EXTRAAUGMENTSLOT:
                if (!Upgrades.isExtraAugmentSlot && Upgrades.freeAugmentSlots > 0)
                {
                    Upgrades.isExtraAugmentSlot = true;
                    Upgrades.freeAugmentSlots = 4;
                }
                else
                {
                    Upgrades.isExtraAugmentSlot = false;
                    Upgrades.freeAugmentSlots = 3;
                    Upgrades.Instance.DisableAllAugments();
                }
                break;
            default:
                break;
        }
        remainingSlotsDisplay.SetText("Augment slots remaining:\n" + Upgrades.freeAugmentSlots);
    }

    private void Update()
    {
        switch (augmentType)
        {
            case AugmentType.LEVELFUELTANK:
                if (Upgrades.isLevelFuelTank) transform.position = basePosition + Vector3.up + (Vector3.up * Mathf.Sin(Time.time * 10) * .25f);
                else transform.position = basePosition;
                break;
            case AugmentType.STRAIGHTSHOOTER:
                if (Upgrades.isStraightShooter) transform.position = basePosition + Vector3.up + (Vector3.up * Mathf.Sin(Time.time * 10) * .25f);
                else transform.position = basePosition;
                break;
            case AugmentType.VEINCRACKER:
                if (Upgrades.isVeinCracker) transform.position = basePosition + Vector3.up + (Vector3.up * Mathf.Sin(Time.time * 10) * .25f);
                else transform.position = basePosition;
                break;
            case AugmentType.DECISIVEHIT:
                if (Upgrades.isDecisiveHit) transform.position = basePosition + Vector3.up + (Vector3.up * Mathf.Sin(Time.time * 10) * .25f);
                else transform.position = basePosition;
                break;
            case AugmentType.AQUIREDGRIT:
                if (Upgrades.isAquiredGrit) transform.position = basePosition + Vector3.up + (Vector3.up * Mathf.Sin(Time.time * 10) * .25f);
                else transform.position = basePosition;
                break;
            case AugmentType.GEARBOXOVERDRIVE:
                if (Upgrades.isGearboxOverdrive) transform.position = basePosition + Vector3.up + (Vector3.up * Mathf.Sin(Time.time * 10) * .25f);
                else transform.position = basePosition;
                break;
            case AugmentType.EXTRAAUGMENTSLOT:
                if (Upgrades.isExtraAugmentSlot) transform.position = basePosition + Vector3.up;
                else transform.position = basePosition;
                break;
            default:
                break;
        }
    }
}
