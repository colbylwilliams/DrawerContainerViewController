using UIKit;

namespace DrawerContainerViewController
{
	public class PanGestureRecognizerDelegate : UIGestureRecognizerDelegate
	{
		public override bool ShouldRecognizeSimultaneously (UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer) => true;
	}
}