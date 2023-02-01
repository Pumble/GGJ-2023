using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Transition
{
    public enum Type
    {
        None,                   //0
        Building,               //1
        Resource,               //2
        Dump,                   //3
        ShipResource,           //4
        AttractAsteroids,       //5
        ResetValue,             //6 //Asteroids, Drones, Freighters, Shipment(CH4,H2O,H2SO4,Ore,OreR)
        Time,                   //7
        DestroyThreat,          //8    
        StartDroneAttack,       //9
        CreatePirateFreighter,  //10
        CreatePirateCorvette,   //11
        FireOrbitalLaser,       //12
        CreateMeteor,           //13
        CreateMeteorFactory,    //14
        StartMissileAttack,     //15
        StoreMaterial,          //16
        

        Defeat = -1,            //-1
        Sandbox = 99            //99
    }

    public Type type;
    public string description = "";
    public bool status = false;
    public bool persistent = false;
    public abstract bool Check();
    public abstract string ToJson();

    public virtual string GetDescription()
    {
        return GetLocalizedDescription();
    }

    public Transition(Type type, string description = "", bool persistent = false)
    {
        this.type = type;
        this.description = description;
        this.persistent = persistent;
    }

    public static Transition FromJson(string json)
    {
        None temp = JsonUtility.FromJson<None>(json);
        Transition t;
        switch(temp.type)
        {
            case Type.Building:
                t = JsonUtility.FromJson<Building>(json);
                break;
            case Type.Resource:
                t = JsonUtility.FromJson<Resource>(json);
                break;
            case Type.Dump:
                t = JsonUtility.FromJson<Dump>(json);
                break;
            case Type.ShipResource:
                t = JsonUtility.FromJson<ShipResource>(json);
                break;
            case Type.AttractAsteroids:
                t = JsonUtility.FromJson<AttractAsteroids>(json);
                break;
            case Type.ResetValue:
                t = JsonUtility.FromJson<ResetValue>(json);
                break;
            case Type.Time:
                t = JsonUtility.FromJson<Time>(json);
                break;
            case Type.DestroyThreat:
                t = JsonUtility.FromJson<DestroyThreat>(json);
                break;
            case Type.StartDroneAttack:
                t = JsonUtility.FromJson<StartDroneAttack>(json);
                break;
            case Type.CreatePirateFreighter:
                t = JsonUtility.FromJson<CreatePirateFreighter>(json);
                break;
            case Type.CreatePirateCorvette:
                t = JsonUtility.FromJson<CreatePirateCorvette>(json);
                break;
            case Type.FireOrbitalLaser:
                t = JsonUtility.FromJson<FireOrbitalLaser>(json);
                break;
            case Type.CreateMeteor:
                t = JsonUtility.FromJson<CreateMeteor>(json);
                break;
            case Type.CreateMeteorFactory:
                t = JsonUtility.FromJson<CreateMeteorFactory>(json);
                break;
            case Type.StartMissileAttack:
                t = JsonUtility.FromJson<StartMissileAttack>(json);
                break;
            case Type.StoreMaterial:
                t = JsonUtility.FromJson<StoreMaterial>(json);
                break;

            case Type.Defeat:
                t = JsonUtility.FromJson<Defeat>(json);
                break;
            case Type.Sandbox:
                t = JsonUtility.FromJson<Sandbox>(json);
                break;
            default:
                t = temp;
                break;
        }
        return t;
    }

    public string GetLocalizedDescription()
    {
        string localizedDescription = Lean.Localization.LeanLocalization.GetTranslationText(description);
        if(localizedDescription == null)
        {
            localizedDescription = description;
        }
        else
        {
            localizedDescription = localizedDescription.Replace('+', '\n');
        }
        return localizedDescription;
    }

    /// <summary>
    /// Transition None: Type 0
    /// Check always returns true
    /// </summary>
    [Serializable]
    public class None : Transition
    {
        public None() : base(Transition.Type.None)
        { }

        public override bool Check()
        {
            return true;
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }

    /// <summary>
    /// Transition Building: Type 1
    /// Check returns true if there is a determined amount of buildings active in the system
    /// </summary>
    [Serializable]
    public class Building : Transition
    {
        public BuildingType buildingType;
        public int quantity;
        private int lastCount;

        public Building(string description, bool persistent, BuildingType type, int quantity) : base(Transition.Type.Building, description, persistent)
        {
            this.buildingType = type;
            this.quantity = quantity;
        }

        public override bool Check()
        {
            if (persistent && status)
            {
                return true;
            }
            else
            {
                lastCount = global::Building.GetBuildingsOfType(buildingType).Count;
                status = lastCount >= quantity;
                return status;
            }
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

        public override string GetDescription()
        {
            string text = GetLocalizedDescription();
            string outOf = Lean.Localization.LeanLocalization.GetTranslationText("out of");
            text += "<size=16>";
            text += "\n";

            if (status) text += "<color=white>";
            else text += "<color=red>";
            text += lastCount;
            text += "</color>";

            text += $" {outOf} " + quantity;

            text += "</size>";
            return text;
        }
    }

    /// <summary>
    /// Transition Resource: Type 2
    /// Check returns true if value is between two thresholds
    /// </summary>
    [Serializable]
    public class Resource : Transition
    {
        public Parameter resourceType;
        public double topValue = double.PositiveInfinity;
        public double bottomValue = double.NegativeInfinity;
        private double value;

        public Resource(string description, bool persistent, Parameter resourceType, double topValue = double.PositiveInfinity, double bottomValue = double.NegativeInfinity) :
            base(Transition.Type.Resource, description, persistent)
        {
            this.resourceType = resourceType;
            this.topValue = topValue;
            this.bottomValue = bottomValue;
        }

        public override bool Check()
        {
            if (persistent && status)
            {
                return true;
            }
            else
            {
                value = Simulation.GetSimulationValue(resourceType);
                status = bottomValue <= value && value <= topValue;
                return status;
            }
        }

        public override string GetDescription()
        {
            string text = GetLocalizedDescription();
            
            text += "<size=16>";
            text += "\n";

            if (!double.IsNegativeInfinity(bottomValue))
            {
                text += "<color=white>";
                text += bottomValue.ToString("0");
                text += " < ";
                text += "</color>";
            }

            if (status) text += "<color=white>";
            else text += "<color=red>";
            text += value.ToString("0");
            text += "</color>";

            if (!double.IsPositiveInfinity(topValue))
            {
                text += "<color=white>";
                text += " < ";
                text += topValue.ToString("0");
                text += "</color>";
            }

            text += "</size>";
            return text;
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }

    /// <summary>
    /// Transition Dump: Type 3
    /// Adds a specific amount of a given resource into the system
    /// Fires only once
    /// </summary>
    [Serializable]
    public class Dump : Transition
    {
        public Parameter resource;
        public int amount;

        /// <summary>
        /// Transition.Dump is an event, it has no description and is always persistent
        /// Receives the resource to dump into the system and the amount to add
        /// </summary>
        /// <param name="resource">the resource that should be added into the system</param>
        /// <param name="amount">the amount of the given resource to add into the system</param>
        public Dump(Parameter resource, int amount) : base(Transition.Type.Dump, "", true)
        {
            this.resource = resource;
            this.amount = amount;
        }

        /// <summary>
        /// Add the resource into the system and then check true
        /// </summary>
        /// <returns></returns>
        public override bool Check()
        {
            if(!status)
            {
                status = true;
                Simulation.IncreaseSimulationParameter(resource.ToString(), amount, false);
            }
            return true;
        }

        /// <summary>
        /// This event should not appear in the objectives list
        /// An event that prints no description automatically gets excluded from the objectives list
        /// </summary>
        /// <returns></returns>
        public override string GetDescription()
        {
            return "";
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }

    /// <summary>
    /// Transition Ship Resource: Type 4
    /// The player has to send a specific amount of a given resource to space
    /// Containers ship resources to space
    /// Shipped resources can be: H2O, CH4, H2SO4, OreRaw, OreRefined
    /// </summary>
    [Serializable]
    public class ShipResource : Transition
    {
        public Parameter resourceType;
        public int amount;

        public ShipResource(string description, bool persistent, Parameter resource, int amount) :
            base(Transition.Type.ShipResource, description, persistent)
        {
            this.resourceType = resource;
            this.amount = amount;
        }

        public override bool Check()
        {
            if(persistent && status)
            {
                return true;
            }
            else
            {
                double shippedResource = Simulation.GetSimulationShippedValue(resourceType.ToString());
                shippedResource = Math.Round(shippedResource);
                status = shippedResource >= amount;
                return status;
            }
        }

        public override string GetDescription()
        {
            string outOf = Lean.Localization.LeanLocalization.GetTranslationText("shipped out of");
            string text = GetLocalizedDescription();

            text += "<size=16>";
            text += "\n";

            double value = Simulation.GetSimulationShippedValue(resourceType.ToString());

            if (status) text += "<color=white>";
            else text += "<color=red>";
            text += value.ToString("0");
            text += "</color>";

            text += $" {outOf} ";
            text += "<color=white>";
            text += amount.ToString();
            text += "</color>";
            text += "</size>";
            return text;
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }

    /// <summary>
    /// Transition AttractAsteroids: Type 5
    /// The player has to attract a certain number of asteroids using the AsteroidAttractor building
    /// </summary>
    [Serializable]
    public class AttractAsteroids : Transition
    {
        public int amount;

        public AttractAsteroids(string description, bool persistent, int amount) :
            base(Transition.Type.AttractAsteroids, description, persistent)
        {
            this.amount = amount;
        }

        public override bool Check()
        {
            if(persistent && status)
            {
                return true;
            }
            else
            {
                status = Simulation.GetAsteroids() >= amount;
                return status;
            }
        }

        public override string GetDescription()
        {
            string outOf = Lean.Localization.LeanLocalization.GetTranslationText("out of");
            string text = GetLocalizedDescription();

            text += "<size=16>";
            text += "\n";

            if (status) text += "<color=white>";
            else text += "<color=red>";
            text += Simulation.GetAsteroids().ToString();
            text += "</color>";

            text += $" {outOf} " + amount;

            text += "</size>";

            return text;
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }

    /// <summary>
    /// Transition ResetValue: Type 6
    /// Resets the value of a specific variable
    /// Fires only once
    /// </summary>
    [Serializable]
    public class ResetValue : Transition
    {
        //Asteroids, Drones, Freighters, Shipment(CH4,H2O,H2SO4,Ore,OreR)
        public enum Value
        {
            ShippedCH4,
            ShippedH2O,
            ShippedH2SO4,
            ShippedOre,
            ShippedOreRefined,
            Asteroids,
            Meteorites,
            MeteoriteHits,
            Drones,
            Freighters,
            Corvettes
        }

        public Value valueType;
        
        public ResetValue(Value valueType) : base(Type.ResetValue)
        {
            this.valueType = valueType;
        }

        public override bool Check()
        {
            // If this is a load, ignore resets
            if (Simulation.quickLoad)
                return true;

            if(!status)
            {
                status = true;
                switch(valueType)
                {
                    case Value.ShippedCH4:
                        Simulation.SetShippedParameter("CH4", 0);
                        break;
                    case Value.ShippedH2O:
                        Simulation.SetShippedParameter("H2O", 0);
                        break;
                    case Value.ShippedH2SO4:
                        Simulation.SetShippedParameter("H2SO4", 0);
                        break;
                    case Value.ShippedOre:
                        Simulation.SetShippedParameter("OreRaw", 0);
                        break;
                    case Value.ShippedOreRefined:
                        Simulation.SetShippedParameter("OreRefined", 0);
                        break;
                    case Value.Asteroids:
                        Simulation.ResetAsteroids();
                        break;
                    case Value.Meteorites:
                        Simulation.ResetMeteorites();
                        break;
                    case Value.MeteoriteHits:
                        Simulation.ResetMeteoriteHits();
                        break;
                    case Value.Drones:
                        Simulation.ResetDrones();
                        break;
                    case Value.Freighters:
                        Simulation.ResetPirateFreighters();
                        break;
                    case Value.Corvettes:
                        Simulation.ResetPirateCorvettes();
                        break;
                }
            }

            return true;
        }

        public override string GetDescription()
        {
            return "";
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }

    /// <summary>
    /// Transition Time: Type 7
    /// Fires when a certain amount of time has passed in the in-game clock
    /// Works as a "survive X amount of minutes"
    /// </summary>
    [Serializable]
    public class Time : Transition
    {
        public int timeInSeconds;
        private float endTime = 0;
        private float startTime = 0;
        private bool timeIsSet = false;

        public Time(string description, bool persistent, int timeInSeconds) : base(Type.Time, description, persistent)
        {
            this.timeInSeconds = timeInSeconds;
        }

        public override bool Check()
        {
            if(persistent && status)
            {
                return true;
            }
            else if(timeIsSet)
            {
                status = TimeControl.GetInGameTime() >= endTime;
                return status;
            }
            else
            {
                timeIsSet = true;
                startTime = TimeControl.GetInGameTime();
                endTime = startTime + timeInSeconds;
                return false;
            }
        }

        public override string GetDescription()
        {
            string outOf = Lean.Localization.LeanLocalization.GetTranslationText("passed out of");
            string text = GetLocalizedDescription();
            text += "<size=16>\n";

            float timePassed = TimeControl.GetInGameTime() - startTime;

            if (status) text += "<color=white>";
            else text += "<color=red>";
            text += formatTime(timePassed) + $"</color> {outOf} <color=white>" + formatTime(timeInSeconds) + "</color>";

            text += "</size>";
            return text;
        }

        /// <summary>
        /// Transforms a float value given in seconds to the MM:SS format
        /// </summary>
        /// <param name="time">the time expressed in seconds</param>
        /// <returns></returns>
        private string formatTime(float time)
        {
            // first, drop decimals
            int timeI = (int)time;
            int seconds = timeI % 60;

            timeI = timeI / 60;
            int minutes = timeI % 60;

            timeI = timeI / 60;
            int hours = timeI;

            string text = "";
            if (hours > 0) text += hours + ":";

            text += minutes.ToString("00") + ":";
            text += seconds.ToString("00");

            return text;
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }

    /// <summary>
    /// Transition Destroy Threat: Type 8
    /// The player has to destroy a certain amount of a given threat type
    /// Threat types are:
    ///     Meteorite
    ///     Drone
    ///     Pirate Freighter
    ///     Pirate Corvette
    /// </summary>
    [Serializable]
    public class DestroyThreat : Transition
    {
        public ThreatType threatType;
        public int quantity;
        private int currentQuantity;

        public DestroyThreat(string description, bool persistent, ThreatType threatType, int quantity):
            base(Type.DestroyThreat, description, persistent)
        {
            this.threatType = threatType;
            this.quantity = quantity;
        }

        public override bool Check()
        {
            if(persistent && status)
            {
                return true;
            }
            else
            {
                switch(threatType)
                {
                    case ThreatType.Meteorite:
                        currentQuantity = Simulation.GetMeteorites();
                        break;
                    case ThreatType.Drone:
                        currentQuantity = Simulation.GetDrones();
                        break;
                    case ThreatType.PirateFreighter:
                        currentQuantity = Simulation.GetPirateFreighters();
                        break;
                    case ThreatType.PirateCorvette:
                        currentQuantity = Simulation.GetPirateCorvettes();
                        break;
                }

                status = currentQuantity >= quantity;
                return status;
            }
        }

        public override string GetDescription()
        {
            string outOf = Lean.Localization.LeanLocalization.GetTranslationText("destroyed out of");
            string text = GetLocalizedDescription();
            text += "<size=16>\n";
            if (status) text += "<color=white>";
            else text += "<color=red>";
            text += currentQuantity + "</color>";
            text += $" {outOf} <color=white>" + quantity + "</color>";
            text += "</size>";
            return text;
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }

    /// <summary>
    /// Transition Start Drone Attack: Type 9
    /// Create a group of drones to attack the player
    /// There are two types of drone attacks:
    ///     Normal: the system will create a drone factory that spawns a drone each [cooldown] seconds
    ///     Swarm: the system will create all the drones in the attack simultáneously
    /// </summary>
    [Serializable]
    public class StartDroneAttack : Transition
    {
        public BuildingType targetType;
        public float cooldown;
        public int quantity;
        public bool swarm;

        private bool check = false;

        public StartDroneAttack(BuildingType targetType, float cooldown, int quantity, bool swarm)
            : base(Type.StartDroneAttack)
        {
            this.targetType = targetType;
            this.cooldown = cooldown;
            this.quantity = quantity;
            this.swarm = swarm;
        }

        public override bool Check()
        {
            if(Simulation.quickLoad)
            {
                check = true;
            }

            if (!check)
            {
                check = true;
                if (swarm)
                {
                    DroneFactory.CreateSwarm(quantity, targetType);
                }
                else
                {
                    DroneFactory.CreateFactory(targetType, cooldown, quantity);
                }
            }
            return true;
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

    }

    /// <summary>
    /// Transition Create Pirate Freighter: Type 10
    /// Create a given quantity of pirate freighters that will try to steal certain amount of a given resource
    /// There will be a maximum of 4 freighters at any time in the level, once a frighter is destroyed, a new freighter can take its place
    /// This event fires only once
    /// </summary>
    [Serializable]
    public class CreatePirateFreighter : Transition
    {
        public Parameter targetResource;
        public int quantity;
        private bool check = false;
        private PirateFreighter[] freighters;
        private int freightersCreated = 0;

        public CreatePirateFreighter(Parameter targetResource, int quantity) 
            : base(Type.CreatePirateFreighter)
        {
            this.targetResource = targetResource;
            this.quantity = quantity;    
        }

        public override bool Check()
        {
            if(Simulation.quickLoad)
            {
                check = true;
            }

            if(!check)
            {
                check = true;
                FreighterFactory.CreateFactory(targetResource, quantity);
            }
            return true;
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }

    /// <summary>
    /// Transition Create Pirate Corvette: Type 11
    /// Create a given quantity of Corvettes that will attack the player in succession
    /// The player has to destroy all the corvettes for the objective to Check
    /// </summary>
    [Serializable]
    public class CreatePirateCorvette : Transition
    {
        public int count;
        private int corvettes;
        private bool check = false;
        private GameObject currentCorvette;

        public CreatePirateCorvette(string description, int count) : base(Type.CreatePirateCorvette, description)
        {
            this.count = count;
        }

        public override bool Check()
        {
            if (Simulation.quickLoad)
            {
                
                check = true;
            }
            if(!check)
            {
                CorvetteFactory.CreateFactory(count);
                check = true;
            }
            corvettes = Simulation.GetPirateCorvettes();
            return corvettes >= count;
        }

        public override string GetDescription()
        {
            string text = GetLocalizedDescription();
            text += "\n";
            text += "<size=9>";

            if (status) text += "<color=white>";
            else text += "<color=red>";
            text += corvettes;
            text += "</color>";

            text += "<color=white>";
            text += " / " + count;
            text += "</color>";

            text += "</size>";
            return text;
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }

    /// <summary>
    /// Transition Fire Orbital Laser: Type 12
    /// The player has to fire the orbital laser a given amount of times
    /// </summary>
    [Serializable]
    public class FireOrbitalLaser : Transition
    {
        public int count;

        public FireOrbitalLaser(string description, bool persistent, int count) : 
            base(Type.FireOrbitalLaser, description, persistent)
        {
            this.count = count;
        }

        public override bool Check()
        {
            if (count <= Simulation.GetOrbitalLaserShots())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }

    /// <summary>
    /// Transition Create Meteor: Type 13
    /// The system creates a meteor that falls on the planet
    /// If buildingType is not null, the meteor will try to fall in a specific building type, or any building
    /// Else the meteor will fall in a random position in the planet
    /// </summary>
    [Serializable]
    public class CreateMeteor : Transition
    {
        public BuildingType buildingType;
        public bool onBuilding;
        private bool check = false;

        public CreateMeteor(BuildingType buildingType, bool onBuilding) : base(Type.CreateMeteor)
        {
            this.buildingType = buildingType;
            this.onBuilding = onBuilding;
        }

        public override bool Check()
        {
            if(!check)
            {
                check = true;
                Vector3 targetPos = UnityEngine.Random.onUnitSphere;
                
                if(onBuilding)
                {
                    List<global::Building> buildings = global::Building.GetBuildingsOfType(buildingType);
                    if (buildings.Count > 0)
                    {
                        targetPos = buildings[0].transform.position;
                    }
                    else
                    {
                        global::Building b = global::Building.GetRandomBuilding();
                        if (b != null)
                        {
                            targetPos = b.transform.position;
                        }
                    }
                }

                MeteorFactory.CreateMeteor(targetPos, true);
            }
            return true;
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }

    /// <summary>
    /// Transition Create Meteor Factory: Type 14
    /// Creates a meteor factory that spawns meteorites at semi-random intervals
    /// If buildingType is not null, the factory will try to spawn meteorites for a certain building type
    /// Else the meteorites will spawn to a random position in the planet
    /// </summary>
    [Serializable]
    public class CreateMeteorFactory: Transition
    {
        public BuildingType buildingType;
        public bool onBuilding;
        public int count;
        public int rate;

        private bool check = false;

        public CreateMeteorFactory(BuildingType buildingType, bool onBuilding, int count, int rate):
            base(Type.CreateMeteorFactory)
        {
            this.buildingType = buildingType;
            this.onBuilding = onBuilding;
            this.count = count;
            this.rate = rate;
        }

        public override bool Check()
        {
            if(Simulation.quickLoad)
            {
                check = true;
            }

            if(!check)
            {
                check = true;
                if (onBuilding)
                    MeteorFactory.CreateFactory(rate, 5, count, buildingType);
                else
                    MeteorFactory.CreateFactory(rate, 5, count, null);
            }

            return true;
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }

    /// <summary>
    /// Transition Start Missile Attack: Type 14
    /// Creates a missile factory that spawns <see cref="fireRate"/> per second for a total of <see cref="missileCount"/> <see cref="MissileBarrage"/>
    /// If there is a building of <see cref="targetType"/> the Missile Attack will concentrate over it
    /// Else it will concentrate on any other random building
    /// </summary>
    [Serializable]
    public class StartMissileAttack : Transition
    {
        public BuildingType targetType;
        public int missileCount;
        public int fireRate;

        private bool check = false;

        public StartMissileAttack(BuildingType targetType, int missileCount, int fireRate):
            base(Type.StartMissileAttack)
        {
            this.targetType = targetType;
            this.missileCount = missileCount;
            this.fireRate = fireRate;
        }

        public override bool Check()
        {
            if(Simulation.quickLoad)
            {
                check = true;
            }
            if(!check)
            {
                check = true;
                MissileBarrage.CreateMissileBarrage(targetType, missileCount, fireRate);
            }
            return true;
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }

    [Serializable]
    public class StoreMaterial : Transition
    {
        public Parameter material;
        public int quantity;
        private double storedMaterial = 0;

        public StoreMaterial(string description, bool persistent, Parameter material, int quantity): 
            base(Type.StoreMaterial)
        {
            base.description = description;
            this.material = material;
            this.quantity = quantity;
        }

        public override bool Check()
        {
            if(persistent && status)
            {
                return true;
            }
            else
            {
                double epsilon = 0;
                if(TimeControl.GetTimeScale() > 1)
                {
                    epsilon = 1 * Math.Pow(TimeControl.GetTimeScale(), 2);
                }
                List<global::Building> containers = null;
                storedMaterial = 0;
                switch(material)
                {
                    case Parameter.OreRaw:
                        containers = global::Building.GetBuildingsOfType(BuildingType.Warehouse);
                        foreach(global::Building b in containers)
                        {
                            storedMaterial += b.GetComponent<Warehouse>().GetStoredOreRaw();
                        }
                        break;
                    case Parameter.OreRefined:
                        containers = global::Building.GetBuildingsOfType(BuildingType.Warehouse);
                        foreach (global::Building b in containers)
                        {
                            storedMaterial += b.GetComponent<Warehouse>().GetStoredOreRefined();
                        }
                        break;
                    case Parameter.Food:
                        containers = global::Building.GetBuildingsOfType(BuildingType.Granary);
                        foreach (global::Building b in containers)
                        {
                            storedMaterial += b.GetComponent<Granary>().GetFoodStored();
                        }
                        break;
                    case Parameter.H2O:
                        containers = global::Building.GetBuildingsOfType(BuildingType.WaterContainer);
                        foreach (global::Building b in containers)
                        {
                            storedMaterial += b.GetComponent<Container>().GetStoredMaterial(Parameter.H2O);
                        }
                        break;
                    case Parameter.H2SO4:
                        containers = global::Building.GetBuildingsOfType(BuildingType.AcidContainer);
                        foreach (global::Building b in containers)
                        {
                            storedMaterial += b.GetComponent<Container>().GetStoredMaterial(Parameter.H2SO4);
                        }
                        break;
                    case Parameter.CH4:
                        containers = global::Building.GetBuildingsOfType(BuildingType.MethaneContainer);
                        foreach (global::Building b in containers)
                        {
                            storedMaterial += b.GetComponent<Container>().GetStoredMaterial(Parameter.CH4);
                        }
                        break;
                }
                status = (storedMaterial + epsilon) >= quantity;
                return status;
            }
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

        public override string GetDescription()
        {
            string text = GetLocalizedDescription();
            text += "\n";
            text += "<size=9>";

            if (status) text += "<color=white>";
            else text += "<color=red>";
            text += (int)storedMaterial;
            text += "</color>";

            text += "<color=white>";
            text += " > " + quantity;
            text += "</color>";

            text += "</size>";
            return text;
        }
    }


    // TODO MORE

    /// <summary>
    /// Transition Defeat: Type -1
    /// The level ends in a defeat condition
    /// </summary>
    [Serializable]
    public class Defeat : Transition
    {
        public string reason;

        public Defeat(string reason) : base(Type.Defeat)
        {
            this.reason = reason;
        }

        public override bool Check()
        {
            Simulation.SetDefeat(reason);
            return false;
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }

    /// <summary>
    /// Transition Sandbox: Type 99
    /// This transition is always false
    /// Allows the player to play indefinitely
    /// </summary>
    [Serializable]
    public class Sandbox : Transition
    {
        public Sandbox() : base(Type.Sandbox)
        {

        }

        public override bool Check()
        {
            return false;
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}

