﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Combiner
{
	/// <summary>
	/// Filter that applies to specific stat values with a given range.
	/// </summary>
	public abstract class StatFilter : CreatureFilter
	{
		public StatFilter(string name, double minDefaultValue, double maxDefaultValue)
			: base(name)
		{
			m_MinDefaultValue = minDefaultValue;
			m_MaxDefaultValue = maxDefaultValue;
			MinValue = minDefaultValue;
			MaxValue = maxDefaultValue;
		}

		private double m_MinDefaultValue;
		private double m_MaxDefaultValue;

		private double m_MinValue;
		public double MinValue
		{
			get { return m_MinValue; }
			set
			{
				if (m_MinValue != value)
				{
					m_MinValue = value;
					OnPropertyChanged(nameof(MinValue));
				}
			}
		}

		private double m_MaxValue;
		public double MaxValue
		{
			get { return m_MaxValue; }
			set
			{
				if (m_MaxValue != value)
				{
					m_MaxValue = value;
					OnPropertyChanged(nameof(MaxValue));
				}
			}
		}

		public override void ResetFilter()
		{
			MinValue = m_MinDefaultValue;
			MaxValue = m_MaxDefaultValue;
			IsActive = false;
		}

		private RelayCommand m_ActivateCommand;
		public RelayCommand ActivateCommand
		{
			get
			{
				return m_ActivateCommand ??
					  (m_ActivateCommand = new RelayCommand(Activate));
			}
			set
			{
				if (m_ActivateCommand != value)
				{
					m_ActivateCommand = value;
					OnPropertyChanged(nameof(ActivateCommand));
				}
			}
		}

		private void Activate(object o)
		{
			IsActive = true;
		}

		private RelayCommand m_DeactivateCommand;
		public RelayCommand DeactivateCommand
		{
			get
			{
				return m_DeactivateCommand ??
					  (m_DeactivateCommand = new RelayCommand(Deactivate));
			}
			set
			{
				if (m_DeactivateCommand != value)
				{
					m_DeactivateCommand = value;
					OnPropertyChanged(nameof(DeactivateCommand));
				}
			}
		}

		private void Deactivate(object o)
		{
			IsActive = false;
		}

		private RelayCommand m_ResetOnlyCommand;
		public RelayCommand ResetOnlyCommand
		{
			get
			{
				return m_ResetOnlyCommand ??
					  (m_ResetOnlyCommand = new RelayCommand(ResetOnly));
			}
			set
			{
				if (m_ResetOnlyCommand != value)
				{
					m_ResetOnlyCommand = value;
					OnPropertyChanged(nameof(ResetOnlyCommand));
				}
			}
		}

		private void ResetOnly(object o)
		{
			MinValue = m_MinDefaultValue;
			MaxValue = m_MaxDefaultValue;
		
		}

	}

}
