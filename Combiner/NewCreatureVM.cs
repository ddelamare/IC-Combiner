﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Combiner
{
	public class NewCreatureVM : BaseViewModel
	{
		private ObservableCollection<Creature> m_Creatures;
		public ObservableCollection<Creature> Creatures
		{
			get
			{
				return m_Creatures ?? (m_Creatures = new ObservableCollection<Creature>());
			}
			set
			{
				if (value != m_Creatures)
				{
					m_Creatures = value;
					CreaturesView = CollectionViewSource.GetDefaultView(m_Creatures);
					OnPropertyChanged(nameof(Creatures));
				}
			}
		}

		private ICollectionView m_CreaturesView;
		public ICollectionView CreaturesView
		{
			get
			{
				return m_CreaturesView ?? (m_CreaturesView = CollectionViewSource.GetDefaultView(Creatures));
			}
			set
			{
				if (value != m_CreaturesView)
				{
					m_CreaturesView = value;
					OnPropertyChanged(nameof(CreaturesView));
				}
			}
		}

		public NewCreatureVM()
		{
			SetDefaultFilters();
		}

		private ICommand m_CreateDatabaseCommand;
		public ICommand CreateDatabaseCommand
		{
			get
			{
				return m_CreateDatabaseCommand ??
				  (m_CreateDatabaseCommand = new RelayCommand(CreateDatabase));
			}
			set
			{
				if (value != m_CreateDatabaseCommand)
				{
					m_CreateDatabaseCommand = value;
					OnPropertyChanged(nameof(CreateDatabaseCommand));
				}
			}
		}
		private void CreateDatabase(object obj)
		{
			string text = "Creating a new database will delete and replace your current database. This could take a while, but a dialog box will appear when it is finished. Would you like to continue?";
			MessageBoxResult result = MessageBox.Show(text, "Database Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
			if (result == MessageBoxResult.Yes)
			{
				Database.CreateDB();
				MessageBox.Show("Finished creating the database.");
			}
		}

		private ICommand m_LoadCreaturesCommand;
		public ICommand LoadCreaturesCommand
		{
			get
			{
				return m_LoadCreaturesCommand ??
				  (m_LoadCreaturesCommand = new RelayCommand(LoadCreatures));
			}
			set
			{
				if (value != m_LoadCreaturesCommand)
				{
					m_LoadCreaturesCommand = value;
					OnPropertyChanged(nameof(LoadCreaturesCommand));
				}
			}
		}
		private void LoadCreatures(object obj)
		{
			Creatures = new ObservableCollection<Creature>(Database.GetAllCreatures());
			CreaturesView.Filter = CreatureFilter;
		}

		private ObservableCollection<string> m_AbilityChoices;
		public ObservableCollection<string> AbilityChoices
		{
			get
			{
				if (m_AbilityChoices == null)
				{
					m_AbilityChoices = new ObservableCollection<string>();
					foreach (string ability in Utility.Abilities)
					{
						m_AbilityChoices.Add(ability);
					}
				}
				return m_AbilityChoices;
			}
			set
			{
				if (value != m_AbilityChoices)
				{
					m_AbilityChoices = value;
					OnPropertyChanged(nameof(AbilityChoices));
				}
			}
		}
		public string SelectedAddAbility { get; set; }

		private ObservableCollection<string> m_ChosenAbilities = new ObservableCollection<string>();
		public ObservableCollection<string> ChosenAbilities
		{
			get
			{
				return m_ChosenAbilities;
			}
			set
			{
				if (value != m_ChosenAbilities)
				{
					m_ChosenAbilities = value;
					OnPropertyChanged(nameof(ChosenAbilities));
				}
			}
		}
		public string SelectedRemoveAbility { get; set; }

		private ICommand m_AddAbilityChoiceCommand;
		public ICommand AddAbilityChoiceCommand
		{
			get
			{
				return m_AddAbilityChoiceCommand ??
				  (m_AddAbilityChoiceCommand = new RelayCommand(AddAbilityChoice));
			}
			set
			{
				if (value != m_AddAbilityChoiceCommand)
				{
					m_AddAbilityChoiceCommand = value;
					OnPropertyChanged(nameof(AddAbilityChoiceCommand));
				}
			}
		}
		private void AddAbilityChoice(object obj)
		{
			if (!ChosenAbilities.Contains(SelectedAddAbility))
			{
				ChosenAbilities.Add(SelectedAddAbility);
				ChosenAbilities = new ObservableCollection<string>(ChosenAbilities.OrderBy(s => s));
				// sort chosen stock
				AbilityChoices.Remove(SelectedAddAbility);
			}
		}

		private ICommand m_RemoveAbilityChoiceCommand;
		public ICommand RemoveAbilityChoiceCommand
		{
			get
			{
				return m_RemoveAbilityChoiceCommand ??
				  (m_RemoveAbilityChoiceCommand = new RelayCommand(RemoveAbilityChoice));
			}
			set
			{
				if (value != m_RemoveAbilityChoiceCommand)
				{
					m_RemoveAbilityChoiceCommand = value;
					OnPropertyChanged(nameof(RemoveAbilityChoiceCommand));
				}
			}
		}
		private void RemoveAbilityChoice(object obj)
		{
			AbilityChoices.Add(SelectedRemoveAbility);
			AbilityChoices = new ObservableCollection<string>(AbilityChoices.OrderBy(s => s));
			// sort stock choices
			ChosenAbilities.Remove(SelectedRemoveAbility);
		}

		private ICommand m_RemoveAllAbilityChoicesCommand;
		public ICommand RemoveAllAbilityChoicesCommand
		{
			get
			{
				return m_RemoveAllAbilityChoicesCommand ??
				  (m_RemoveAllAbilityChoicesCommand = new RelayCommand(RemoveAllAbilityChoices));
			}
			set
			{
				if (value != m_RemoveAllAbilityChoicesCommand)
				{
					m_RemoveAllAbilityChoicesCommand = value;
					OnPropertyChanged(nameof(RemoveAllAbilityChoicesCommand));
				}
			}
		}
		private void RemoveAllAbilityChoices(object obj)
		{
			foreach (string ability in ChosenAbilities)
			{
				AbilityChoices.Add(ability);
			}
			ChosenAbilities = new ObservableCollection<string>();
			AbilityChoices = new ObservableCollection<string>(AbilityChoices.OrderBy(s => s));
		}

		private ICommand m_FilterCreaturesCommand;
		public ICommand FilterCreaturesCommand
		{
			get
			{
				return m_FilterCreaturesCommand ??
				  (m_FilterCreaturesCommand = new RelayCommand(FilterCreatures));
			}
			set
			{
				if (value != m_FilterCreaturesCommand)
				{
					m_FilterCreaturesCommand = value;
					OnPropertyChanged(nameof(FilterCreaturesCommand));
				}
			}
		}
		private void FilterCreatures(object obj)
		{
			CreaturesView.Filter = CreatureFilter;
		}

		private bool CreatureFilter(object obj)
		{
			Creature creature = obj as Creature;
			if (creature != null)
			{
				return FilterStats(creature)
					&& FilterSingleRangeDamage(creature)
					&& FilterArtillery(creature)
					&& FilterRangeDamage(creature)
					&& FilterAbilities(creature);
			}
			return false;
		}

		private bool FilterStats(Creature creature)
		{
			return creature.Rank >= MinRank
					&& creature.Rank <= MaxRank
					&& creature.Coal >= MinCoal
					&& creature.Coal <= MaxCoal
					&& creature.Electricity >= MinElec
					&& creature.Electricity <= MaxElec
					&& creature.Power >= MinPower
					&& creature.Power <= MaxPower
					&& creature.EffectiveHitpoints >= MinEHP
					&& creature.EffectiveHitpoints <= MaxEHP
					&& creature.Hitpoints >= MinHitpoints
					&& creature.Hitpoints <= MaxHitpoints
					&& creature.Armour >= MinArmour
					&& creature.Armour <= MaxArmour
					&& creature.SightRadius >= MinSightRadius
					&& creature.SightRadius <= MaxSightRadius
					&& creature.LandSpeed >= MinLandSpeed
					&& creature.LandSpeed <= MaxLandSpeed
					&& creature.WaterSpeed >= MinWaterSpeed
					&& creature.WaterSpeed <= MaxWaterSpeed
					&& creature.AirSpeed >= MinAirSpeed
					&& creature.AirSpeed <= MaxAirSpeed
					&& creature.MeleeDamage >= MinMeleeDamage
					&& creature.MeleeDamage <= MaxMeleeDamage;
		}

		private bool FilterRangeDamage(Creature creature)
		{
			return (creature.RangeDamage1 >= MinRangeDamage
				&& creature.RangeDamage1 <= MaxRangeDamage)
				|| (creature.RangeDamage2 >= MinRangeDamage
				&& creature.RangeDamage2 <= MaxRangeDamage);
		}

		private bool FilterAbilities(Creature creature)
		{
			bool hasAbilities = true;
			foreach (string ability in ChosenAbilities)
			{
				if (creature.Abilities.ContainsKey(ability))
				{
					hasAbilities = (creature.Abilities[ability]);
					if (!hasAbilities)
					{
						break;
					}
				}
			}
			return hasAbilities;
		}

		private bool FilterArtillery(Creature creature)
		{
			if (DoArtilleryFilter)
			{
				return creature.RangeSpecial1 > 0 || creature.RangeSpecial2 > 0;
			}
			return true;
		}

		private bool FilterSingleRangeDamage(Creature creature)
		{
			if (DoSingleRangeFilter)
			{
				return !(creature.RangeDamage2 > 0);
			}
			return true;
		}

		private ICommand m_SetDefaultFiltersCommand;
		public ICommand SetDefaultFiltersCommand
		{
			get
			{
				return m_SetDefaultFiltersCommand ??
				  (m_SetDefaultFiltersCommand = new RelayCommand(SetDefaultFilters));
			}
			set
			{
				if (value != m_SetDefaultFiltersCommand)
				{
					m_SetDefaultFiltersCommand = value;
					OnPropertyChanged(nameof(SetDefaultFiltersCommand));
				}
			}
		}
		private void SetDefaultFilters(object obj)
		{
			SetDefaultFilters();
		}

		private void SetDefaultFilters()
		{
			MinRank = 0;
			MaxRank = 5;
			MinCoal = 0;
			MaxCoal = 2000;
			MinElec = 0;
			MaxElec = 2000;
			MinPower = 0;
			MaxPower = 10000;
			MinEHP = 0;
			MaxEHP = 5000;
			MinHitpoints = 0;
			MaxHitpoints = 2000;
			MinArmour = 0;
			MaxArmour = 100;
			MinSightRadius = 0;
			MaxSightRadius = 50;
			MinLandSpeed = 0;
			MaxLandSpeed = 50;
			MinWaterSpeed = 0;
			MaxWaterSpeed = 50;
			MinAirSpeed = 0;
			MaxAirSpeed = 50;
			MinMeleeDamage = 0;
			MaxMeleeDamage = 100;
			MinRangeDamage = 0;
			MaxRangeDamage = 100;
			DoArtilleryFilter = false;
			DoSingleRangeFilter = false;

			RemoveAllAbilityChoices(null);
		}



		private int m_MinRank;
		public int MinRank
		{
			get { return m_MinRank; }
			set
			{
				if (m_MinRank != value)
				{
					m_MinRank = value;
					OnPropertyChanged(nameof(MinRank));
				}
			}
		}

		private int m_MaxRank;
		public int MaxRank
		{
			get { return m_MaxRank; }
			set
			{
				if (m_MaxRank != value)
				{
					m_MaxRank = value;
					OnPropertyChanged(nameof(MaxRank));
				}
			}
		}

		private int m_MinCoal;
		public int MinCoal
		{
			get { return m_MinCoal; }
			set
			{
				if (m_MinCoal != value)
				{
					m_MinCoal = value;
					OnPropertyChanged(nameof(MinCoal));
				}
			}
		}

		private int m_MaxCoal;
		public int MaxCoal
		{
			get { return m_MaxCoal; }
			set
			{
				if (m_MaxCoal != value)
				{
					m_MaxCoal = value;
					OnPropertyChanged(nameof(MaxCoal));
				}
			}
		}

		private int m_MinElec;
		public int MinElec
		{
			get { return m_MinElec; }
			set
			{
				if (m_MinElec != value)
				{
					m_MinElec = value;
					OnPropertyChanged(nameof(MinElec));
				}
			}
		}

		private int m_MaxElec;
		public int MaxElec
		{
			get { return m_MaxElec; }
			set
			{
				if (m_MaxElec != value)
				{
					m_MaxElec = value;
					OnPropertyChanged(nameof(MaxElec));
				}
			}
		}

		private int m_MinPower;
		public int MinPower
		{
			get { return m_MinPower; }
			set
			{
				if (m_MinPower != value)
				{
					m_MinPower = value;
					OnPropertyChanged(nameof(MinPower));
				}
			}
		}

		private int m_MaxPower;
		public int MaxPower
		{
			get { return m_MaxPower; }
			set
			{
				if (m_MaxPower != value)
				{
					m_MaxPower = value;
					OnPropertyChanged(nameof(MaxPower));
				}
			}
		}

		private int m_MinEHP;
		public int MinEHP
		{
			get { return m_MinEHP; }
			set
			{
				if (m_MinEHP != value)
				{
					m_MinEHP = value;
					OnPropertyChanged(nameof(MinEHP));
				}
			}
		}

		private int m_MaxEHP;
		public int MaxEHP
		{
			get { return m_MaxEHP; }
			set
			{
				if (m_MaxEHP != value)
				{
					m_MaxEHP = value;
					OnPropertyChanged(nameof(MaxEHP));
				}
			}
		}

		private int m_MinHitpoints;
		public int MinHitpoints
		{
			get { return m_MinHitpoints; }
			set
			{
				if (m_MinHitpoints != value)
				{
					m_MinHitpoints = value;
					OnPropertyChanged(nameof(MinHitpoints));
				}
			}
		}

		private int m_MaxHitpoints;
		public int MaxHitpoints
		{
			get { return m_MaxHitpoints; }
			set
			{
				if (m_MaxHitpoints != value)
				{
					m_MaxHitpoints = value;
					OnPropertyChanged(nameof(MaxHitpoints));
				}
			}
		}

		private int m_MinArmour;
		public int MinArmour
		{
			get { return m_MinArmour; }
			set
			{
				if (m_MinArmour != value)
				{
					m_MinArmour = value;
					OnPropertyChanged(nameof(MinArmour));
				}
			}
		}

		private int m_MaxArmour;
		public int MaxArmour
		{
			get { return m_MaxArmour; }
			set
			{
				if (m_MaxArmour != value)
				{
					m_MaxArmour = value;
					OnPropertyChanged(nameof(MaxArmour));
				}
			}
		}

		private int m_MinSightRadius;
		public int MinSightRadius
		{
			get { return m_MinSightRadius; }
			set
			{
				if (m_MinSightRadius != value)
				{
					m_MinSightRadius = value;
					OnPropertyChanged(nameof(MinSightRadius));
				}
			}
		}

		private int m_MaxSightRadius;
		public int MaxSightRadius
		{
			get { return m_MaxSightRadius; }
			set
			{
				if (m_MaxSightRadius != value)
				{
					m_MaxSightRadius = value;
					OnPropertyChanged(nameof(MaxSightRadius));
				}
			}
		}

		private int m_MinLandSpeed;
		public int MinLandSpeed
		{
			get { return m_MinLandSpeed; }
			set
			{
				if (m_MinLandSpeed != value)
				{
					m_MinLandSpeed = value;
					OnPropertyChanged(nameof(MinLandSpeed));
				}
			}
		}

		private int m_MaxLandSpeed;
		public int MaxLandSpeed
		{
			get { return m_MaxLandSpeed; }
			set
			{
				if (m_MaxLandSpeed != value)
				{
					m_MaxLandSpeed = value;
					OnPropertyChanged(nameof(MaxLandSpeed));
				}
			}
		}

		private int m_MinWaterSpeed;
		public int MinWaterSpeed
		{
			get { return m_MinWaterSpeed; }
			set
			{
				if (m_MinWaterSpeed != value)
				{
					m_MinWaterSpeed = value;
					OnPropertyChanged(nameof(MinWaterSpeed));
				}
			}
		}

		private int m_MaxWaterSpeed;
		public int MaxWaterSpeed
		{
			get { return m_MaxWaterSpeed; }
			set
			{
				if (m_MaxWaterSpeed != value)
				{
					m_MaxWaterSpeed = value;
					OnPropertyChanged(nameof(MaxWaterSpeed));
				}
			}
		}

		private int m_MinAirSpeed;
		public int MinAirSpeed
		{
			get { return m_MinAirSpeed; }
			set
			{
				if (m_MinAirSpeed != value)
				{
					m_MinAirSpeed = value;
					OnPropertyChanged(nameof(MinAirSpeed));
				}
			}
		}

		private int m_MaxAirSpeed;
		public int MaxAirSpeed
		{
			get { return m_MaxAirSpeed; }
			set
			{
				if (m_MaxAirSpeed != value)
				{
					m_MaxAirSpeed = value;
					OnPropertyChanged(nameof(MaxAirSpeed));
				}
			}
		}

		private int m_MinMeleeDamage;
		public int MinMeleeDamage
		{
			get { return m_MinMeleeDamage; }
			set
			{
				if (m_MinMeleeDamage != value)
				{
					m_MinMeleeDamage = value;
					OnPropertyChanged(nameof(MinMeleeDamage));
				}
			}
		}

		private int m_MaxMeleeDamage;
		public int MaxMeleeDamage
		{
			get { return m_MaxMeleeDamage; }
			set
			{
				if (m_MaxMeleeDamage != value)
				{
					m_MaxMeleeDamage = value;
					OnPropertyChanged(nameof(MaxMeleeDamage));
				}
			}
		}

		private int m_MinRangeDamage;
		public int MinRangeDamage
		{
			get { return m_MinRangeDamage; }
			set
			{
				if (m_MinRangeDamage != value)
				{
					m_MinRangeDamage = value;
					OnPropertyChanged(nameof(MinRangeDamage));
				}
			}
		}

		private int m_MaxRangeDamage;
		public int MaxRangeDamage
		{
			get { return m_MaxRangeDamage; }
			set
			{
				if (m_MaxRangeDamage != value)
				{
					m_MaxRangeDamage = value;
					OnPropertyChanged(nameof(MaxRangeDamage));
				}
			}
		}

		private bool m_DoArtilleryFilter;
		public bool DoArtilleryFilter
		{
			get
			{
				return m_DoArtilleryFilter;
			}
			set
			{
				if (m_DoArtilleryFilter != value)
				{
					m_DoArtilleryFilter = value;
					OnPropertyChanged(nameof(DoArtilleryFilter));
				}
			}
		}

		private bool m_DoSingleRangeFilter;
		public bool DoSingleRangeFilter
		{
			get
			{
				return m_DoSingleRangeFilter;
			}
			set
			{
				if (m_DoSingleRangeFilter != value)
				{
					m_DoSingleRangeFilter = value;
					OnPropertyChanged(nameof(DoSingleRangeFilter));
				}
			}
		}
	}
}
