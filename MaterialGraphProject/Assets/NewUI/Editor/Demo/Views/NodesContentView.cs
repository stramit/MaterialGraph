using UnityEditor;
using UnityEngine;
using UnityEngine.RMGUI;

namespace RMGUI.GraphView.Demo
{
	[StyleSheet("Assets/Editor/Demo/Views/NodalView.uss")]
	class NodesContentView : SimpleContentView
	{
		private readonly System.Random rnd = new System.Random();

		public NodesContentView()
		{
			// Contextual menu to create new nodes
			AddManipulator(new ContextualMenu((evt, customData) =>
			{
				var menu = new GenericMenu();
				menu.AddItem(new GUIContent("Create Operator"), false,
							 contentView => CreateOperator(),
							 this);
				menu.ShowAsContext();
				return EventPropagation.Continue;
			}));

			dataMapper[typeof(CustomEdgePresenter)] = typeof(CustomEdge);
			dataMapper[typeof(NodeAnchorPresenter)] = typeof(NodeAnchor);
			dataMapper[typeof(NodePresenter)] = typeof(Node);
			dataMapper[typeof(VerticalNodePresenter)] = typeof(Node);
		}

		public void CreateOperator()
		{
			var presenter = ((GraphView) this).presenter as NodesContentViewPresenter;
			if (presenter != null)
			{
				var x = rnd.Next(0, 600);
				var y = rnd.Next(0, 300);

				presenter.CreateOperator(typeof(Vector3), new Rect(x, y, 200, 176), "Shiny New Operator");
			}
		}
	}
}
