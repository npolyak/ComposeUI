using Avalonia;
using Avalonia.Controls;
using System;

namespace Sebastion.Visuals
{
    public static class DataGridHasSelectedItemsBehavior
    {
        #region HasSelectedItemsBehaviorOn Attached Avalonia Property
        public static bool GetHasSelectedItemsBehaviorOn(DataGrid obj)
        {
            return obj.GetValue(HasSelectedItemsBehaviorOnProperty);
        }

        public static void SetHasSelectedItemsBehaviorOn(DataGrid obj, bool value)
        {
            obj.SetValue(HasSelectedItemsBehaviorOnProperty, value);
        }

        public static readonly AttachedProperty<bool> HasSelectedItemsBehaviorOnProperty =
            AvaloniaProperty.RegisterAttached<DataGrid, DataGrid, bool>
            (
                "HasSelectedItemsBehaviorOn"
            );
        #endregion HasSelectedItemsBehaviorOn Attached Avalonia Property


        #region NumberSelectedRows Attached Avalonia Property
        public static int GetNumberSelectedRows(DataGrid obj)
        {
            return obj.GetValue(NumberSelectedRowsProperty);
        }

        public static void SetNumberSelectedRows(DataGrid obj, int value)
        {
            obj.SetValue(NumberSelectedRowsProperty, value);
        }

        public static readonly AttachedProperty<int> NumberSelectedRowsProperty =
            AvaloniaProperty.RegisterAttached<DataGrid, DataGrid, int>
            (
                "NumberSelectedRows"
            );
        #endregion NumberSelectedRows Attached Avalonia Property


        static DataGridHasSelectedItemsBehavior()
        {
            HasSelectedItemsBehaviorOnProperty.Changed.Subscribe(OnIsOnChanged);
        }

        private static void OnIsOnChanged(AvaloniaPropertyChangedEventArgs<bool> args)
        {
            DataGrid dataGrid = (DataGrid)args.Sender;

            dataGrid.SelectionChanged -= DataGrid_SelectionChanged;
            if (args.NewValue.Value)
            {
                dataGrid.SelectionChanged += DataGrid_SelectionChanged;
            }
        }

        private static void DataGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender!;

            SetNumberSelectedRows(dataGrid, dataGrid.SelectedItems.Count);
        }
    }
}
