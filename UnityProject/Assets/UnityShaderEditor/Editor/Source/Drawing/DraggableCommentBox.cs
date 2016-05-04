using UnityEngine;
using UnityEditor.Experimental;
using UnityEditorInternal.Experimental;
using System.Collections.Generic;


namespace UnityEditor.MaterialGraph
{
    internal class DraggableCommentBox : Draggable
    {

        // If on, nodes are only dragged with the box if commenbox surrounds them fully
        // If off, nodes are dragged with the commentbox even if they only partially intersect the commentbox
        private const bool strictEncompass = true;

        public override bool StartDrag(CanvasElement element, Event e, Canvas2D canvas)
        {
            bool doStartDrag = true;
            if (element is DrawableCommentBox)
            {
                List<CanvasElement> nodesInsideBox = new List<CanvasElement>();
                DrawableCommentBox dbox = (DrawableCommentBox)element;
                List<CanvasElement> elementsInCoveredregion = canvas.FindInRegion(dbox.boundingRect);
                foreach (var ce in elementsInCoveredregion)
                {
                    if (ce == dbox)
                    {
                        continue;
                    }

                    Debug.Log("ZIndex of node " + ce.zIndex);

                    if (RectUtils.Contains(dbox.boundingRect, ce.boundingRect) || !strictEncompass)
                    {
                        if (RectUtils.Contains(canvas.MouseToCanvas(e.mousePosition), ce.boundingRect))
                        {
                            doStartDrag = false;
                        }
                        nodesInsideBox.Add(ce);
                    }
                }
                dbox.UpdateContainedNodesList(nodesInsideBox);
            }
            if (doStartDrag) return base.StartDrag(element, e, canvas);
            else return false;
        }

        public override bool EndDrag(CanvasElement element, Event e, Canvas2D canvas)
        {
            if (element is DrawableCommentBox)
            {
                DrawableCommentBox dbox = (DrawableCommentBox)element;
                dbox.ClearContainingNodesList();
            }
            return base.EndDrag(element, e, canvas);
        }


    }
}

