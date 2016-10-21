﻿using System.Collections;
using ReactNative.UIManager;
using ReactNative.UIManager.Annotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ReactNative.Views.Text
{
    /// <summary>
    /// The view manager for text views.
    /// </summary>
    public class ReactTextViewManager : ViewParentManager<RichTextBox, ReactTextShadowNode>
    {
        private static readonly IReactCompoundView s_compoundView = new ReactTextCompoundView();
        private const double DefaultFontSize = 15;

        /// <summary>
        /// The name of the view manager.
        /// </summary>
        public override string Name
        {
            get
            {
                return "RCTText";
            }
        }

        /// <summary>
        /// Sets the font color for the node.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="color">The masked color value.</param>
        [ReactProp(ViewProps.Color, CustomType = "Color")]
        public void SetColor(RichTextBox view, uint? color)
        {
            view.Foreground = color.HasValue
                ? new SolidColorBrush(ColorHelpers.Parse(color.Value))
                : null;
        }

        /// <summary>
        /// Sets whether or not the text is selectable.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="selectable">A flag indicating whether or not the text is selectable.</param>
        [ReactProp("selectable")]
        public void SetSelectable(RichTextBox view, bool selectable)
        {
            // ToDo: Manually control selectable
        }

        /// <summary>
        /// Adds a child at the given index.
        /// </summary>
        /// <param name="parent">The parent view.</param>
        /// <param name="child">The child view.</param>
        /// <param name="index">The index.</param>
        public override void AddView(RichTextBox parent, DependencyObject child, int index)
        {
            var inlineChild = child as Inline;
            if (inlineChild == null)
            {
                inlineChild = new InlineUIContainer
                {
                    Child = (UIElement)child,
                };
            }

            ((IList) parent.Document.Blocks.OfType<Paragraph>().First().Inlines).Insert(index, inlineChild);
        }

        /// <summary>
        /// Creates the shadow node instance.
        /// </summary>
        /// <returns>The shadow node instance.</returns>
        public override ReactTextShadowNode CreateShadowNodeInstance()
        {
            return new ReactTextShadowNode();
        }

        /// <summary>
        /// Gets the child at the given index.
        /// </summary>
        /// <param name="parent">The parent view.</param>
        /// <param name="index">The index.</param>
        /// <returns>The child view.</returns>
        public override DependencyObject GetChildAt(RichTextBox parent, int index)
        {
            var child = ((IList) parent.Document.Blocks.OfType<Paragraph>().First().Inlines)[index];
            var childInlineContainer = child as InlineUIContainer;
            if (childInlineContainer != null)
            {
                return childInlineContainer.Child;
            }
            else
            {
                return (DependencyObject)child;
            }
        }

        /// <summary>
        /// Gets the number of children in the view parent.
        /// </summary>
        /// <param name="parent">The view parent.</param>
        /// <returns>The number of children.</returns>
        public override int GetChildCount(RichTextBox parent)
        {
            return parent.Document.Blocks.OfType<Paragraph>().First().Inlines.Count;
        }

        /// <summary>
        /// Removes all children from the view parent.
        /// </summary>
        /// <param name="parent">The view parent.</param>
        public override void RemoveAllChildren(RichTextBox parent)
        {
            var inlines = parent.Document.Blocks.OfType<Paragraph>().First().Inlines;
            inlines.Clear();
        }

        /// <summary>
        /// Removes the child at the given index.
        /// </summary>
        /// <param name="parent">The view parent.</param>
        /// <param name="index">The index.</param>
        public override void RemoveChildAt(RichTextBox parent, int index)
        {
            var inlines = ((IList) parent.Document.Blocks.OfType<Paragraph>().First().Inlines);
            inlines.RemoveAt(index);
        }

        /// <summary>
        /// Receive extra updates from the shadow node.
        /// </summary>
        /// <param name="root">The root view.</param>
        /// <param name="extraData">The extra data.</param>
        public override void UpdateExtraData(RichTextBox root, object extraData)
        {
            base.UpdateExtraData(root, extraData);

            var textNode = extraData as ReactTextShadowNode;
            if (textNode != null)
            {
                textNode.UpdateTextBlock(root);
            }
        }

        /// <summary>
        /// Creates the view instance.
        /// </summary>
        /// <param name="reactContext">The React context.</param>
        /// <returns>The view instance.</returns>
        protected override RichTextBox CreateViewInstance(ThemedReactContext reactContext)
        {
            var richTextBlock = new RichTextBox
            {
                //IsTextSelectionEnabled = false,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                //TextTrimming = TextTrimming.CharacterEllipsis,
            };

            richTextBlock.Document = new FlowDocument();
            richTextBlock.Document.Blocks.Add(new Paragraph());
            richTextBlock.SetReactCompoundView(s_compoundView);

            return richTextBlock;
        }
    }
}
