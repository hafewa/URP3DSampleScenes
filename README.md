![URPSampleBanner](https://media.github.cds.internal.unity3d.com/user/1194/files/f1307f60-0ccf-4be8-ac03-e03f00050418)

# Introducing the new URP 3D Sample
We are excited to share with you the public Beta for our brand-new 3D Sample for the Universal Render Pipeline! 

We have created 4 vignette environments with different art styles, rendering paths, and scene complexity, to represent the great variety of 3D projects built with URP. This sample targets a wide range of platforms, from mobile devices, to consoles and PCs, as well as VR headsets. 

We wanted to make a visually engaging experience to showcase the customizability and scalability of URP, while taking into account the many hardware limitations that impact cross-platform development.

We have ironed out most obvious bugs, however because this project is still in Beta phase, we expect you will encounter issues we haven't run into. Therefore, we are looking forward to hearing your inputs, so we can make this sample, and URP as a whole, better. Please see the [How to give feedback?](#how-to-give-feedback) section below.

You can download the project right away from GitHub, please check the [Getting Started](#getting-started) section below. By the end of the year, we will release the project directly via the Unity Hub. Regularly, we will post updates on the forum to let you know about our progress as we approach the final release, and whenever we update the repository with major changes.

Now, let’s take a look at the different environments we created!

## Terminal

**Recommended minimum configurations:** iPhone 6S or similar

This environment offers a radical bump in visual quality compared to the current URP Sample (i.e., the mini construction site). We took inspiration from various modern museums, airports, and sci-fi architecture designs. In particular, this scene takes advantage of physically-based materials and the GPU Lightmapper to offer a realistic rendering.

![Collage_Terminal_1080](https://media.github.cds.internal.unity3d.com/user/1194/files/37d35286-c036-484e-9c77-6d506ff910fc)

In the main outdoor area, a large platform surrounded by water lets you review your assets in a neutral environment with calibrated lighting conditions. Once on the ramp, you can enter 3 rooms which will teleport you into universes with radically different art styles and platform reach: the Garden, the Oasis, and the Cockpit. To initiate the teleportation, focus on the Unity logo above the teleporting device for a few seconds (requires Play Mode).

![TerminalTeleportation](https://media.github.cds.internal.unity3d.com/user/1194/files/523d673b-5e2c-4b1d-8008-59fe3d474eb5)

## Garden

**Recommended minimum configurations:** iPhone 6S or similar

This environment offers a more stylized rendering, with painterly materials and more organic geometries. Have a stroll in the courtyard of this quiet and nocturnal Japanese building (inspired by [Shoin-zukuri style](https://en.wikipedia.org/wiki/Shoin-zukuri#:~:text=Shoin%2Dzukuri%20(%E6%9B%B8%E9%99%A2%E9%80%A0)%20is,today%27s%20traditional%2Dstyle%20Japanese%20house), or visit one of its interiors for a cup of jasmine tea.

![Collage_Garden_1080](https://media.github.cds.internal.unity3d.com/user/1194/files/b2747d3f-3266-47c0-914f-bf449ca6b72e)

This environment scales effortlessly on mobile devices and higher end platforms. It features a modular architectural set relying on the default Lit shader, beautifully crafted vegetation (SpeedTree), interconnected interiors, a gentle water stream meandering across the garden, stylized visual effects, as well as numerous decorative dynamic lights to take advantage of URP's Forward+ (or Deferred) rendering path.


## Oasis

**Recommended minimum configurations:** iPhone 8 or similar

Enjoy the warm glow of the sun setting on this peaceful oasis. Walk down the sandy hill, and you’ll find a traditional Bedouin tent set up for the night.

![Collage_Oasis_1080](https://media.github.cds.internal.unity3d.com/user/1194/files/469fdabf-4f30-43bb-a687-ef06b55e1a52)

This environment targets handheld consoles and higher end devices, with the use of more complex Shader Graphs for sand, water, fog, and vegetation. This level is an opportunity to demonstrate the ability for URP to reach a higher level of visual quality when the content is tailored for platforms with more performant GPUs.

## Cockpit

**Recommended minimum configurations:** Meta Quest 2

Jump into this ultra-stylized rollercoaster where your fleet is under heavy attack from an unknown alien faction. You start your ride inside a hangar ship, and quickly realize your armada isn’t a match for the opposing force.

![Collage_Cockpit_1080](https://media.github.cds.internal.unity3d.com/user/1194/files/11d84d67-8464-47e2-bb75-acaa4f7e31bc)

This environment is specifically tailored for VR headsets and lower end mobile devices: it is designed to run at high frame rates, while still featuring beautifully stylized visuals. Furthermore, we offer a custom lighting model to showcase the ease of customizability of URP. 

# Getting started
- Install Unity 2022.3.7f1 via the [Unity Hub](https://unity3d.com/get-unity/download) or the dedicated [download page](https://beta.unity3d.com/download/b16b3b16c7a0/download.html)
- Install a GitHub client, such as [GitHub Desktop](https://desktop.github.com/) (free and open source)
- Clone the project locally with your GitHub client: https://github.com/Unity-Technologies/URP3DSampleScenes
- Add this project to the Unity Hub

![image](https://media.github.cds.internal.unity3d.com/user/1194/files/878859dc-4a68-4ec2-9724-19b29bb520eb)

**Note 1:** Do not use the “Download ZIP” functionality on GitHub, as the ZIP won’t contain all assets needed for the project

**Note 2:** Later this year, this project will be directly accessible through the Unity Hub, it will not require GitHub anymore.

# What's next
We are aiming for a final release at the time of Unite 2023 (mid-November), when the project will be directly available inside the Unity Hub. Until then, we will keep on polishing the project on the GitHub repository, based on your feedback and our internal plans.

Before this final release, we will also provide an in-depth In-Editor tutorial, with step-by-step instructions and documentation, similarly to the one available for the HDRP 3D Sample already available in the Unity Hub. You can also get some insights watching our GDC talks: [Cross-platform game development with the new URP sample scene](https://www.youtube.com/watch?v=zPTNrSgoJow), and [How to Build for PS VR2 with URP](https://www.youtube.com/watch?v=5z55_k0MgGA).

Currently, our focus is on the Unity 2022.3 LTS version. After the final release, our objective is to maintain the project, and integrate new features developed for Unity 2023 and beyond.

# Known issues
**Build Times:** Due to the wide variety of use cases, target platforms, renderers, and features presented in these samples, some URP configurations can result in large numbers of shader variants, which take a long time to compile. To reduce build times, we recommend only building with one Quality Level, a subset of scenes, and toggling VR on or off. We are currently working on optimizing the shader compilation times.

**General performance:** For this public beta, we focused on getting each scene running at the target frame rate for the recommended minimum configurations. However, you may see significant slowdowns in areas where multiple scenes/cameras are active at the same time, for the teleportation mechanic. 

**First editor run:** When opening the editor, navigating the scenes and entering game mode for the first time, many shader compilations will happen.

**Temporal Anti-Aliasing (TAA):** TAA is a set on the camera and is currently not set per quality level.

**Oasis:** The wind is currently turned off in the Oasis. It will be re-added in the upcoming months.

**Misc:** Changing the resolution at runtime (such as scaling the game view) will cause the screen textures to have incorrect resolution.

# How to give feedback?

Please take a look at the [Known issues](#known-issues) above, before reporting issues.

We invite you to share your general sentiment, ask questions and request help directly in the forum. We also encourage you to experiment with the content of this project and share your findings.

Furthermore, we are also interested in the level of performance you are getting on your devices. This may help us to identify performance issues on certain hardware configurations.

For bug reports and crashes, we invite you to discuss them in the forum as well. However, we may ask you to also formally [report bugs](https://unity3d.com/unity/qa/bug-reporting) that aren’t trivial for us to reproduce.

Thank you!
