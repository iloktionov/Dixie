using System.ComponentModel;

namespace Dixie.Presentation
{
	internal class DixieModel : INotifyPropertyChanged
	{
		public DixieGraph TopologyGraph
		{
			get { return topologyGraph; }
			set
			{
				topologyGraph = value;
				RaisePropertyChanged("TopologyGraph");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged(string property)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}

		private DixieGraph topologyGraph;
	}
}