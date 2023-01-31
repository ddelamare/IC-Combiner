using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Combiner
{
	/// <summary>
	/// Interaction logic for NewCreatureView.xaml
	/// </summary>
	public partial class CreatureDataView : UserControl
	{
		public CreatureDataView()
		{
			InitializeComponent();
		}

		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
		{
			var member = e.Column.SortMemberPath;
			var direction = e.Column.SortDirection;

			var dg = sender as DataGrid;
			var creatureView = dg.DataContext as CreatureDataVM;
			

			creatureView.Parent.FiltersVM.Sort(member, direction);
			e.Handled = true;
		}
	}
}
