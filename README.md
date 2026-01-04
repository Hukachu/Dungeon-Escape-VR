# Dungeon-Escape-VR
Dungeon Escape is an escape-room style VR application for Meta Quest 2, developed in Unity 6 (6000.0.34f1). The experience guides the player through three compact puzzles—Move the Block, Morse Code, and Braille—designed to compare interaction quality across three input modes: XR controllers, XR Hands (hand tracking), and bHaptics TactGlove haptics. The project demonstrates practical XR architecture in Unity, authoring and triggering tactile patterns, and shipping to Meta’s distribution channels.

<img width="1074" height="753" alt="scena2" src="https://github.com/user-attachments/assets/e46751b2-6a17-4c0e-b3fa-f36862ef4de6" />

## Overview
The application opens with a short menu scene and transitions to a dungeon room that contains the three puzzles. Each solved puzzle grants a colored gem; placing all gems on their pedestals opens the exit and ends the session. Locomotion is implemented via Teleportation Anchors. The Braille and Morse puzzles are intentionally tactile-forward to highlight how haptics can carry information that is normally presented through audio or text.

## Technology Stack
The project targets Meta Quest 2 (Android) using OpenXR, XR Interaction Toolkit, and XR Hands for device-agnostic input and hand tracking. Haptic feedback is integrated through the bHaptics Unity SDK and the bHaptics Player/Designer tools. Art and environment elements are primarily assembled from curated Unity Asset Store packages to keep the focus on interaction and haptics rather than original asset production.

## Getting Started
Opening the project. Clone the repository and open it with Unity Hub using Unity 6 (6000.0.34f1). Unity Package Manager will prompt for package resolution; ensure OpenXR, XR Interaction Toolkit, XR Hands, and the Input System are present. In Project Settings → XR Plug-in Management (Android) enable OpenXR. In OpenXR → Features (Android) enable Meta Quest support and hand tracking (optionally “initialize on startup” for automatic permission prompts).

bHaptics integration. Install bHaptics Player on the target device (Windows or Quest). Pair the TactGlove and verify motors through the Player test screen. In Unity, import the bHaptics Haptic Plugin and add the manager prefab to the active scene. Supply the App ID and API Key from the bHaptics Developer portal. Tactile patterns authored in bHaptics Designer can be referenced by name and triggered from gameplay events.

## Interaction and Haptics Design
The interaction model is intentionally conservative: teleport locomotion, direct object manipulation, and button activation. Controllers provide the most predictable experience, while hand tracking introduces naturalistic gestures at the expense of robustness in certain conditions. The haptics layer is designed as a small service that maps semantic events—such as “drag block,” “button press,” “Braille bead contact,” and “Morse pulse”—to short, distinctive patterns. The goal is to keep patterns brief, spatially consistent (left/right), and aligned with the visual frame to minimize perceived latency and avoid tactile fatigue.

In Move the Block, continuous low-intensity patterns communicate surface friction while dragging. Morse Code uses short and long pulses on the left palm to convey dots and dashes when the player’s hand rests on the panel. Braille employs light taps on the index finger when touching the invisible bead clusters; the player reads the word tactually, then enters it using letter buttons. A short “click” pulse accompanies successful button presses to reinforce confirmation.

## Research and Background
This project was informed by work on haptic gloves and VR interaction including:
Perret & Vander Poorten, Touching Virtual Reality: A Review of Haptic Gloves (ACTUATOR 2018);
Popescu, Burdea & Bouzit, VR Simulation Modeling for a Haptic Glove (1999);
Shanmugam et al., A Comprehensive Review of Haptic Gloves: Advances, Challenges, and Future Directions (2023).

## License and Contributions
This project is my final thesis and is the property of the Faculty of Electrical Engineering, University of Sarajevo (Elektrotehnički fakultet Sarajevo).
