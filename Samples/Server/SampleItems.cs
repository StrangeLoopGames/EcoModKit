// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Shared.Serialization;

[Serialized]
public class SampleItem : WorldObjectItem<SampleObject>
{ }

[Serialized]
public class Sample2Item : Item
{ }

[Serialized]
public class SampleObject : WorldObject
{ }