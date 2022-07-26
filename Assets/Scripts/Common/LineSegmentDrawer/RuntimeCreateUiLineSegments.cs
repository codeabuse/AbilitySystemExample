using UnityEngine;
using UnityEngine.UIElements;

public class RuntimeCreateUiLineSegments : MonoBehaviour
{
	[SerializeField] UIDocument _uiDocument = null;
	void OnEnable ()
	{
		var ROOT = _uiDocument.rootVisualElement;

		var LINE1 = new LineSegmentDrawer( new Vector2(0,0) , new Vector2(3,1) , 10 )
		{
				style =
				{
						position = new StyleEnum<Position>(Position.Absolute),
						width = 100,
						height = 100,
						color = Color.magenta
				}
		};
		ROOT.Add( LINE1 );

		var LINE2 = new LineSegmentDrawer( new Vector2(2,4) , new Vector2(8,8) , 15 )
		{
				style =
				{
						position = new StyleEnum<Position>(Position.Absolute),
						width = 100,
						height = 100,
						color = Color.magenta
				}
		};
		ROOT.Add( LINE2 );
	}
}
