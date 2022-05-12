# Ring Color Plugin

This unofficial TaleSpire mod allows setting of a mini's faction color (the color of
the ring used to indicated that the mini is selected).

## Change Log

```
1.0.0: Initial release
```

## Install

Use R2ModMan or similar installer to install.

## Usage

Right click on the mini to open the context menu. Select the Ring Color option.
Select one of the available colors.
	  
## Configuration

The Ring Color plugin comes with a configuration of 10 colors. While more colors
cannot be added at this time, the 10 available colors are fully programmable.
In the R2ModMan configruation for the plugin each of the 10 colors has a name entry
and a value entry. The name entry indicates the name that will be displayed for the
color selection (and also the name that is used to look up the corresponding icon).
The value entry is a Hex string representing the color. The Hex string is in the
format RRGGBB meaning the first two digits are a Hex number between 0 and 255 (FF)
for the red component, the next two are a Hex number between 0 and 255 (FF) for the
green component and lastly the last two digits are a Hex number between 0 and 255
(FF) for the blue component.

Player's colors don't need to match. When a ring color request is sent out to other
players it uses the color specified by the initiating player and ignores the color
that the destination player has configured.

When adding a new color, icons for that color can be created by creating PNG file with
the name RingColor.png where Color is the color name. For example, RingRed.png. If
such a file does not exist, the plugin uses the default icon which is a white icon.

## Limitation

Currently this function is available in the main menu and to all users (GM and players).
Eventually it is planned to move options to the GM Tools menu where it will only be
available to the GM.
