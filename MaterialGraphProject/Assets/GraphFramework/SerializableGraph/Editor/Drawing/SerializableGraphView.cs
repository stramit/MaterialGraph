using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RMGUI.GraphView;
using UnityEngine;
using UnityEngine.RMGUI;

namespace UnityEditor.Graphing.Drawing
{
    // TODO JOCE Maybe bring SimpleGraphView public. This implements pretty much all that it does.
    [StyleSheet("Assets/GraphFramework/SerializableGraph/Editor/Drawing/Styles/SerializableGraph.uss")]
    public class SerializableGraphView : GraphView
    {
        public SerializableGraphView()
        {
            // Shortcut handler to delete elements
            AddManipulator(new ShortcutHandler(
                new Dictionary<Event, ShortcutDelegate>
                {
                    {Event.KeyboardEvent("a"), FrameAll},
                    {Event.KeyboardEvent("f"), FrameSelection},
                    {Event.KeyboardEvent("o"), FrameOrigin},
                    {Event.KeyboardEvent("delete"), DeleteSelection},
                    {Event.KeyboardEvent("#tab"), FramePrev},
                    {Event.KeyboardEvent("tab"), FrameNext}
                }));

            AddManipulator(new ContentZoomer());
            AddManipulator(new ContentDragger());
            AddManipulator(new RectangleSelector());
            AddManipulator(new SelectionDragger());
            AddManipulator(new ClickSelector());

            InsertChild(0, new GridBackground());

            dataMapper[typeof(NodeDrawData)] = typeof(NodeDrawer);
        }

        public override void DoRepaint(IStylePainter painter)
        {
            int joce = 0;
            joce++;
            base.DoRepaint(painter);
            Trace.WriteLine(joce);
        }

        // TODO JOCE Remove the "new" here. Use the base class' impl
        private new EventPropagation DeleteSelection()
        {
            var nodalViewData = GetPresenter<AbstractGraphDataSource>();
            if (nodalViewData == null)
                return EventPropagation.Stop;

            nodalViewData.RemoveElements(
                selection.OfType<NodeDrawer>().Select(x => x.GetPresenter<NodeDrawData>()),
                selection.OfType<Edge>().Select(x => x.GetPresenter<EdgeDrawData>())
                );

            return EventPropagation.Stop;
        }
    }
}
