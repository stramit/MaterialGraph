#if false

using System;
using RMGUI.GraphView;
using UnityEngine;
using UnityEngine.RMGUI;
using UnityEngine.RMGUI.StyleSheets;

namespace UnityEditor.Graphing.Drawing
{
	internal class NodeAnchor : GraphElement
	{
		public const float k_NodeSize = 15.0f;

		private readonly EdgeConnector<EdgePresenter> m_RegularConnector = new EdgeConnector<EdgePresenter>();
		private readonly EdgeConnector<EdgePresenter> m_CustomConnector = new EdgeConnector<EdgePresenter>();

		private IManipulator m_CurrentConnector;

		VisualElement m_ConnectorBox;
		VisualElement m_ConnectorText;

		public NodeAnchor(NodeAnchorPresenter data)
		{
			// currently we don't want to be styled as .graphElement since we're contained in a Node
            classList = new ClassList("NodeAnchor");

			m_CurrentConnector = m_RegularConnector;
			AddManipulator(m_CurrentConnector);

			m_ConnectorBox = new VisualElement() { name = "connector", width = k_NodeSize, height = k_NodeSize };
			m_ConnectorBox.pickingMode = PickingMode.Ignore;
			AddChild(m_ConnectorBox);

			m_ConnectorText = new VisualElement() { name = "type" };
			m_ConnectorText.content = new GUIContent();
			m_ConnectorText.pickingMode = PickingMode.Ignore;
			AddChild(m_ConnectorText);

			dataProvider = data;
		}

		private void UpdateConnector()
		{
			var nodeAnchorData = GetData<NodeAnchorPresenter>();
			if (nodeAnchorData == null)
				return;

			RemoveManipulator(m_CurrentConnector);
			if (!nodeAnchorData.connected || nodeAnchorData.direction != Direction.Input)
			{
				if (nodeAnchorData.orientation == Orientation.Horizontal)
				{
					m_CurrentConnector = m_RegularConnector;
				}
				else
				{
					m_CurrentConnector = m_CustomConnector;
				}
				AddManipulator(m_CurrentConnector);
			}
		}

		public override void OnDataChanged()
		{
			UpdateConnector();

			var nodeAnchorData = GetData<NodeAnchorPresenter>();
			if (nodeAnchorData == null)
				return;

			Type type = nodeAnchorData.type;

			Type genericClass = typeof(PortSource<>);
			Type constructedClass = genericClass.MakeGenericType(type);
			nodeAnchorData.source = Activator.CreateInstance(constructedClass);

			if (nodeAnchorData.highlight)
			{
				m_ConnectorBox.AddToClassList("anchorHighlight");
			}
			else
			{
				m_ConnectorBox.RemoveFromClassList("anchorHighlight");
			}

			string anchorName = string.IsNullOrEmpty(nodeAnchorData.name) ? type.Name : nodeAnchorData.name;
			m_ConnectorText.content.text = anchorName;
			GetData<NodeAnchorPresenter>().capabilities &= ~Capabilities.Selectable;
		}

		public override Vector3 GetGlobalCenter()
		{
			var center = m_ConnectorBox.position.center;
			center = m_ConnectorBox.transform.MultiplyPoint3x4(center);
			return this.LocalToGlobal(center);
		}

		public override bool ContainsPoint(Vector2 localPoint)
		{
			// Here local point comes without position offset...
			localPoint -= position.position;
			return m_ConnectorBox.ContainsPoint(m_ConnectorBox.transform.MultiplyPoint3x4(localPoint));
		}
	}
}

#endif
