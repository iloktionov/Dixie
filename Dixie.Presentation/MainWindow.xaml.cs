namespace Dixie.Presentation
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = model;
		}

		private readonly DixieModel model = new DixieModel();
	}
}
