namespace Dixie.Presentation
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			model = new DixieModel();
			InitializeComponent();
			DataContext = model;
			topologyGraphLayout.SetupLayoutParameters();
			presentationEngine = new DixiePresentationEngine(model);
			presentationEngine.Start();
		}

		private readonly DixieModel model;
		private readonly DixiePresentationEngine presentationEngine;
	}
}
