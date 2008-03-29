using System;
using System.Windows.Forms.Design;

namespace Trustbridge.Windows.Controls.Design
{
	public class BevelLineDesigner : System.Windows.Forms.Design.ControlDesigner
	{
		public BevelLineDesigner()
		{
			
		}
		public override SelectionRules SelectionRules
		{
			get
			{
				SelectionRules rules;

				rules = base.SelectionRules;
				
				// If not using VS.net 2005 Beta 2 then uncomment this code to have
				// selective grip handles on the BevelLine control in the Designer
				//if (((BevelLine)base.Control).Orientation == System.Windows.Forms.Orientation.Horizontal)
				//{
				//    rules = SelectionRules.Moveable | SelectionRules.Visible
				//        | SelectionRules.LeftSizeable | SelectionRules.RightSizeable;
				//}
				//else
				//{
				//    rules = SelectionRules.Moveable | SelectionRules.Visible
				//        | SelectionRules.TopSizeable | SelectionRules.BottomSizeable;
				//}
			
				return rules;
			}
		}
	}
}
