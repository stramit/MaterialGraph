using UnityEditor.Experimental;
using UnityEngine;
using System.Collections.Generic;

namespace UnityEditor.MaterialGraph
{
    public class DrawableCommentBox : CanvasElement
    {

        public delegate void MovementEventHandler(List<CanvasElement> elements, Vector2 movement);

        public static event MovementEventHandler onMove;

        public static void registerCommentBoxMoveEvent(MovementEventHandler function)
        {
            if (onMove == null)
            {
                DrawableCommentBox.onMove += function;
                return;
            }

            foreach (MovementEventHandler existingHandler in onMove.GetInvocationList())
            {
                if (existingHandler == function)
                {
                    return;
                }
            }

            onMove += function;
        }

        protected string m_Title = "CommentBox";

        public CommentBox m_CommentBox;

        private List<CanvasElement> m_ContainedNodes;
        public List<CanvasElement> containedNodes
        {
            get
            {
                if (m_ContainedNodes == null)
                {
                    m_ContainedNodes = new List<CanvasElement>();
                }
                return m_ContainedNodes;
            }
            set
            {
                m_ContainedNodes = value;
            }
        }

        public DrawableCommentBox(CommentBox box)
        {
            translation = new Vector2(box.m_Rect.x, box.m_Rect.y);
            scale = new Vector2(box.m_Rect.width, box.m_Rect.height);
            AddManipulator(new DraggableCommentBox());
            AddManipulator(new Resizable());

            m_CommentBox = box;
        }

        public override void Render(Rect parentRect, Canvas2D canvas)
        {
            Color backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.7f);
            Color selectedColor = new Color(0.0f, 0.7f, 1.0f, 0.7f);
            EditorGUI.DrawRect(new Rect(0, 0, scale.x, scale.y), selected ? selectedColor : backgroundColor);
            GUI.Label(new Rect(0, 0, scale.x, 26f), GUIContent.none, new GUIStyle("preToolbar"));
            GUI.Label(new Rect(10, 2, scale.x - 20.0f, 16.0f), m_Title, EditorStyles.toolbarTextField);

            base.Render(parentRect, canvas);
        }

        public void UpdateContainedNodesList(List<CanvasElement> elements)
        {
            containedNodes = elements;
        }

        public override void UpdateModel(UpdateType t)
        {
            base.UpdateModel(t);

            Vector2 oldTranslation = new Vector2(m_CommentBox.m_Rect.x, m_CommentBox.m_Rect.y);
            Rect oldFrame = new Rect(m_CommentBox.m_Rect.x, m_CommentBox.m_Rect.y, m_CommentBox.m_Rect.width, m_CommentBox.m_Rect.height);

            m_CommentBox.m_Rect.x      = translation.x;
            m_CommentBox.m_Rect.y      = translation.y;
            m_CommentBox.m_Rect.width  = scale.x;
            m_CommentBox.m_Rect.height = scale.y;

            Vector2 newTranslation = new Vector2(translation.x, translation.y);

            if (onMove != null)
                onMove(containedNodes, (newTranslation - oldTranslation));
        }
    }
    
}
