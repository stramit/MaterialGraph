using UnityEngine;
using UnityEditor.Experimental;
using UnityEditorInternal.Experimental;
using System.Collections.Generic;


namespace UnityEditor.MaterialGraph
{
    internal class DraggableCommentBox : Draggable
    {

        public override bool StartDrag(CanvasElement element, Event e, Canvas2D canvas)
        {
            Debug.Log("Drag started");
            if (element is DrawableCommentBox)
            {
                List<CanvasElement> nodesInsideBox = new List<CanvasElement>();
                DrawableCommentBox dbox = (DrawableCommentBox)element;
                foreach (var ce in canvas.elements)
                {
                    if (RectUtils.Contains(dbox.m_CommentBox.m_Rect, ce.boundingRect) && ce is DrawableMaterialNode)
                    {
                        nodesInsideBox.Add(ce);
                    }
                }
                dbox.UpdateContainedNodesList(nodesInsideBox);
            }
            return base.StartDrag(element, e, canvas);
        }


    }
}

