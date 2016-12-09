using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RMGUI.GraphView
{
	[Serializable]
	class NodePresenter : SimpleElementPresenter
	{
		[SerializeField]
		protected List<NodeAnchorPresenter> m_InputAnchors;
		public List<NodeAnchorPresenter> inputAnchors
 		{
 			get { return m_InputAnchors ?? (m_InputAnchors = new List<NodeAnchorPresenter>()); }
 		}

		[SerializeField]
		protected List<NodeAnchorPresenter> m_OutputAnchors;
		public List<NodeAnchorPresenter> outputAnchors
		{
			get { return m_OutputAnchors ?? (m_OutputAnchors = new List<NodeAnchorPresenter>()); }
		}

		[SerializeField]
		protected Orientation m_Orientation;
		public virtual Orientation orientation
		{
			get { return m_Orientation; }
			set { m_Orientation = value; }
		}

		// TODO make a simple creation function
		protected new void OnEnable()
		{
			base.OnEnable();

			capabilities |= Capabilities.Deletable;
		}

		protected NodePresenter()
		{
			m_Orientation = Orientation.Horizontal;
		}
	}
}
