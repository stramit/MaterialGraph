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
            Debug.Log("Dragging box");
            if (element is DrawableCommentBox)
            {
                List<CanvasElement> nodesInsideBox = new List<CanvasElement>();
                DrawableCommentBox dbox = (DrawableCommentBox)element;
                foreach (var ce in canvas.elements)
                {
                    if (RectUtils.Contains(dbox.m_CommentBox.m_Rect, ce.boundingRect) && ce is DrawableMaterialNode)
                    {
                        if(RectUtils.Contains(canvas.MouseToCanvas(e.mousePosition), ce.boundingRect))
                        {
                            Debug.Log("Drag started in other node");
                            dbox.UpdateContainedNodesList(new List<CanvasElement>());
                            return base.StartDrag(ce, e, canvas);
                            //if (!ce.selected)
                            //    nodesInsideBox.Add(ce);
                        }
                        else
                        {
                            if (!ce.selected)
                                nodesInsideBox.Add(ce);
                        }
                    }
                }
                dbox.UpdateContainedNodesList(nodesInsideBox);
            }
            return base.StartDrag(element, e, canvas);
        }


    }
}

