using UnityEditor.Experimental;
using UnityEngine;

namespace UnityEditor.MaterialGraph
{
    public class DrawableCommentBox : CanvasElement
    {
        protected string m_Title = "CommentBox";

        public DrawableCommentBox(Rect box, string title)
        {
            translation = new Vector2(box.x, box.y);
            scale = new Vector2(box.width, box.height);
            AddManipulator(new Draggable());
            AddManipulator(new Resizable());
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
    }
    
}
