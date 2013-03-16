namespace Dixie.Presentation
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			model = new DixieModel();
			presentationEngine = new DixiePresentationEngine(model);
			InitializeComponent();
			DataContext = model;

			presentationEngine.Start();
		}

		private readonly DixieModel model;
		private readonly DixiePresentationEngine presentationEngine;
	}
}
