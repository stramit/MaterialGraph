using System;
using System.Linq;
using UnityEngine;
using UnityEngine.RMGUI;

namespace RMGUI.GraphView
{
	public class SelectionDragger : Dragger
	{
		public SelectionDragger()
		{
			activateButtons[(int)MouseButton.LeftMouse] = true;
			panSpeed = new Vector2(1, 1);
			clampToParentEdges = false;
		}

		public override EventPropagation HandleEvent(Event evt, VisualElement finalTarget)
		{
			if (finalTarget == target && !this.HasCapture())
				return EventPropagation.Continue;

			var graphView = target as GraphView;
			if (graphView == null)
			{
				throw new InvalidOperationException("Manipulator can only be added to a GraphView");
			}

			switch (evt.type)
			{
				case EventType.MouseDown:
					if (CanStartManipulation(evt))
					{
						// avoid starting a manipulation on a non movable object
						var ce = finalTarget as GraphElement;
						if (ce == null)
						{
							ce = finalTarget.GetFirstAncestorOfType<GraphElement>();
							if (ce == null)
								return EventPropagation.Continue;
						}

						GraphElementPresenter presenter = ce.presenter;
						if (presenter != null && ((ce.presenter.capabilities & Capabilities.Movable) != Capabilities.Movable))
							return EventPropagation.Continue;

						this.TakeCapture();
						return EventPropagation.Stop;
					}
					break;

				case EventType.MouseDrag:
					if (this.HasCapture())
					{
						foreach (ISelectable s in graphView.selection)
						{
							GraphElement ce = s as GraphElement;
							if (ce == null || ce.presenter == null)
								continue;

							GraphElementPresenter presenter = ce.presenter;
							if ((ce.presenter.capabilities & Capabilities.Movable) != Capabilities.Movable)
								continue;

							Matrix4x4 g = ce.globalTransform;
							var scale = new Vector3(g.m00, g.m11, g.m22);

							presenter.position = CalculatePosition(presenter.position.x + evt.delta.x * panSpeed.x / scale.x,
																   presenter.position.y + evt.delta.y * panSpeed.y / scale.y,
																   presenter.position.width, presenter.position.height);
						}

						return EventPropagation.Stop;
					}
					break;

				case EventType.MouseUp:
					if (CanStopManipulation(evt))
					{
						foreach (var presenter in graphView.selection.OfType<GraphElement>()
																	 .Select(ge => ge.presenter as NodePresenter)
																	 .Where(pr => pr != null))
						{
							presenter.CommitChanges();
						}
						this.ReleaseCapture();
						return EventPropagation.Stop;
					}
					break;
			}
			return EventPropagation.Continue;
		}
	}
}
