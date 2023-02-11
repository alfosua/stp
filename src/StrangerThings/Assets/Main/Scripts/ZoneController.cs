using System.Collections.Generic;
using UnityEngine;

public class ZoneController : MonoBehaviour
{
    [SerializeField]
    private Player player;

    [SerializeField]
    private ZoneSet[] zoneSets;

    private ZoneSet? activeZoneSet;

    private ZoneSet? nextZoneSet;

    public void SwitchZoneSet()
    {
        if (activeZoneSet is not null)
        {
            activeZoneSet.Unactivate();
        }

        nextZoneSet.Activate();
        activeZoneSet = nextZoneSet;

        player.SwitchZoneSet(nextZoneSet);

        nextZoneSet = null;
    }

    public void PrepareNextZoneSet(ZoneSet zoneSet)
    {
        nextZoneSet = zoneSet;

        if (activeZoneSet is null)
        {
            SwitchZoneSet();
        }
    }

    private void Reset()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        var foundZoneSets = new List<ZoneSet>();
        foreach (Transform child in transform)
        {
            foundZoneSets.Add(child.GetComponent<ZoneSet>());
        }
        zoneSets = foundZoneSets.ToArray();
    }
}
