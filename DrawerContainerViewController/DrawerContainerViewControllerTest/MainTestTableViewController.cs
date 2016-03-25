using System;

namespace DrawerContainerViewControllerTest
{
	public partial class MainTestTableViewController : TestTableViewController
	{
		public MainTestTableViewController (IntPtr handle) : base (handle) { }

		public override string ReuseId => "MainTestTableViewCell";
	}
}