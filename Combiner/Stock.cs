﻿using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combiner
{
    class Stock
    {
        public string Name { get; set; }
        public Dictionary<Limb, bool> BodyParts { get; set; }
        public Table LimbAttritbutes { get; set; }
		public StockType Type { get; set; }

        public Stock(string name, Table limbAttributes)
        {
            Name = name;
            LimbAttritbutes = limbAttributes;
			Type = DoubleToStockType(GetLimbAttributeValue("stocktype"));
			InitBodyParts();
        }

		// TODO: Exception handling maybe...
		// or null handling
        public double GetLimbAttributeValue(string key)
        {
			var value = LimbAttritbutes[key] as Table;
			if (value != null)
			{
				return (double)value[2];
			}
			return -1;
        }

		// TODO: will casting to int mess this up?
		public int GetLimbAttributeBodyPart(string key)
		{
			var bodyPart = LimbAttritbutes[key] as Table;
			if (bodyPart != null)
			{
				return (int)(double)bodyPart[1];
			}
			return -1;
		}

		private StockType DoubleToStockType(double d)
        {
            foreach (StockType stockType in Enum.GetValues(typeof(StockType)))
            {
                if ((int)stockType == (int)d)
                {
                    return stockType;
                }
            }
            return StockType.Bird;
        }

        private void InitBodyParts()
        {
            BodyParts = new Dictionary<Limb, bool>();
            foreach (Limb limb in Enum.GetValues(typeof(Limb)))
            {
                BodyParts.Add(limb, true);
            }
            BodyParts[Limb.Nothing] = false;

            string[] clawedArachnids = new string [] { "lobster", "shrimp", "scorpion", "praying_mantis", "tarantula", "pistol shrimp", "siphonophore" };
            switch (Type)
            {
                case StockType.Bird:
                    BodyParts[Limb.FrontLegs] = false;
                    BodyParts[Limb.Claws] = false;
                    break;

                case StockType.Quadruped:
                    BodyParts[Limb.Claws] = false;
                    BodyParts[Limb.Wings] = false;
                    break;

                case StockType.Arachnid:
                    if (Name == "siphonophore")
                    {
                        BodyParts[Limb.FrontLegs] = false;
                        BodyParts[Limb.BackLegs] = false;
                        BodyParts[Limb.Wings] = false;
                    }
                    else if (clawedArachnids.Contains(Name))
                    {
                        BodyParts[Limb.Wings] = false;
                    }
                    else
                    {
                        BodyParts[Limb.Claws] = false;
                        BodyParts[Limb.Wings] = false;
                    }
                    break;

                case StockType.Snake:
					BodyParts[Limb.FrontLegs] = false;
					BodyParts[Limb.BackLegs] = false;
					BodyParts[Limb.Claws] = false;
					BodyParts[Limb.Wings] = false;
					break;

                case StockType.Insect:
                    BodyParts[Limb.Claws] = false;
                    break;

                case StockType.Fish:
                    if (Name == "humpback")
                    {
                        BodyParts[Limb.BackLegs] = false;
                        BodyParts[Limb.Claws] = false;
                        BodyParts[Limb.Wings] = false;
                    }
                    else
                    {
                        BodyParts[Limb.FrontLegs] = false;
                        BodyParts[Limb.BackLegs] = false;
                        BodyParts[Limb.Claws] = false;
                        BodyParts[Limb.Wings] = false;
                    }
                    break;

                default:
                    break;
            }
        }

        public bool IsGreaterSize(Stock stock)
		{
			return GetLimbAttributeValue("size") >= stock.GetLimbAttributeValue("size");
		}

		/// <summary>
		/// Gets the difference of the greater stock over the smaller if smaller, otherwise
		/// return 1.0.
		/// Used to increase the stat value of body parts that have grown from their original size.
		/// For example, an ant torso's health value will increase when combined with a sperm whale.
		/// </summary>
		/// <param name="stock"></param>
		/// <returns></returns>
		private double SizeDifference(Stock stock)
		{
			if (IsGreaterSize(stock))
			{
				return 1.0;
			}
			else
			{
				return stock.GetLimbAttributeValue("size") / GetLimbAttributeValue("size");
			}
		}

		#region Calculate Limb Stats

		// TODO: Might want to move calculations to a separate class
		// TODO: GetLimbStats and CalcLimbStats
		private double CalcLimbStats(Limb limb, string stat)
		{
			double limbStats = 0.0;
			switch (limb)
			{
				case Limb.FrontLegs:
					limbStats = GetLimbAttributeValue(stat + "-front");
					break;
				case Limb.BackLegs:
					limbStats = GetLimbAttributeValue(stat + "-back");
					break;
				case Limb.Head:
					limbStats = GetLimbAttributeValue(stat + "-head");
					break;
				case Limb.Torso:
					limbStats = GetLimbAttributeValue(stat + "-torso");
					break;
				case Limb.Tail:
					limbStats = GetLimbAttributeValue(stat + "-tail");
					break;
				case Limb.Wings:
					limbStats = GetLimbAttributeValue(stat + "-wings");
					break;
				case Limb.Claws:
					limbStats = GetLimbAttributeValue(stat + "-claws");
					break;
				default:
					// throw exception
					break;
			}
			return limbStats;
		}

		public double CalcLimbHitpoints(Stock stock, Limb limb)
		{
			double limbHitpoints = CalcLimbStats(limb, "hitpoints");
			return Math.Pow(SizeDifference(stock), GetLimbAttributeValue("exp_hitpoints")) * limbHitpoints;
		}

		public double CalcLimbArmour(Stock stock, Limb limb)
		{
			double limbArmour = CalcLimbStats(limb, "armour");
			return limbArmour * .8;
		}

		public double CalcLimbLandSpeed(Stock stock, Limb limb)
		{
			double limbLandSpeed = CalcLimbStats(limb, "speed_max");
			return Math.Pow(SizeDifference(stock), GetLimbAttributeValue("exp_speed_max")) * limbLandSpeed;
		}

		public double CalcLimbWaterSpeed(Stock stock, Limb limb)
		{
			double limbWaterSpeed = CalcLimbStats(limb, "waterspeed_max");
			return Math.Pow(SizeDifference(stock), GetLimbAttributeValue("exp_waterspeed_max")) * limbWaterSpeed;
		}

		public double CalcLimbAirSpeed(Stock stock, Limb limb)
		{
			double limbAirSpeed = CalcLimbStats(limb, "airspeed_max");
			return Math.Pow(SizeDifference(stock), GetLimbAttributeValue("exp_airspeed_max")) * limbAirSpeed;
		}

		public double CalcLimbMeleeDamage(Stock stock, Limb limb)
		{
			string damageName = "melee" + (int)limb + "_damage";
			string damageExp = "exp_" + damageName;
			return Math.Pow(SizeDifference(stock), GetLimbAttributeValue(damageExp)) * GetLimbAttributeValue(damageName);
		}

		#endregion

	}

    // Not thread safe
    class StockFactory
    {
        private static readonly StockFactory _instance = new StockFactory();

        public static StockFactory Instance
        {
            get
            {
                return _instance;
            }
        }

		private Dictionary<Limb, bool> CreateBodyParts(bool[] values)
		{
			Limb[] limbs = (Limb[])Enum.GetValues(typeof(Limb));
			return limbs.Zip(values, (k, v) => new { k, v })
				.ToDictionary(x => x.k, x => x.v);
		}

		private StockFactory() { }

        public Stock CreateStock(string animalName, LuaHandler lua)
        {
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
			string path = Path.Combine(Environment.CurrentDirectory, Utility.Attrcombiner);
			return new Stock(animalName, lua.GetLimbAttributes(path + animalName + ".lua"));
        }
    }

    enum Limb
    {
        Nothing,
        General,
        FrontLegs,
        BackLegs,
        Head,
        Tail,
        Torso,
        Wings,
        Claws
    }

    enum StockType
    {
        Bird,
        Quadruped,
        Arachnid,
        Snake,
        Insect,
        Fish
    }

    
}
