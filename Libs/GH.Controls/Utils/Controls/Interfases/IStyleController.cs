using System;

namespace GH.Controls.Utils.Controls
{
    public interface IStyleController
    {
        AppearanceObject Appearance { get; }

        AppearanceObject AppearanceReadOnly { get; }

        AppearanceObject AppearanceDisabled { get; }

        AppearanceObject AppearanceFocused { get; }

        AppearanceObject AppearanceDropDown { get; }

        AppearanceObject AppearanceDropDownHeader { get; }


        BorderStyles BorderStyle { get; }

        BorderStyles ButtonsStyle { get; }

        PopupBorderStyles PopupBorderStyle { get; }

        event EventHandler PropertiesChanged;

        event EventHandler Disposed;
    }

}
