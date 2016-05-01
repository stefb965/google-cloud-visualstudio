using System;
using Microsoft.VisualStudio.Shell;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub.Common
{
    public static class ExtensionBrushes
    {
        public static Guid CommonControls = new Guid("c01072a1-a915-4abf-89b7-e2f9e8ec4c7f");

        public static object TextBoxBackground => new ThemeResourceKey(CommonControls, "TextBoxBackground", ThemeResourceKeyType.BackgroundBrush);
        public static object TextBoxText => new ThemeResourceKey(CommonControls, "TextBoxText", ThemeResourceKeyType.BackgroundBrush);
        public static object TextBoxBorder => new ThemeResourceKey(CommonControls, "TextBoxBorder", ThemeResourceKeyType.BackgroundBrush);
        public static object TextBoxBorderFocused => new ThemeResourceKey(CommonControls, "TextBoxBorderFocused", ThemeResourceKeyType.BackgroundBrush);

    }
}
