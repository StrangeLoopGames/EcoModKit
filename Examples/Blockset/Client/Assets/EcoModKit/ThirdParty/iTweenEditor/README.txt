iTween Visual Editor version 0.6.1

After installation, if you have a Javascript based project it is highly recommended that you run the "Component/iTween/Prepare Visual Editor for Javascript Usage" command. This will copy files into your Plugins directory which is necessary for the visual editor events to be called from Javascript. C# based projects require no further configuration.

Release History ===============

0.6.1 ====

Added Stop method to iTweenEvent.  Improved display of boolean values.

0.6.0 ====

Fixed warnings resulting from API changes in Unity 3.4.  Fixed error where bool fields could pass the wrong value.  Updated bundled iTween version to 2.0.45.1.  Made initial delay field always show, even when 'Play Automatically' was not selected.

0.5.2 ====

Fixed a problem with Vector3 or Transform paths introduced when implemented path support.

0.5.1 ====

Added iTween menu item to move files into the correct position for Javascript based projects.
Fixed bug with delay of 0 seconds still pausing slightly before beginning tween.
Changed visual path editor selection list to use global listing instead of just paths attached to the same GameObject.

0.5 ======

Added integration for the iTween Path Editor.
This work was based on code contributed by Gabriel Gheorghiu (gabison@gmail.com).

0.4 ======

Minor bug fix release.  iTweenEvents now show an icon in the scene view.

0.3 ======

Major bug fixed where iTweenEvent settings would be reset.
Name field was added to the iTweenEvent so you can distinguish them.
Added static method on the iTweenEvent class named GetEvent.

0.2 ======

Added support for the path parameter of MoveTo and MoveFrom.
Callbacks changed to take a single string parameter.

0.1 ======

Initial release.