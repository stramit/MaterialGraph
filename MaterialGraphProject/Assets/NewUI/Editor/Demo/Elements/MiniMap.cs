using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.RMGUI;

namespace RMGUI.GraphView.Demo
{
	public class MiniMap : GraphElement
	{
		private float m_PreviousContainerWidth = -1;
		private float m_PreviousContainerHeight = -1;

		private readonly Label m_Label;
		private Dragger m_Dragger;

		public MiniMap()
		{
			clipChildren = false;

			m_Label = new Label(new GUIContent("Floating Minimap"));
			AddChild(m_Label);

			AddManipulator(new ContextualMenu((evt, customData) =>
			{
				var boxPresenter = presenter as MiniMapPresenter;
				if (boxPresenter != null)
				{
					var menu = new GenericMenu();
					menu.AddItem(new GUIContent(boxPresenter.anchored ? "Make floating" :  "Anchor"), false,
						contentView =>
						{
							var bPresenter = presenter as MiniMapPresenter;
							if (bPresenter != null)
								bPresenter.anchored = !bPresenter.anchored;
						},
								 this);
					menu.ShowAsContext();
				}
				return EventPropagation.Continue;
			}));
		}

		public override void OnDataChanged()
		{
			base.OnDataChanged();
			AdjustAnchoring();
			Resize();
		}

		private void AdjustAnchoring()
		{
			var miniMapPresenter = presenter as MiniMapPresenter;
			if (miniMapPresenter == null)
			{
				return;
			}

			if (miniMapPresenter.anchored)
			{
				presenter.capabilities &= ~Capabilities.Movable;
				ResetPositionProperties();
				AddToClassList("anchored");
			}
			else
			{
				if (m_Dragger == null)
				{
					m_Dragger = new Dragger {clampToParentEdges = true};
					AddManipulator(m_Dragger);
				}
				presenter.capabilities |= Capabilities.Movable;
				RemoveFromClassList("anchored");
			}
		}

		private void Resize()
		{
			var miniMapPresenter = presenter as MiniMapPresenter;
			if (miniMapPresenter == null || parent == null)
 			{
				return;
			}

			if (parent.position.height > parent.position.width)
			{
				height = miniMapPresenter.maxHeight;
				width = parent.position.width * height / parent.position.height;
			}
			else
			{
				width = miniMapPresenter.maxWidth;
				height = parent.position.height * width / parent.position.width;
 			}

			// Relocate if partially visible on bottom or right side (left/top not checked, only bottom/right affected by a size change)
			if (positionLeft + width > parent.position.x + parent.position.width)
			{
				var newPosition = miniMapPresenter.position;
				newPosition.x -= positionLeft + width - (parent.position.x + parent.position.width);
				miniMapPresenter.position = newPosition;
			}

			if (positionTop + height > parent.position.y + parent.position.height)
			{
				var newPosition = miniMapPresenter.position;
				newPosition.y -= positionTop + height - (parent.position.y + parent.position.height);
				miniMapPresenter.position = newPosition;
			}

			var newMiniMapPos = miniMapPresenter.position;
			newMiniMapPos.width = width;
			newMiniMapPos.height = height;
			newMiniMapPos.x = Mathf.Max(parent.position.x, newMiniMapPos.x);
			newMiniMapPos.y = Mathf.Max(parent.position.y, newMiniMapPos.y);
			miniMapPresenter.position = newMiniMapPos;

			if (!miniMapPresenter.anchored)
			{
				// Update to prevent onscreen mishaps especially at tiny window sizes
				position = miniMapPresenter.position;
			}
		}

		public override void DoRepaint(IStylePainter painter)
		{
			var gView = this.GetFirstAncestorOfType<GraphView>();
			VisualContainer container = gView.contentViewContainer;

			Matrix4x4 containerTransform = container.globalTransform;
			Vector4 containerTranslation = containerTransform.GetColumn(3);
			var containerScale = new Vector2(containerTransform.m00, containerTransform.m11);
			Rect containerPosition = container.position;

			float containerWidth = parent.position.width / containerScale.x;
			float containerHeight = parent.position.height / containerScale.y;

			if ( (containerWidth != m_PreviousContainerWidth || containerHeight != m_PreviousContainerHeight) && presenter != null)
			{
				m_PreviousContainerWidth = containerWidth;
				m_PreviousContainerHeight = containerHeight;
				Resize();
			}

			m_Label.content.text = "Minimap p:" +
								   String.Format("{0:0}", containerPosition.position.x) + "," + String.Format("{0:0}", containerPosition.position.y) + " t: " +
								   String.Format("{0:0}", containerTranslation.x) + "," + String.Format("{0:0}", containerTranslation.y) + " s: " +
								   String.Format("{0:N2}", containerScale.x)/* + "," + String.Format("{0:N2}", containerScale.y)*/;

			base.DoRepaint(painter);

			foreach (var child in container.children)
			{
				// For some reason, I can't seem to be able to use Linq (IEnumerable.OfType() nor IEnumerable.Where appear to be working here. ??)
				var elem = child as GraphElement;

				// TODO: Should Edges be displayed at all?
				// TODO: Maybe edges need their own capabilities flag.
				if (elem == null || (elem.presenter.capabilities & Capabilities.Floating) != 0 || (elem.presenter is EdgePresenter))
				{
					continue;
				}

				var titleBarOffset = (int)paddingTop;
				Rect rect = child.position;

				rect.x /= containerWidth;
				rect.width /= containerWidth;
				rect.y /= containerHeight;
				rect.height /= containerHeight;

				rect.x *= position.width;
				rect.y *= position.height-titleBarOffset;
				rect.width *= position.width;
				rect.height *= position.height-titleBarOffset;

				rect.y += titleBarOffset;

				rect.x += position.x;
				rect.y += position.y;

				rect.x += containerTranslation.x * position.width / parent.position.width;
				rect.y += containerTranslation.y * (position.height-titleBarOffset) / parent.position.height;

				rect.x += containerPosition.x * position.width / containerWidth;
				rect.y += containerPosition.y * (position.height-titleBarOffset) / containerHeight;

				// Clip using a minimal 2 pixel wide frame around edges
				// (except yMin since we already have the titleBar offset which is enough for clipping)
				var xMin = position.xMin + 2;
				var xMax = position.xMax - 2;
				var yMax = position.yMax - 2;

				if (rect.x < xMin)
				{
					if (rect.x < xMin - rect.width)
					{
						continue;
					}
					rect.width -= xMin - rect.x;
					rect.x = xMin;
				}

				if (rect.x + rect.width >= xMax)
				{
					if (rect.x >= xMax)
					{
						continue;
					}
					rect.width -= rect.x + rect.width - xMax;
				}

				if (rect.y < position.yMin+titleBarOffset)
				{
					if (rect.y < position.yMin+titleBarOffset - rect.height)
					{
						continue;
					}
					rect.height -= position.yMin+titleBarOffset - rect.y;
					rect.y = position.yMin+titleBarOffset;
				}

				if (rect.y + rect.height >= yMax)
				{
					if (rect.y >= yMax)
					{
						continue;
					}
					rect.height -= rect.y + rect.height - yMax;
				}

				Handles.DrawSolidRectangleWithOutline(rect, Color.grey, Color.grey);
			}
		}
	}
}
