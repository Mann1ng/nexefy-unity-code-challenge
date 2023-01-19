# nexefy-unity-skills-test-project

Submission of the nexefy code challenge.
Auth: Isaac Dart
Date: 2023-01-19

_Quick Start_

1. Open the Unity project
2. Add the prefab 'Asset-SpawnPointAndExporter' to an empty scene
3. Press Play
4. The following behaviour will occur:

-   Upon entering [Unity] playmode, the program will iterate through a selection of
    models and capture several screenshots for each model.
-   The program will capture 16 screenshots of each model, rotating the model in 22.5 degree intervals until
    it has rotated 360 degrees.
-   The exported image format will be .png.
-   Each output frame renders to a transparent background.
-   The Unity Editor GameView displays what the output frame looks like.
-   Once all of the frames of a model have been captured, the models are unloaded from memory

_Extra features via Export Controller Component_

-   For faster rendering you can turn off flag 'Do Wait For End Of Frames'. Note: This will prevent the game view from showing each frame that is to be exported.
-   To Use the existing camera in the scene as the camera to use during rendering turn on the flag 'Use Existing Camera'.
-   To Use the existing lighting in the scene, rather than generating lighting, turn on the flag 'Use Existing Lighting'.
