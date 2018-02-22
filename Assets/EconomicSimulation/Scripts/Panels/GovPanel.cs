using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using Nashet.UnityUIUtils;
namespace Nashet.EconomicSimulation

{
	public class GovPanel : DragPanel
	{
		[SerializeField]
		private Text govTitle;
	
	// Use this for initialization
	void Start ()
		{
			MainCamera.govPanel = this;
			GetComponent<RectTransform> ().anchoredPosition = new Vector2 (150f, -150f);
			Hide ();
		}
		public override void Refresh()
		{
			var sbg = new StringBuilder ();
			sbg.Append ("You Rule: ").Append (Game.Player.GetFullName ());
			if (Game.Player.isAlive ())
				govTitle.text = sbg.ToString ();
			
		}
}
}