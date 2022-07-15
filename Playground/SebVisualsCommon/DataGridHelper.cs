using Avalonia;
using Avalonia.Controls;
using NP.Utilities;
using System.Linq;

namespace Sebastion.Visuals
{
    public static class DataGridHelper
    {
        #region CopyMenuItemString Attached Avalonia Property
        public static string GetCopyMenuItemString(DataGrid obj)
        {
            return obj.GetValue(CopyMenuItemStringProperty);
        }

        public static void SetCopyMenuItemString(DataGrid obj, string value)
        {
            obj.SetValue(CopyMenuItemStringProperty, value);
        }

        public static readonly AttachedProperty<string> CopyMenuItemStringProperty =
            AvaloniaProperty.RegisterAttached<DataGrid, DataGrid, string>
            (
                "CopyMenuItemString"
            );
        #endregion CopyMenuItemString Attached Avalonia Property


        public static async void CopySelectedRowsToClipboard(this DataGrid dataGrid, string? separator = null)
        {
            var selectedItems = dataGrid.SelectedItems;

            if (selectedItems.IsNullOrEmptyCollection())
            {
                return;
            }

            if (string.IsNullOrEmpty(separator))
            { 
                separator = "\n";
            }

            string? result = string.Join(separator, selectedItems.Cast<object>().Select(obj => obj.ToString()).ToArray());

            if (result != null)
            {
                await Application.Current!.Clipboard!.SetTextAsync(result);
            }
        }
    }
}
