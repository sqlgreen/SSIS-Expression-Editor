//-------------------------------------------------------------------------------------------------
// <copyright file="MruComboBox.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.ExpressionEditor.Controls
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// ComboBox control with most recently used string list. Can be used in place of a TextBox control
    /// when memory of items previously entered is required. Ideal for use on Find dialogs or similar.
    /// </summary>
    /// <remarks>
    /// When the text value is retrieved through the <see cref="GetText"/> method, it is added to the list.
    /// The list can be pre-populated and manipulated in the usual manner through the <see cref="ComboBox.Items"/>
    /// collection but in normal usage this is not expected.
    /// </remarks>
    public class MruComboBox : ComboBox
    {
        /// <summary>
        /// Gets the text that is selected. Getting text adds it to the list. To set text, use the <see cref="MruComboBox.Text"/> property.
        /// </summary>
        public new string SelectedText
        {
            get { return this.Text; }
        }

        /// <summary>
        /// Gets the value of the text that is selected.
        /// </summary>
        /// <remarks>The <see cref="ComboBox.ValueMember"/> property is ignored by this method, as only string values are supported.</remarks>
        public new string SelectedValue
        {
            get { return this.Text; }
        }

        /// <summary>
        /// Gets the current text.
        /// </summary>
        /// <returns>The curent text.</returns>
        public string GetText()
        {
            this.AddListItem(this.Text);
            return this.Text;
        }

        /// <summary>
        /// Raises the <see cref="ComboBox.DropDown"/> event. Includes drop-down width sizing by <see cref="ComboBox.Items"/> strings.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnDropDown(EventArgs e)
        {
            Graphics graphics = Graphics.FromHwnd(Handle);
            int width = Width;

            for (int index = 0; index < Items.Count; index++)
            {
                SizeF size = graphics.MeasureString(Items[index].ToString(), Font);
                if (Convert.ToInt32(size.Width) > width)
                {
                    width = Convert.ToInt32(size.Width);
                }
            }

            if (Width < width)
            {
                DropDownWidth = width;
            }

            base.OnDropDown(e);
        }

        /// <summary>
        /// Manage the Items collection, the most recently used list.
        /// </summary>
        /// <param name="newItem">The item to add to the list.</param>
        /// <remarks>
        /// <para>Called from the <see cref="GetText"/> method.</para>
        /// New items will be added to the top of the list.
        /// If the <see cref="MruComboBox.MaxDropDownItems"/> limit has already been reached then the oldest entry will be discarded first.
        /// Existing items will be moved to the top of the list.
        /// </remarks>
        private void AddListItem(string newItem)
        {
            if (String.IsNullOrEmpty(newItem))
            {
                return;
            }

            Items.Remove(string.Empty);

            int index = Items.IndexOf(newItem);
            if (index == 0)
            {
                return;
            }
            else if (index > 0)
            {
                // Remove existing entry
                Items.RemoveAt(index);

                // Add back at top
                Items.Insert(0, newItem);
            }
            else
            {
                // Remove the last item to make space, if required
                if (MaxDropDownItems == Items.Count)
                {
                    Items.RemoveAt(MaxDropDownItems - 1);
                }

                // Item is new, so add it.
                Items.Insert(0, newItem);
            }
        }
    }
}
