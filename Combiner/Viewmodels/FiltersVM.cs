﻿using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Combiner
{
	public class FiltersVM : BaseViewModel
	{
		private DatabaseManagerVM m_DatabaseManagerVM;
		private CreatureDataVM m_CreatureDataVM;
		private ProgressVM m_ProgressVM;
		private Database m_Database;
		private List<CreatureFilter> m_ActiveFilters;

		private ObservableCollection<CreatureFilter> m_ChosenStatFilters;
		public ObservableCollection<CreatureFilter> ChosenStatFilters
		{
			get
			{
				return m_ChosenStatFilters ??
					(m_ChosenStatFilters = new ObservableCollection<CreatureFilter>());
			}
			set
			{
				if (m_ChosenStatFilters != value)
				{
					m_ChosenStatFilters = value;
					OnPropertyChanged(nameof(ChosenStatFilters));
				}
			}
		}

		public FiltersVM(CreatureDataVM creatureDataVM, ProgressVM progressVM, Database database, DatabaseManagerVM databaseManagerVM)
		{
			m_CreatureDataVM = creatureDataVM;
			m_ProgressVM = progressVM;
			m_Database = database;
			m_DatabaseManagerVM = databaseManagerVM;
			m_DatabaseManagerVM.CollectionChangedEvent += UpdateCollection;
			m_ActiveFilters = new List<CreatureFilter>();
			IsQueryFilteringSelected = false;
			InitIsActiveHandlers();
		}

		private void InitIsActiveHandlers()
		{
			AirSpeedFilter.IsActiveChanged += OnFilterIsActive;
			ArmourFilter.IsActiveChanged += OnFilterIsActive;
			CoalFilter.IsActiveChanged += OnFilterIsActive;
			EffectiveHitpointsFilter.IsActiveChanged += OnFilterIsActive;
			SuicideCoefficientFilter.IsActiveChanged += OnFilterIsActive;
			PopSizeFilter.IsActiveChanged += OnFilterIsActive;
			ElectricityFilter.IsActiveChanged += OnFilterIsActive;
			HitpointsFilter.IsActiveChanged += OnFilterIsActive;
			LandSpeedFilter.IsActiveChanged += OnFilterIsActive;
			MeleeDamageFilter.IsActiveChanged += OnFilterIsActive;
			PowerFilter.IsActiveChanged += OnFilterIsActive;
			RangeDamageFilter.IsActiveChanged += OnFilterIsActive;
			RankFilter.IsActiveChanged += OnFilterIsActive;
			SightRadiusFilter.IsActiveChanged += OnFilterIsActive;
			WaterSpeedFilter.IsActiveChanged += OnFilterIsActive;
			RangeDistanceFilter.IsActiveChanged += OnFilterIsActive;
			SizeFilter.IsActiveChanged += OnFilterIsActive;

			SingleRangedFilter.IsActiveChanged += OnFilterIsActive;
			HornsFilter.IsActiveChanged += OnFilterIsActive;
			PoisonFilter.IsActiveChanged += OnFilterIsActive;
			BarrierDestroyFilter.IsActiveChanged += OnFilterIsActive;
			LandOnlyFilter.IsActiveChanged += OnFilterIsActive;
			WaterOnlyFilter.IsActiveChanged += OnFilterIsActive;
			AmphibOnlyFilter.IsActiveChanged += OnFilterIsActive;
			AirOnlyFilter.IsActiveChanged += OnFilterIsActive;
			TicksFilter.IsActiveChanged += OnFilterIsActive;
			CoalElecRatioFilter.IsActiveChanged += OnFilterIsActive;
			NERatingFilter.IsActiveChanged += OnFilterIsActive;


			// TODO: Can these be handled in a different way?
			RangeOptionsFilter.MeleeOnlyFilter.IsActiveChanged += OnFilterIsActive;
			RangeOptionsFilter.RangeOnlyFilter.IsActiveChanged += OnFilterIsActive;
			RangeOptionsFilter.DirectRangeFilter.IsActiveChanged += OnFilterIsActive;
			RangeOptionsFilter.SonicRangeFilter.IsActiveChanged += OnFilterIsActive;
			RangeOptionsFilter.PoisonRangeFilter.IsActiveChanged += OnFilterIsActive;
			RangeOptionsFilter.QuillRangeFilter.IsActiveChanged += OnFilterIsActive;
			RangeOptionsFilter.ArtilleryOnlyFilter.IsActiveChanged += OnFilterIsActive;
			RangeOptionsFilter.RockArtilleryFilter.IsActiveChanged += OnFilterIsActive;
			RangeOptionsFilter.WaterArtilleryFilter.IsActiveChanged += OnFilterIsActive;
			RangeOptionsFilter.ChemicalArtilleryFilter.IsActiveChanged += OnFilterIsActive;

			StockFilter.IsActiveChanged += OnFilterIsActive;
			AbilityFilter.IsActiveChanged += OnFilterIsActive;
		}

		private void OnFilterIsActive(CreatureFilter filter, IsActiveArgs args)
		{
			if (args.IsActive)
			{
				m_ActiveFilters.Add(filter);
				if (filter is StatFilter)
				{
					ChosenStatFilters.Add(filter);
				}
			}
			else
			{
				m_ActiveFilters.Remove(filter);
				if (filter is StatFilter)
				{
					ChosenStatFilters.Remove(filter);
				}
			}
		}

		private RelayCommand m_ResetFiltersCommand;
		public RelayCommand ResetFiltersCommand
		{
			get
			{
				return m_ResetFiltersCommand ??
					(m_ResetFiltersCommand = new RelayCommand(ResetFilters));
			}
			set
			{
				if (m_ResetFiltersCommand != value)
				{
					m_ResetFiltersCommand = value;
					OnPropertyChanged(nameof(ResetFiltersCommand));
				}
			}
		}

		private void ResetFilters(object o)
		{
			AirSpeedFilter.ResetFilter();
			ArmourFilter.ResetFilter();
			CoalFilter.ResetFilter();
			EffectiveHitpointsFilter.ResetFilter();
			SuicideCoefficientFilter.ResetFilter();
			ElectricityFilter.ResetFilter();
			HitpointsFilter.ResetFilter();
			LandSpeedFilter.ResetFilter();
			MeleeDamageFilter.ResetFilter();
			PowerFilter.ResetFilter();
			RangeDamageFilter.ResetFilter();
			RangeDistanceFilter.ResetFilter();
			RankFilter.ResetFilter();
			SightRadiusFilter.ResetFilter();
			SizeFilter.ResetFilter();
			WaterSpeedFilter.ResetFilter();
			SingleRangedFilter.ResetFilter();
			HornsFilter.ResetFilter();
			PoisonFilter.ResetFilter();
			BarrierDestroyFilter.ResetFilter();
			RangeOptionsFilter.ResetFilter();
			StockFilter.ResetFilter();
			AbilityFilter.ResetFilter();
			LandOnlyFilter.ResetFilter();
			WaterOnlyFilter.ResetFilter();
			AmphibOnlyFilter.ResetFilter();
			AirOnlyFilter.ResetFilter();
			PopSizeFilter.ResetFilter();
			TicksFilter.ResetFilter();
			CoalElecRatioFilter.ResetFilter();
			NERatingFilter.ResetFilter();
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
			//m_ProgressVM.StartWork();
			//await Task.Run(() =>
			//{
			if (m_DatabaseManagerVM.ActiveCollection == null)
			{
				MessageBox.Show("Please set an active collection before filtering.");
			}
			else if (IsQueryFilteringSelected)
			{
				m_CreatureDataVM.Creatures = 
					new ObservableCollection<Creature>(
						m_Database.GetCreatureQuery(
							BuildFilterQuery(), 
							m_DatabaseManagerVM.ActiveCollection));
			}
			else
			{
				m_CreatureDataVM.Creatures =
					new ObservableCollection<Creature>(
							m_Database.GetAllCreatures(m_DatabaseManagerVM.ActiveCollection));
				m_CreatureDataVM.CreaturesView.Filter = CreatureFilter;
			}
			//});
			//m_ProgressVM.EndWork();
		}

		private void UpdateCollection(ModCollection collectionName)
		{
			FilterCreatures(null);
		}

		/// <summary>
		/// Builds the Predicate for all filters that are being applied
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public bool CreatureFilter(object obj)
		{
			if (m_ActiveFilters.Count == 0)
			{
				return true;
			}

			Creature creature = obj as Creature;
			if (creature != null)
			{
				bool result = true;
				foreach (CreatureFilter filter in m_ActiveFilters)
				{
					result = result && filter.Filter(creature);
				}
				return result;
			}
			return false;
		}

		private bool m_IsQueryFilteringSelected;
		public bool IsQueryFilteringSelected
		{
			get { return m_IsQueryFilteringSelected; }
			set
			{
				if (m_IsQueryFilteringSelected != value)
				{
					m_IsQueryFilteringSelected = value;
					OnPropertyChanged(nameof(IsQueryFilteringSelected));
				}
			}
		}

		private BsonExpression BuildFilterQuery()
		{
			if (m_ActiveFilters.Count == 0)
			{
				return Query.All().Select;
			}

			if (m_ActiveFilters.Count == 1)
			{
				return m_ActiveFilters.First().BuildQuery();
			}

			IEnumerable<BsonExpression> queries = m_ActiveFilters.Select(x => x.BuildQuery());
			return Query.And(queries.ToArray());
		}

		#region Filters

		private CreatureFilter m_RankFilter;
		public CreatureFilter RankFilter
		{
			get
			{
				return m_RankFilter
					?? (m_RankFilter = new RankFilter());
			}
			set
			{
				if (m_RankFilter != value)
				{
					m_RankFilter = value;
					OnPropertyChanged(nameof(RankFilter));
				}
			}
		}

		private CreatureFilter m_CoalFilter;
		public CreatureFilter CoalFilter
		{
			get
			{
				return m_CoalFilter
					?? (m_CoalFilter = new CoalFilter());
			}
			set
			{
				if (m_CoalFilter != value)
				{
					m_CoalFilter = value;
					OnPropertyChanged(nameof(CoalFilter));
				}
			}
		}

		private CreatureFilter m_ElectricityFilter;
		public CreatureFilter ElectricityFilter
		{
			get
			{
				return m_ElectricityFilter
					?? (m_ElectricityFilter = new ElectricityFilter());
			}
			set
			{
				if (m_ElectricityFilter != value)
				{
					m_ElectricityFilter = value;
					OnPropertyChanged(nameof(ElectricityFilter));
				}
			}
		}

		private CreatureFilter m_PowerFilter;
		public CreatureFilter PowerFilter
		{
			get
			{
				return m_PowerFilter
					?? (m_PowerFilter = new PowerFilter());
			}
			set
			{
				if (m_PowerFilter != value)
				{
					m_PowerFilter = value;
					OnPropertyChanged(nameof(PowerFilter));
				}
			}
		}

		private CreatureFilter m_ArmourFilter;
		public CreatureFilter ArmourFilter
		{
			get
			{
				return m_ArmourFilter
					?? (m_ArmourFilter = new ArmourFilter());
			}
			set
			{
				if (m_ArmourFilter != value)
				{
					m_ArmourFilter = value;
					OnPropertyChanged(nameof(ArmourFilter));
				}
			}
		}

		private CreatureFilter m_HitpointsFilter;
		public CreatureFilter HitpointsFilter
		{
			get
			{
				return m_HitpointsFilter
					?? (m_HitpointsFilter = new HitpointsFilter());
			}
			set
			{
				if (m_HitpointsFilter != value)
				{
					m_HitpointsFilter = value;
					OnPropertyChanged(nameof(HitpointsFilter));
				}
			}
		}

		private CreatureFilter m_EffectiveHitpointsFilter;
		public CreatureFilter EffectiveHitpointsFilter
		{
			get
			{
				return m_EffectiveHitpointsFilter
					?? (m_EffectiveHitpointsFilter = new EffectiveHitpointsFilter());
			}
			set
			{
				if (m_EffectiveHitpointsFilter != value)
				{
					m_EffectiveHitpointsFilter = value;
					OnPropertyChanged(nameof(EffectiveHitpointsFilter));
				}
			}
		}

		private CreatureFilter m_MeleeDamageFilter;
		public CreatureFilter MeleeDamageFilter
		{
			get
			{
				return m_MeleeDamageFilter
					?? (m_MeleeDamageFilter = new MeleeDamageFilter());
			}
			set
			{
				if (m_MeleeDamageFilter != value)
				{
					m_MeleeDamageFilter = value;
					OnPropertyChanged(nameof(MeleeDamageFilter));
				}
			}
		}

		private CreatureFilter m_RangeDamageFilter;
		public CreatureFilter RangeDamageFilter
		{
			get
			{
				return m_RangeDamageFilter
					?? (m_RangeDamageFilter = new RangeDamageFilter());
			}
			set
			{
				if (m_RangeDamageFilter != value)
				{
					m_RangeDamageFilter = value;
					OnPropertyChanged(nameof(RangeDamageFilter));
				}
			}
		}

		private CreatureFilter m_RangeDistanceFilter;
		public CreatureFilter RangeDistanceFilter
		{
			get
			{
				return m_RangeDistanceFilter
					?? (m_RangeDistanceFilter = new RangeDistanceFilter());
			}
			set
			{
				if (m_RangeDistanceFilter != value)
				{
					m_RangeDistanceFilter = value;
					OnPropertyChanged(nameof(RangeDistanceFilter));
				}
			}
		}

		private CreatureFilter m_SightRadiusFilter;
		public CreatureFilter SightRadiusFilter
		{
			get
			{
				return m_SightRadiusFilter
					?? (m_SightRadiusFilter = new SightRadiusFilter());
			}
			set
			{
				if (m_SightRadiusFilter != value)
				{
					m_SightRadiusFilter = value;
					OnPropertyChanged(nameof(SightRadiusFilter));
				}
			}
		}

		private CreatureFilter m_SizeFilter;
		public CreatureFilter SizeFilter
		{
			get
			{
				return m_SizeFilter
					?? (m_SizeFilter = new SizeFilter());
			}
			set
			{
				if (m_SizeFilter != value)
				{
					m_SizeFilter = value;
					OnPropertyChanged(nameof(SizeFilter));
				}
			}
		}

		private CreatureFilter m_LandSpeedFilter;
		public CreatureFilter LandSpeedFilter
		{
			get
			{
				return m_LandSpeedFilter
					?? (m_LandSpeedFilter = new LandSpeedFilter());
			}
			set
			{
				if (m_LandSpeedFilter != value)
				{
					m_LandSpeedFilter = value;
					OnPropertyChanged(nameof(LandSpeedFilter));
				}
			}
		}

		private CreatureFilter m_WaterSpeedFilter;
		public CreatureFilter WaterSpeedFilter
		{
			get
			{
				return m_WaterSpeedFilter
					?? (m_WaterSpeedFilter = new WaterSpeedFilter());
			}
			set
			{
				if (m_WaterSpeedFilter != value)
				{
					m_WaterSpeedFilter = value;
					OnPropertyChanged(nameof(WaterSpeedFilter));
				}
			}
		}

		private CreatureFilter m_AirSpeedFilter;
		public CreatureFilter AirSpeedFilter
		{
			get
			{
				return m_AirSpeedFilter
					?? (m_AirSpeedFilter = new AirSpeedFilter());
			}
			set
			{
				if (m_AirSpeedFilter != value)
				{
					m_AirSpeedFilter = value;
					OnPropertyChanged(nameof(AirSpeedFilter));
				}
			}
		}

		private CreatureFilter m_StockFilter;
		public CreatureFilter StockFilter
		{
			get
			{
				return m_StockFilter
					?? (m_StockFilter = new StockFilter());
			}
			set
			{
				if (m_StockFilter != value)
				{
					m_StockFilter = value;
					OnPropertyChanged(nameof(StockFilter));
				}
			}
		}

		private CreatureFilter m_AbilityFilter;
		public CreatureFilter AbilityFilter
		{
			get
			{
				return m_AbilityFilter
					?? (m_AbilityFilter = new AbilityFilter());
			}
			set
			{
				if (m_AbilityFilter != value)
				{
					m_AbilityFilter = value;
					OnPropertyChanged(nameof(AbilityFilter));
				}
			}
		}

		private CreatureFilter m_SingleRangedFilter;
		public CreatureFilter SingleRangedFilter
		{
			get
			{
				return m_SingleRangedFilter
					?? (m_SingleRangedFilter = new SingleRangedFilter());
			}
			set
			{
				if (m_SingleRangedFilter != value)
				{
					m_SingleRangedFilter = value;
					OnPropertyChanged(nameof(SingleRangedFilter));
				}
			}
		}

		private RangeOptionsFilter m_RangeOptionsFilter;
		public RangeOptionsFilter RangeOptionsFilter
		{
			get
			{
				return m_RangeOptionsFilter
					?? (m_RangeOptionsFilter = new RangeOptionsFilter());
			}
			set
			{
				if (m_RangeOptionsFilter != value)
				{
					m_SingleRangedFilter = value;
					OnPropertyChanged(nameof(RangeOptionsFilter));
				}
			}
		}

		private CreatureFilter m_HornsFilter;
		public CreatureFilter HornsFilter
		{
			get
			{
				return m_HornsFilter
					?? (m_HornsFilter = new HornsFilter());
			}
			set
			{
				if (m_HornsFilter != value)
				{
					m_HornsFilter = value;
					OnPropertyChanged(nameof(HornsFilter));
				}
			}
		}

		private CreatureFilter m_PoisonFilter;
		public CreatureFilter PoisonFilter
		{
			get
			{
				return m_PoisonFilter
					?? (m_PoisonFilter = new PoisonFilter());
			}
			set
			{
				if (m_PoisonFilter != value)
				{
					m_PoisonFilter = value;
					OnPropertyChanged(nameof(PoisonFilter));
				}
			}
		}

		private CreatureFilter m_BarrierDestroyFilter;
		public CreatureFilter BarrierDestroyFilter
		{
			get
			{
				return m_BarrierDestroyFilter
					?? (m_BarrierDestroyFilter = new BarrierDestroyFilter());
			}
			set
			{
				if (m_BarrierDestroyFilter != value)
				{
					m_BarrierDestroyFilter = value;
					OnPropertyChanged(nameof(BarrierDestroyFilter));
				}
			}
		}

		private CreatureFilter m_LandOnlyFilter;
		public CreatureFilter LandOnlyFilter
		{
			get
			{
				return m_LandOnlyFilter
					?? (m_LandOnlyFilter = new LandOnlyFilter());
			}
			set
			{
				if (m_LandOnlyFilter != value)
				{
					m_LandOnlyFilter = value;
					OnPropertyChanged(nameof(LandOnlyFilter));
				}
			}
		}

		private CreatureFilter m_AirOnlyFilter;
		public CreatureFilter AirOnlyFilter
		{
			get
			{
				return m_AirOnlyFilter
					?? (m_AirOnlyFilter = new AirOnlyFilter());
			}
			set
			{
				if (m_AirOnlyFilter != value)
				{
					m_AirOnlyFilter = value;
					OnPropertyChanged(nameof(AirOnlyFilter));
				}
			}
		}

		private CreatureFilter m_WaterOnlyFilter;
		public CreatureFilter WaterOnlyFilter
		{
			get
			{
				return m_WaterOnlyFilter
					?? (m_WaterOnlyFilter = new WaterOnlyFilter());
			}
			set
			{
				if (m_WaterOnlyFilter != value)
				{
					m_WaterOnlyFilter = value;
					OnPropertyChanged(nameof(WaterOnlyFilter));
				}
			}
		}

		private CreatureFilter m_AmphibOnlyFilter;
		public CreatureFilter AmphibOnlyFilter
		{
			get
			{
				return m_AmphibOnlyFilter
					?? (m_AmphibOnlyFilter = new AmphibOnlyFilter());
			}
			set
			{
				if (m_AmphibOnlyFilter != value)
				{
					m_AmphibOnlyFilter = value;
					OnPropertyChanged(nameof(AmphibOnlyFilter));
				}
			}
		}

		private CreatureFilter m_SuicideCoefficientFilter;
		public CreatureFilter SuicideCoefficientFilter
		{
			get
			{
				return m_SuicideCoefficientFilter
					?? (m_SuicideCoefficientFilter = new SuicideCoefficientFilter());
			}
			set
			{
				if (m_SuicideCoefficientFilter != value)
				{
					m_SuicideCoefficientFilter = value;
					OnPropertyChanged(nameof(SuicideCoefficientFilter));
				}
			}
		}

		private CreatureFilter m_PopSizeFilter;
		public CreatureFilter PopSizeFilter
		{
			get
			{
				return m_PopSizeFilter
					?? (m_PopSizeFilter = new PopSizeFilter());
			}
			set
			{
				if (m_PopSizeFilter != value)
				{
					m_PopSizeFilter = value;
					OnPropertyChanged(nameof(PopSizeFilter));
				}
			}
		}

		private CreatureFilter m_CoalElecRatioFilter;
		public CreatureFilter CoalElecRatioFilter
		{
			get
			{
				return m_CoalElecRatioFilter
					?? (m_CoalElecRatioFilter = new CoalElecRatioFilter());
			}
			set
			{
				if (m_CoalElecRatioFilter != value)
				{
					m_CoalElecRatioFilter = value;
					OnPropertyChanged(nameof(CoalElecRatioFilter));
				}
			}
		}

		private CreatureFilter m_NERatingFilter;
		public CreatureFilter NERatingFilter
		{
			get
			{
				return m_NERatingFilter
					?? (m_NERatingFilter = new NERatingFilter());
			}
			set
			{
				if (m_NERatingFilter != value)
				{
					m_NERatingFilter = value;
					OnPropertyChanged(nameof(NERatingFilter));
				}
			}
		}


		private CreatureFilter m_TicksFilter;
		public CreatureFilter TicksFilter
		{
			get
			{
				return m_TicksFilter
					?? (m_TicksFilter = new TicksFilter());
			}
			set
			{
				if (m_TicksFilter != value)
				{
					m_TicksFilter = value;
					OnPropertyChanged(nameof(TicksFilter));
				}
			}
		}

		#endregion
	}
}

