using UnityEngine.RMGUI;

namespace RMGUI.GraphView.Demo
{
	class Node : SimpleElement
	{
		readonly VisualContainer m_InputContainer;
		readonly VisualContainer m_OutputContainer;

		public override void OnDataChanged()
		{
			base.OnDataChanged();

			m_OutputContainer.ClearChildren();
			m_InputContainer.ClearChildren();

			var nodeData = GetData<NodeData>();

			if (nodeData != null)
			{
				foreach (var anchorData in nodeData.anchors)
				{
					m_InputContainer.AddChild(new NodeAnchor(anchorData));
				}
				m_OutputContainer.AddChild(new NodeAnchor(nodeData.outputAnchor));
			}

			if (!classList.Contains("vertical") && !classList.Contains("horizontal"))
			{
				if (nodeData is VerticalNodeData)
				{
					AddToClassList("vertical");
				}
				else
				{
					AddToClassList("horizontal");
				}
			}
		}

		public Node()
		{
			m_InputContainer = new VisualContainer
			{
				name = "input", // for USS&Flexbox
				pickingMode = PickingMode.Ignore,
			};
			m_OutputContainer = new VisualContainer
			{
				name = "output", // for USS&Flexbox
				pickingMode = PickingMode.Ignore,
			};

			AddChild(m_InputContainer);
			AddChild(m_OutputContainer);
		}
	}
}