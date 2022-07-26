// src* https://gist.github.com/andrew-raphael-lukasik/7229d38dbbc0baa3c2a2ab106fb4f7ef
using UnityEngine;
using UnityEngine.UIElements;

[UnityEngine.Scripting.Preserve]
public class LineSegmentDrawer : VisualElement
{
	public float startX { get; set; }
	public float startY { get; set; }
	public float endX { get; set; }
	public float endY { get; set; }
	public float lineWidth { get; set; }

	private Vertex[] _vertices = new Vertex[4];
	private Color _color;
	
	private static ushort[] _indicies = { 0,1,2, 0,2,3 };
	
	public new class UxmlFactory : UxmlFactory<LineSegmentDrawer,UxmlTraits> {}
	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		UxmlFloatAttributeDescription lineWidthAttr = new UxmlFloatAttributeDescription{ name="line-width" , defaultValue=4 , restriction = new UxmlValueBounds{ min="0" } };
		UxmlFloatAttributeDescription startXAttr = new UxmlFloatAttributeDescription{ name="start-x" , defaultValue=0 };
		UxmlFloatAttributeDescription startYAttr = new UxmlFloatAttributeDescription{ name="start-y" , defaultValue=1 };
		UxmlFloatAttributeDescription endXAttr = new UxmlFloatAttributeDescription{ name="end-x" , defaultValue=1 };
		UxmlFloatAttributeDescription endYAttr = new UxmlFloatAttributeDescription{ name="end-y" , defaultValue=0 };
		public override void Init ( VisualElement ve , IUxmlAttributes attributes , CreationContext context )
		{
			base.Init( ve , attributes , context );

			var instance = ve as LineSegmentDrawer;
			instance.startX = startXAttr.GetValueFromBag( attributes , context );
			instance.startY = startYAttr.GetValueFromBag( attributes , context );
			instance.endX = endXAttr.GetValueFromBag( attributes , context );
			instance.endY = endYAttr.GetValueFromBag( attributes , context );
			instance.lineWidth = lineWidthAttr.GetValueFromBag( attributes , context );
		}
	}

	public LineSegmentDrawer(Vector3 start, Vector3 end, float width, Color color) : this((Vector2)start, (Vector2)end, width, color) { }

	public LineSegmentDrawer(Vector2 start, Vector2 end, float width, Color color) : this(start, end, width)
	{
		_color = color;
	}

	public LineSegmentDrawer(Vector3 start, Vector3 end, float width) : this((Vector2)start, (Vector2)end, width) { }

	public LineSegmentDrawer ( Vector2 start , Vector2 end , float width )
		: this()
	{
		this.startX = start.x;
		this.startY = start.y;
		this.endX = end.x;
		this.endY = end.y;
		this.lineWidth = width;
	}
	public LineSegmentDrawer () => this.generateVisualContent += OnGenerateVisualContent;
	private void OnGenerateVisualContent (MeshGenerationContext mgc)
	{
		float width = this.resolvedStyle.width;
		float height = this.resolvedStyle.height;
		Vector2 p0 = new Vector2{ x=endX , y=endY } * new Vector2{ x=width , y=height };
		Vector2 p1 = new Vector2{ x=startX , y=startY } * new Vector2{ x=width , y=height };
		Vector2 dir = ( p0 - p1 ).normalized;
		Vector2 ortho = -Vector3.Cross( Vector3.forward , dir );
		Vector2 orthoOffset = ortho * ( lineWidth*0.5f );
		var tint = this.resolvedStyle.color;
		
		_vertices[0] = new Vertex{// bottom left
			position =  new Vector3{ z=Vertex.nearZ } + (Vector3)( p1 - orthoOffset ) ,
			tint = tint ,
			uv = new Vector2{ x=0 , y=0 }
		};
		_vertices[1] = new Vertex{// top left
			position =  new Vector3{ z=Vertex.nearZ } + (Vector3)( p1 + orthoOffset ) ,
			tint = tint ,
			uv = new Vector2{ x=0 , y=1 }
		};
		_vertices[2] = new Vertex{// top right
			position = new Vector3{ z=Vertex.nearZ } + (Vector3)( p0 + orthoOffset ) ,
			tint = tint ,
			uv = new Vector2{ x=1 , y=1 }
		};
		_vertices[3] = new Vertex{// bottom right
			position =  new Vector3{ z=Vertex.nearZ } + (Vector3)( p0 - orthoOffset ) ,
			tint = tint ,
			uv = new Vector2{ x=0 , y=1 }
		};

		var mesh = mgc.Allocate( 4 , 6 );
		mesh.SetAllVertices( _vertices );
		mesh.SetAllIndices( _indicies );
	}
}
