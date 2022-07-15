using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Sebastion.Visuals
{
    public class SebastionAttachedProps
    {
        #region Icon Attached Avalonia Property
        public static IImage GetIcon(IControl obj)
        {
            return obj.GetValue(IconProperty);
        }

        public static void SetIcon(IControl obj, IImage value)
        {
            obj.SetValue(IconProperty, value);
        }

        public static readonly AttachedProperty<IImage> IconProperty =
            AvaloniaProperty.RegisterAttached<SebastionAttachedProps, IControl, IImage>
            (
                "Icon"
            );
        #endregion Icon Attached Avalonia Property


        #region ButtonText Attached Avalonia Property
        public static string GetButtonText(IControl obj)
        {
            return obj.GetValue(ButtonTextProperty);
        }

        public static void SetButtonText(IControl obj, string value)
        {
            obj.SetValue(ButtonTextProperty, value);
        }

        public static readonly AttachedProperty<string> ButtonTextProperty =
            AvaloniaProperty.RegisterAttached<SebastionAttachedProps, IControl, string>
            (
                "ButtonText"
            );
        #endregion ButtonText Attached Avalonia Property


        #region RowClasses Attached Avalonia Property
        public static string GetRowClasses(IControl obj)
        {
            return obj.GetValue(RowClassesProperty);
        }

        public static void SetRowClasses(IControl obj, string value)
        {
            obj.SetValue(RowClassesProperty, value);
        }

        public static readonly AttachedProperty<string> RowClassesProperty =
            AvaloniaProperty.RegisterAttached<SebastionAttachedProps, IControl, string>
            (
                "RowClasses"
            );
        #endregion RowClasses Attached Avalonia Property

        public static readonly RoutedEvent<RoutedEventArgs> DataGridRowDoubleTappedEvent =
            RoutedEvent.Register<RoutedEventArgs>("DataGridRowDoubleTapped", RoutingStrategies.Bubble, typeof(SebastionAttachedProps));
    }
}
