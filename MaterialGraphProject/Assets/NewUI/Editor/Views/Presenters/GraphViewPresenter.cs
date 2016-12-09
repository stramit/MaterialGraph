using System;
using System.Collections.Generic;
using UnityEngine;

namespace RMGUI.GraphView
{
	[Serializable]
	public abstract class GraphViewPresenter : ScriptableObject
	{
		[SerializeField]
		private List<GraphElementPresenter> m_Elements = new List<GraphElementPresenter>();

		public IEnumerable<GraphElementPresenter> elements
		{
			get { return m_Elements; }
		}

		public void AddElement(GraphElementPresenter element)
		{
			m_Elements.Add(element);
		}

		public void RemoveElement(GraphElementPresenter element)
		{
			m_Elements.RemoveAll(x => x == element);
		}

		protected void OnEnable()
		{
			m_Elements.Clear();
		}
	}
}
