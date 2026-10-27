# Smash Campus — Unreal Android rebuild

This is a new Unreal Engine C++ project scaffold for the Android-first rebuild of 경덕중학교 뿌시기. It does not reuse the former Unity block-school scene or terrain.

## Current starter implementation
- Runtime-generated rolling green terrain using ProceduralMeshComponent
- Fresh campus layout with paved road, landscaped quad, several differently sized buildings, windows, and tree-lined edges
- Fixed elevated camera for an initial overview
- Android touch and desktop mouse attack input
- Tagged facade components can be knocked loose with physics

## Requirements
- Unreal Engine 5.4 (or update the .uproject engine association to the installed UE5 version)
- Android Studio / SDK and NDK versions supported by that Unreal version
- C++ toolchain required by Unreal on the development computer
- Enable Android support through Unreal's Android setup documentation before packaging

## Open and package
1. Download this Unreal/SmashCampus folder or clone the repository branch unreal-android-rebuild.
2. Open SmashCampus.uproject with Unreal Engine 5.4 and allow project files to be generated.
3. Build the SmashCampusEditor target, then open the project.
4. Set Android as the target platform, configure Android SDK/NDK, and package an Android ASTC or ETC2 build for device testing.
5. Profile on the actual target phone and reduce terrain resolution, foliage, shadows, and physics if performance requires it.

## Important limitations
- This is a first playable scaffold, not a finished visual match and not yet verified by an Unreal build or on an Android device.
- Accurate recreation of the reference video requires the video itself or clear screenshots. Current architecture and terrain are an original placeholder layout.
- The destruction system is a prototype: tagged pieces detach on a tap. A production version needs better targeting, impact effects, debris limits, reset controls, mobile HUD, and performance tuning.
- The project currently uses engine primitive meshes and a procedural terrain. Replace these with licensed, optimized assets and a tuned landscape/material setup as the visual target becomes clear.
