using System;
using System.Collections.Generic;
using UnityEngine;

namespace RMGUI.GraphView
{
	[Serializable]
	public abstract class GraphViewPresenter : ScriptableObject
	{
		[SerializeField]
		// TODO TEMP protected while upgrading MaterialGraph. Needs to go back private
		protected List<GraphElementPresenter> m_Elements = new List<GraphElementPresenter>();

		// TODO TEMP Revert to non virtual
		public virtual IEnumerable<GraphElementPresenter> elements
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
