## v1.3.1

* New Bugfixes
    * Hedging Weapons AC bonus now shows on the character screen and applies against non-weapon attacks (e.g. spells)
    * Energy Channel should apply more consistently
    * Damage from Energy Channel is multiplied on crits

## v1.3.0

* Horrific Doubles no longer aggros enemies
* New Bugfixes
    * Fixed game freeze caused by Hurtful triggering off of Assassinate
    * Fixed descriptors for dimensional Blade
    * Buffs are no long blocked by descriptor immunity
    * Eldritch Heritage: Destined now correctly grants a touch attack ability

## v1.2.9

* Localization fixes
* New Bugfixes
    * Keen Edge works on daggers
    * Paired Opportunists won't trigger attacks targeting allies

## v1.2.8

* Localization fixes

## v1.2.7

* Chinese Localization courtesy of 呶呶BOT

## v1.2.6

* New Bugfixes
    * Frozen Caress no longer turns buff spells into a bad touch

## v1.2.5

* Compatibility fixes for TTT-Base
    * Arrowsong Minstrel and Winter Witch are compatible with TTT-Base Loremaster
    * Second Patron no longer lets you select invalid patrons if you have the Winter Witch archetype

* New Bugfixes
    * Loremaster secrets should work properly for Winter Witch and Arrowsong Minstrel

## v1.2.4

* Compatibility for Patch 2.1

* New Bugfixes
    * Frozen Caress now adds damage to all touch spells
    * Dimensional Blade actually changes the attack type instead of granting an attack bonus

## v1.2.3

* Update for Patch 2.1 Beta
* Removed redundant fixes
    * Cone of Cold on Witch Spell List

* Mortal Terror is now disabled by default
    * Owlcat implemented it but you can still enable COP if you prefer.
    * If you're mid-playthrough then you might want to respec and use Owlcat's version

## v1.2.2

* New Spells
    * Cheetah's Sprint
    * Defensive Shock

* New Bufixes
    * Compatibility fix for Expanded Content mod

## v1.2.1

* New Bugfixes
    * Horrific Doubles and Weapon of Awe can now be cast on characters immune to mind-affecting abilities

## v1.2

* Reworked Signature Skill: Persuasion and Mortal Terror
    * Wrath doesn't have an implementation of the Panicked condition so only Shaken, Cowering, and Frightened are used
* Reworked Keen Edge and Weapon of Awe
    * They can now explicitly target a Main Hand or Off Hand weapon
* Reworked Horrific Doubles
    * Range reduced from 120ft to 60ft
    * Performance improvements
    * Improved compatibility with multiple instances (i.e. multiple characters using it simultaneously)
* Reduced range for abilities targeting "anyone you can see" from 120ft to 60ft
* Minor performance optimizations
* Updated icon for Purifying Channel
* New feats and class features with prerequisites are now listed on their respective prerequisites
* New spells should support scroll and potion crafting

* New Feats
    * Energy Channel

* New Spells
    * Frostbite
    * Chill Touch

* New Bugfixes
    * Fixed several abilities not working while mounted including:
        * Weapon of Awe
        * Keen Edge
        * Hedging Weapons
        * Dimensional Blade
        * Iomedae's Inspiring Sword
    * Implosion correctly indicates a Fortitude throw negates the effect

## v1.1.2

* Russian localization update

* New Feats
    * Purifying Channel

* New Bugfixes
    * Eliminates lag caused by Lord Beyond the Grave
    * Fixed damage on Implosion
    * Implosion works in turn-based mode

## v1.1.1

* New Class Features
    * Slayer Talents
        * Armored Marauder
        * Armored Swiftness

* New Bugfixes:
    * Share Spells won't break Alchemist Infusions and Brown Fur Transmutor share transmutations

## v1.1.0

* New Optional Changes:
    * Share Spells - When enabled, Personal range spells can be cast on animal companions

* New Bugfixes:
    * Weapon of Awe & Keen Edge can target ranged weapons
    * Screaming Flames target text no longer indicates only enemies are affected
    * Eldritch Heritage selection now requires you to have at least one Skill Focus
        * This should prevent issues where the selection shows up but you do not qualify for any bloodline

## v1.0.5

* Winter Witch spellbook should include all modded spells for Witch
* Added duration and saving throw text to all abilities
* Frozen Caress no longer turns off after combat

## v1.0.4

* Nine Lives has duration text
* Fixed mod conflicts that sometimes prevent new spells from being learnable

## v1.0.3

* Frozen Caress should work with ranged touch spells

## v1.0.2

* Freezing Sphere description reflects the optional rule setting

## v1.0.1

* Fixes a bug preventing Winter Witch Winter Witches from getting hexes

## v1.0.0

* New Archetypes:
    * Winter Witch

* New Class Features:
    * Shimmering Mirage (Wild Talent)
    * Suffocate (Wild Talent)

* New Spells:
    * Dimensional Blade
    * Fleshworm Infestation
    * Freezing Sphere
        * Optional rule to make it only affect enemies
    * Frost Fall
    * Ice Spears
    * Implosion
    * Judgement Light
    * Keen Edge
    * Nine Lives
    * Unshakable Zeal

* New Bugfixes:
    * Cone of Cold is now on the Witch spell list
    * Evasion is now available as an Advanced Slayer Talent
    * Arrowsong Minstrel should work with Archetypes that extend your spellbook

* New Optional Changes:
    * Cone of Cold replaces Cold Ice Strike as a bonus spell granted by the Winter Patron

* Updates:
    * Single Draconic Bloodline / Single Elemental Bloodline fixes are now under "Optional" and default to false
        * By popular demand + Owlcat stating this is working as intended

## v0.9.1

* New Spells:
    * Mortal Terror
    * Invisibility Purge
    * Weapon of Awe

* Updates:
    * Patched WorldCrawl compatibility

## v0.9.0

* New Spells:
    * Touch of Blindness - Includes support for multiple touch charges
    * Shadow Trap
    * Bladed Dash and Bladed Dash, Greater
    * Frozen Note
    * Wrath
    * Hedging Weapons

* Updates
    * Added Localized Duration to some spells
    * Divine Fighting Techniques granting Vital Strike effects do not multiply on crits
        * This is a base game bug that is fixed if you're using TTT-Base
    * Furious Focus works when two-handing 1h weapons

## v0.8.1

* New Feats:
    * Abadar's Divine Fighting Technique

* Updates
    * Improved text templating on Divine Fighting Technique
        * Clarify Advanced Effects on initial feat
        * Remove Advanced text from abilities and buffs granted by the initial benefit
    * Torag's Divine Fighting Technique qualifies as Combat Reflexes for prerequisites

## v0.8.0

* New Feats:
    * Divine Fighting Technique - Special combat techniques associated with a diety and their favored weapon
        * Asmodeus, Erastil, Gorum, Iomedae, Irori, Lamashtu, Norgorber, Rovagug, Torag, Urgathoa
    * Dazing Assault

* Updates:
    * Glorious Heat should properly be removed during respec

## v0.7.1

* Updates:
    * Burning Disarm now has max damage dice

## v0.7.0

* New Spells:
    * Consecrate / Desecrate
    * Burning Disarm
        * Characters automatically determine whether they want to drop their weapon based on int, wis, health, and their preference for martial combat vs. spells

* New Class Features:
    * Slowing Strike (Rogue Talent)
    * Shadow Duplicate (Rogue Talent)

* Updates:
    * Russian localization for most content courtesy of Kurufinve
        * If you spot errors or want to help localize text, reach out to me
    * Ice Slick has a reasonable icon now...
    * Multiple icons updated courtesy of Cyrix
        * Furious Focus
        * Break Free
        * Burning Disarm
        * Glorious Heat

## v0.6.0

* New Class Features:
    * Ice Tomb Hex (Witch)
* New Spells:
    * Ice Slick

## v0.5.0

* New Feats:
    * Signature Skill
        * Free bonus feat for Rogues at 5 / 10 / 15 / 20
        * Available for every skill except Use Magic Device and Thievery
* Updates:
    * Lich Companion upgrade can now select Paired Opportunists as a teamwork feat
    * Hurtful now has an attack animation when it triggers!

## v0.4.2

* Hurtful works again, and correctly...

## v0.4.1

* Updates
    * Draconic / Elemental Bloodline changes now flagged as bug fixes
    * Draconic / Elemental Heritage should function properly with respec

## v0.4.0

* New Feats:
    * Eldritch Heritage (Improved, Greater)
        * Supports base game bloodlines + TTT-Base bloodlines
        * Notably Arcane Level 9 power is not supported. It only works for some classes / archetypes and it's a lot of work to build. Maybe on day.
* Bug Fixes:
    * Serpentine Bloodine's bite power correctly computes the DC
    * Serpentine Bloodline's Serpent Friend grants the correct skill bonuses
* Updates
    * Hurtful no longer triggers if you don't have a swift
    * Clarified Arrowsong Minstrel's Arcane Archery feature

## v0.3.3

* Fixed compatibility with EE

## v0.3.2

* Updated Paired Opportunists should correctly be shared with all teamwork feat sharing abilities
    * Now shares with all teamwork feat sharing abilities
    * Available for selection as a Pack Rager bonus feat
    * Adjusted recommendations so it isn't so aggressively recommended

## v0.3.0

* New Feats:
    * Glorious Heat
    * Paired Opportunists
* Updates:
    * Added Hurricane Bow to Arrowsong Minstrel's bonus spell selection
