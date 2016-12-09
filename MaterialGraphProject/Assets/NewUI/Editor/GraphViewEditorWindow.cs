using UnityEngine;
using UnityEditor;
using UnityEngine.RMGUI;

namespace RMGUI.GraphView
{
	public abstract class GraphViewEditorWindow : EditorWindow
	{
		GraphView m_View;
		GraphViewPresenter m_Presenter;

		// we watch the data source for destruction and re-create it
		IDataWatchHandle handle;

		void OnEnable()
		{
			m_Presenter = BuildPresenters();
			m_View = BuildView();
			m_View.name = "theView";
			m_View.presenter = m_Presenter;
			m_View.StretchToParentSize();
			m_View.onEnter += OnEnterPanel;
			m_View.onLeave += OnLeavePanel;
			rootVisualContainer.AddChild(m_View);
		}

		void OnDisable()
		{
			rootVisualContainer.RemoveChild(m_View);
		}

		// Override these methods to properly support domain reload & enter/exit playmode
		protected abstract GraphView BuildView();
		protected abstract GraphViewPresenter BuildPresenters();

		void OnEnterPanel()
		{
			if (m_Presenter == null)
			{
				m_Presenter = BuildPresenters();
				m_View.presenter = m_Presenter;
			}
			handle = m_View.panel.dataWatch.AddWatch(m_View, m_Presenter, OnChanged);
		}

		void OnLeavePanel()
		{
			if (handle != null)
			{
				handle.Dispose();
				handle = null;
			}
			else
			{
				Debug.LogError("No active handle to remove");
			}
		}

		void OnChanged()
		{
			// If data was destroyed, remove the watch and try to re-create it
			if (m_Presenter == null && m_View.panel != null)
			{
				if (handle != null)
				{
					handle.Dispose();
				}

				m_Presenter = BuildPresenters();
				m_View.presenter = m_Presenter;
				handle = m_View.panel.dataWatch.AddWatch(m_View, m_Presenter, OnChanged);
			}
		}
	}
}
