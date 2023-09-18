![URPSampleBanner](https://media.github.cds.internal.unity3d.com/user/1194/files/f1307f60-0ccf-4be8-ac03-e03f00050418)

# Introducing a new 3D Sample for URP
We are excited to announce the public Beta for our brand-new 3D Sample for the Universal Render Pipeline! 

We have created 4 vignette environments, with different art styles, rendering paths, and scene complexity, to represent common types of projects made with URP, and targeting a wide range of platforms. We wanted to make a visually engaging experience, while showcasing the customizability and scalability of URP on many platforms.

You can download the project right away from GitHub (see [Getting Started](#getting-started) section below). Later this year, by Unite 2023, we will release the project via the Unity Hub.

Let’s take a look at the different environments we created.

## Terminal

This environment offers a radical bump in visual quality compared to the current URP Sample (i.e., the mini construction site). We took inspiration from various modern museums, airports, and sci-fi architecture designs. In particular, this scene takes advantage of physically-based materials and the GPU Lightmapper, to offer a realistic rendering.

![Collage_Terminal_1080](https://media.github.cds.internal.unity3d.com/user/1194/files/37d35286-c036-484e-9c77-6d506ff910fc)

In the main outdoor area, a large platform surrounded by water lets you review your assets in a neutral environment with calibrated lighting conditions. Once you climb the ramp, you can enter 3 rooms, which lets you teleport into universes with radically different art style and platform reach: the Garden, the Oasis, and the Cockpit.
 
## Garden
Have a stroll in the courtyard of this quiet and nocturnal Japanese building (inspired by [Shoin-zukuri style](https://en.wikipedia.org/wiki/Shoin-zukuri#:~:text=Shoin%2Dzukuri%20(%E6%9B%B8%E9%99%A2%E9%80%A0)%20is,today%27s%20traditional%2Dstyle%20Japanese%20house), or visit one of its interiors for a cup of jasmine tea. This environment offers a more stylized rendering, with painterly materials and more organic geometries.

![Collage_Garden_1080](https://media.github.cds.internal.unity3d.com/user/1194/files/b2747d3f-3266-47c0-914f-bf449ca6b72e)

This environment targets lower end mobiles phones, can scale effortlessly on higher end platforms. It features a modular architectural set relying on the default Lit shader, beautifully crafted vegetation (SpeedTree), interconnected interiors, a gentle water stream meandering across the garden, stylized visual effects, as well as numerous decorative dynamic lights to take advantage of URP's Forward+ (or Deferred) rendering path.


## Oasis
Enjoy the warm glow of the sun bathing this peaceful oasis. Walk down the sandy hill, and you’ll find a traditional Bedouin tent set up for the night.

![Collage_Oasis_1080](https://media.github.cds.internal.unity3d.com/user/1194/files/469fdabf-4f30-43bb-a687-ef06b55e1a52)

This environment targets higher-end devices (Nintendo Switch and above), with the use of more complex Shader Graphs for sand, water, fog, and vegetation. This level is an opportunity to demonstrate the ability for URP to reach a high level of photorealistic visual quality when the content is tailored for platforms with more performant GPUs.
Cockpit
Jump into this ultra-stylized rollercoaster where your fleet is under heavy attack from an unknown alien faction. You start your ride inside a hangar ship, and quickly realize your armada isn’t a match for the opposing force…

![Collage_Cockpit_1080](https://media.github.cds.internal.unity3d.com/user/1194/files/11d84d67-8464-47e2-bb75-acaa4f7e31bc)

This environment is specifically tailored for VR use cases: it is designed to run at a high frame rates. Furthermore, we offer a custom lighting model to showcase the ease of customizability of URP, which gives you great insight into 

# Requirements
The scenes are optimized for a wide range of platforms, with dedicated Quality Levels (see Project Settings > Quality) to enjoy an optimal quality and performance, from mobile devices to high-end PCs.

## Recommended minimum configurations:
- Terminal: iPhone 6S Plus or similar
- Garden: iPhone 6S Plus or similar
- Oasis: Nintendo Switch and above
- Cockpit: Meta Quest 2

# Getting started
- Download Unity 2022.3.7f1 through the [Unity Hub](https://unity3d.com/get-unity/download) or the dedicated [download page](https://beta.unity3d.com/download/b16b3b16c7a0/download.html)
- Download a GitHub client, such as [GitHub Desktop](https://desktop.github.com/) (free and open source)
- Clone the project locally with your GitHub client: https://github.com/Unity-Technologies/URP3DSampleScenes
![image](https://media.github.cds.internal.unity3d.com/user/1194/files/56f94627-8da0-427a-9031-35c47e6d2744)

Note 1: Do not use the “Download ZIP” functionality on GitHub, as the ZIP won’t contain all assets needed for the project
Note 2: Later this year, this project will be directly accessible through the Unity Hub, it will not require GitHub anymore.

# What's next
We are aiming for a final release at the time of Unite 2023 (mid-November), when the project will be directly available inside the Unity Hub. Until then, we will keep on polishing the project on the GitHub repository, based on our internal plans and your feedback.

Before this final release, we will also provide an in-depth In-Editor tutorial, with step-by-step instructions and documentation, similarly to the one available for the HDRP 3D Sample already available in the Unity Hub.

Regularly, we will post updates in this thread to let you know about our progress as we approach the final release, and whenever we update the repository with major changes.

Currently, our focus is on the Unity 2022.3 LTS version. After the final release, our objective is to maintain the project, and integrate new features developed for Unity 2023 and beyond.

# Known issues
- Build Times: Some URP configurations can result in large numbers of shader variants which take a long time to compile. To reduce the build times, we recommend only building with one Quality Level only, and toggle VR on or off. Additionally, the compilation time of the individual variants is slow on Windows. We are currently working on improving the situation.

- General performance: For this public beta, we focused on getting each scene running at the target frame rate on each target device. We hopefully succeeded at this, but it means that you might see significant slowdowns in areas where more scenes/cameras are active at the same time, for the teleportation mechanic. We’re looking into solving these performance hits at a later time. 

- First editor run: When opening the editor for the first time and entering game mode, many (blocking) shader compilations will happen.

- Temporal Anti-Aliasing (TAA): TAA is a set on the camera and is currently not set per quality level.

- Oasis: The wind is currently turned off in the Oasis. It will be re-added in the upcoming months.

- Misc: Changing the resolution at runtime (like scaling the game view) will cause the screen textures to have incorrect resolution.

# How to give feedback?
Please take a look at the “Known Issues” section above, before reporting issues.

We invite you to share your general sentiment, ask questions and request help directly in this thread.

For bug reports and crashes, we invite you to discuss them in this thread as well. However, we may ask you to also formally [report bugs](https://unity3d.com/unity/qa/bug-reporting) that aren’t trivial for us to reproduce.

We also encourage you to play with the content, modify it, etc. and share your findings or experiments in this thread.

We are also interested in the level of performance you are getting on your devices. This may help us to identify performance issues hitting very specific types of hardware that we haven’t been able to identify.

Thank you!
