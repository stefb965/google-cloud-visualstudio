using System;
using Microsoft.VisualStudio.Shell;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub.Common
{
    public static class ExtensionBrushes
    {
        public static Guid CommonControls = new Guid("c01072a1-a915-4abf-89b7-e2f9e8ec4c7f");

        public static object TextBoxBackground => new ThemeResourceKey(CommonControls, "TextBoxBackground", ThemeResourceKeyType.BackgroundBrush);
        public static object TextBoxBorder => new ThemeResourceKey(CommonControls, "TextBoxBorder", ThemeResourceKeyType.BackgroundBrush);
        public static object TextBoxText => new ThemeResourceKey(CommonControls, "TextBoxText", ThemeResourceKeyType.BackgroundBrush);
        public static object TextBoxBackgroundDisabled => new ThemeResourceKey(CommonControls, "TextBoxBackgroundDisabled", ThemeResourceKeyType.BackgroundBrush);
        public static object TextBoxBorderDisabled => new ThemeResourceKey(CommonControls, "TextBoxBorderDisabled", ThemeResourceKeyType.BackgroundBrush);
        public static object TextBoxTextDisabled => new ThemeResourceKey(CommonControls, "TextBoxTextDisabled", ThemeResourceKeyType.BackgroundBrush);
        public static object TextBoxBackgroundFocused => new ThemeResourceKey(CommonControls, "TextBoxBackgroundFocused", ThemeResourceKeyType.BackgroundBrush);
        public static object TextBoxBorderFocused => new ThemeResourceKey(CommonControls, "TextBoxBorderFocused", ThemeResourceKeyType.BackgroundBrush);
        public static object TextBoxTextFocused => new ThemeResourceKey(CommonControls, "TextBoxTextFocused", ThemeResourceKeyType.BackgroundBrush);

        public static object CheckBoxBackground => new ThemeResourceKey(CommonControls, "CheckBoxBackground", ThemeResourceKeyType.BackgroundBrush);
        public static object CheckBoxBackgroundPressed => new ThemeResourceKey(CommonControls, "CheckBoxBackgroundPressed", ThemeResourceKeyType.BackgroundBrush);
        public static object CheckBoxBackgroundHover => new ThemeResourceKey(CommonControls, "CheckBoxBackgroundHover", ThemeResourceKeyType.BackgroundBrush);
        public static object CheckBoxText => new ThemeResourceKey(CommonControls, "CheckBoxText", ThemeResourceKeyType.BackgroundBrush);
        public static object CheckBoxTextHover => new ThemeResourceKey(CommonControls, "CheckBoxTextHover", ThemeResourceKeyType.BackgroundBrush);
        public static object CheckBoxTextPressed => new ThemeResourceKey(CommonControls, "CheckBoxTextPressed", ThemeResourceKeyType.BackgroundBrush);
        public static object CheckBoxTextDisabled => new ThemeResourceKey(CommonControls, "CheckBoxTextDisabled", ThemeResourceKeyType.BackgroundBrush);
        public static object CheckBoxTextFocused => new ThemeResourceKey(CommonControls, "CheckBoxTextFocused", ThemeResourceKeyType.BackgroundBrush);
        public static object CheckBoxBackgroundDisabled => new ThemeResourceKey(CommonControls, "CheckBoxBackgroundDisabled", ThemeResourceKeyType.BackgroundBrush);
        public static object CheckBoxBackgroundFocused => new ThemeResourceKey(CommonControls, "CheckBoxBackgroundFocused", ThemeResourceKeyType.BackgroundBrush);
        public static object CheckBoxBorder => new ThemeResourceKey(CommonControls, "CheckBoxBorder", ThemeResourceKeyType.BackgroundBrush);
        public static object CheckBoxBorderHover => new ThemeResourceKey(CommonControls, "CheckBoxBorderHover", ThemeResourceKeyType.BackgroundBrush);
        public static object CheckBoxBorderPressed => new ThemeResourceKey(CommonControls, "CheckBoxBorderPressed", ThemeResourceKeyType.BackgroundBrush);
        public static object CheckBoxBorderDisabled => new ThemeResourceKey(CommonControls, "CheckBoxBorderDisabled", ThemeResourceKeyType.BackgroundBrush);
        public static object CheckBoxBorderFocused => new ThemeResourceKey(CommonControls, "CheckBoxBorderFocused", ThemeResourceKeyType.BackgroundBrush);
        public static object CheckBoxGlyph => new ThemeResourceKey(CommonControls, "CheckBoxGlyph", ThemeResourceKeyType.BackgroundBrush);
        public static object CheckBoxGlyphHover => new ThemeResourceKey(CommonControls, "CheckBoxGlyphHover", ThemeResourceKeyType.BackgroundBrush);
        public static object CheckBoxGlyphPressed => new ThemeResourceKey(CommonControls, "CheckBoxGlyphPressed", ThemeResourceKeyType.BackgroundBrush);
        public static object CheckBoxGlyphDisabled => new ThemeResourceKey(CommonControls, "CheckBoxGlyphDisabled", ThemeResourceKeyType.BackgroundBrush);
        public static object CheckBoxGlyphFocused => new ThemeResourceKey(CommonControls, "CheckBoxGlyphFocused", ThemeResourceKeyType.BackgroundBrush);

    }
}
