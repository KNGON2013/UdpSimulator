using System;
using System.Windows.Data;

namespace UdpSimulator.Xamls.Converters
{
    /// <summary>
    /// XAMLコンバーター(Bindingプロパティ(bool)変換).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BoolToValueConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }

        public T TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return this.FalseValue;
            }
            else
            {
                return (bool)value ? this.TrueValue : this.FalseValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null && value.Equals(this.TrueValue);
        }
    }

    /// <summary>
    /// Bindingプロパティ変換(T=string).
    /// </summary>
    public class BoolToStringConverter : BoolToValueConverter<string>
    {
    }

    /// <summary>
    /// Bindingプロパティ変換(T=bool).
    /// </summary>
    public class BoolToEnabledConverter : BoolToValueConverter<bool>
    {
    }
}
