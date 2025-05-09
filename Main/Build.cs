using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamInfrastructure
{
    public IFacility Arena { get; private set; }
    public IFacility MerchandiseStore { get; private set; }
    public IFacility PlayerAcademy { get; private set; }
    public AffiliatedTeamsHandler Affiliates { get; private set; }

    public TeamInfrastructure(List<string> nameOptions, bool loadFromState = false)
    {
        if (!loadFromState)
        {
            Arena = new ArenaFacility();
            MerchandiseStore = new StoreFacility();
            PlayerAcademy = new AcademyFacility();
            Affiliates = new AffiliatedTeamsHandler(nameOptions);
        }
    }
}



public abstract class FacilityCore : IFacility
{
    public string FacilityName { get; protected set; }
    public int FacilityTier { get; protected set; } = 1;
    protected int TierCap = 10;
    protected int TierFloor = 1;

    public virtual void Promote()
    {
        if (FacilityTier >= TierCap)
            throw new Exception($"{FacilityName} reached the tier limit.");
        FacilityTier++;
    }

    public virtual void Demote()
    {
        if (FacilityTier <= TierFloor)
            throw new Exception($"{FacilityName} is already at the lowest tier.");
        FacilityTier--;
    }
}

public class ArenaFacility : FacilityCore
{
    public int SeatCount => 1000 + (FacilityTier - 1) * 1000;
    public int PricePerTicket => 10 + FacilityTier;

    public ArenaFacility()
    {
        FacilityName = "Arena";
        TierCap = 12;
    }
}

public class StoreFacility : FacilityCore
{
    public int RevenueEstimate => 10000 + (FacilityTier - 1) * 5000;

    public StoreFacility()
    {
        FacilityName = "Fan Store";
        TierCap = 20;
    }
}

public class AcademyFacility : FacilityCore
{
    public string TrainingRank => FacilityTier switch
    {
        <= 5 => "Entry",
        <= 10 => "Standard",
        _ => "Elite"
    };

    public AcademyFacility()
    {
        FacilityName = "Academy";
        TierCap = 15;
    }
}

public class AffiliatedTeamsHandler
{
    public List<AffiliateTeam> ActiveAffiliates { get; private set; } = new();
    private readonly List<string> availableNames;
    private int affiliateCap = 5;

    public AffiliatedTeamsHandler(List<string> availableNames)
    {
        this.availableNames = availableNames;
    }

    public void AddAffiliate()
    {
        if (ActiveAffiliates.Count >= affiliateCap)
            throw new Exception("Affiliate capacity reached.");

        string candidateName;
        do
        {
            candidateName = availableNames[UnityEngine.Random.Range(0, availableNames.Count)];
        } while (ActiveAffiliates.Exists(t => t.TeamLabel == candidateName));

        ActiveAffiliates.Add(new AffiliateTeam(candidateName));
    }

    public void RemoveAffiliate(string label)
    {
        var team = ActiveAffiliates.Find(t => t.TeamLabel == label);
        if (team != null)
            ActiveAffiliates.Remove(team);
    }

    public class AffiliateTeam
    {
        public string TeamLabel { get; }
        public List<string> Roster { get; } = new();

        public AffiliateTeam(string name)
        {
            TeamLabel = name;
        }

        public void RegisterPlayer(string playerName) => Roster.Add(playerName);
        public void UnregisterPlayer(string playerName) => Roster.Remove(playerName);
    }
}
