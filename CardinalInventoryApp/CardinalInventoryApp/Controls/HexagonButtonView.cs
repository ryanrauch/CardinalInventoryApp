using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CardinalInventoryApp.Controls
{
    public class HexagonButtonView : View
    {
        public static readonly BindableProperty RadiusProperty = BindableProperty.Create(
                nameof(Radius),
                typeof(double),
                typeof(HexagonButtonView),
                10.0);
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly BindableProperty PointyTopProperty = BindableProperty.Create(
                nameof(PointyTop),
                typeof(bool),
                typeof(HexagonButtonView),
                true);
        public bool PointyTop
        {
            get { return (bool)GetValue(PointyTopProperty); }
            set { SetValue(PointyTopProperty, value); }
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
                nameof(Text),
                typeof(string),
                typeof(HexagonButtonView),
                string.Empty);
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty TextColorProperty =
                            BindableProperty.Create(nameof(TextColor),
                                                    typeof(Color),
                                                    typeof(HexagonButtonView),
                                                    Color.Black);
        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public static readonly BindableProperty FontSizeProperty =
                    BindableProperty.Create(nameof(FontSize),
                                            typeof(Double),
                                            typeof(HexagonButtonView),
                                            10d);
        public Double FontSize
        {
            get { return (Double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly BindableProperty FontFamilyProperty =
            BindableProperty.Create(nameof(FontFamily),
                                    typeof(string),
                                    typeof(HexagonButtonView),
                                    "highlandgothiclightflf");
        public string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public static readonly BindableProperty FATextProperty = BindableProperty.Create(
                nameof(FAText),
                typeof(string),
                typeof(HexagonButtonView),
                string.Empty);

        public string FAText
        {
            get { return (string)GetValue(FATextProperty); }
            set { SetValue(FATextProperty, value); }
        }

        public static readonly BindableProperty FAFontFamilyProperty =
                BindableProperty.Create(nameof(FAFontFamily),
                                        typeof(string),
                                        typeof(HexagonButtonView),
                                        "FontAwesome5FreeSolid");
        public string FAFontFamily
        {
            get { return (string)GetValue(FAFontFamilyProperty); }
            set { SetValue(FAFontFamilyProperty, value); }
        }

        public static readonly BindableProperty FAFontSizeProperty =
            BindableProperty.Create(nameof(FAFontSize),
                                    typeof(Double),
                                    typeof(HexagonButtonView),
                                    18d);
        public Double FAFontSize
        {
            get { return (Double)GetValue(FAFontSizeProperty); }
            set { SetValue(FAFontSizeProperty, value); }
        }

        public static readonly BindableProperty BorderColorProperty =
                    BindableProperty.Create(nameof(BorderColor),
                                            typeof(Color),
                                            typeof(HexagonButtonView),
                                            Color.Transparent);
        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public static readonly BindableProperty BorderSizeProperty =
            BindableProperty.Create(nameof(BorderSize),
                                    typeof(Double),
                                    typeof(HexagonButtonView),
                                    4d);
        public Double BorderSize
        {
            get { return (Double)GetValue(BorderSizeProperty); }
            set { SetValue(BorderSizeProperty, value); }
        }

        public static readonly BindableProperty IsMenuProperty =
            BindableProperty.Create("IsMenu",
                                    typeof(bool),
                                    typeof(HexagonButtonView),
                                    false);
        public bool IsMenu
        {
            get { return (bool)GetValue(IsMenuProperty); }
            set { SetValue(IsMenuProperty, value); }
        }
    }
}
