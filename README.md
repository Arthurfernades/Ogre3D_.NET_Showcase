Ogre 3D Showcase in WPF

📌 Introduction

This project demonstrates key features of the Ogre 3D engine using C# bindings generated with SWIG (Simplified Wrapper and Interface Generator). It integrates Ogre 3D into a WPF application, allowing real-time interaction with various rendering elements.

The goal is to provide a foundation for developers who want to explore 3D rendering in WPF using Ogre 3D, leveraging the power of DirectX.

✅ Features

This project showcases the following Ogre 3D capabilities:

Scene Management – Includes a camera, lighting, and basic scene setup.

Entities – Loads and displays Ogre .mesh objects within the scene.

Terrain Generation – Uses Ogre's terrain system to create and manipulate landscapes.

Shadows – Implements texture-based shadows, with the potential to expand to other shadowing techniques.

Users can:

Move the camera freely within the scene.

Modify the terrain and skybox dynamically.

Add and manipulate entities in real time.

⚙️ Installation & Setup

You can download this repository without cloning and run the .exe file located in Bin/x64/Teste.exe. Alternatively, you can follow the steps below to clone the repository, which might be useful for debugging errors.

Clone this repository:

git clone https://github.com/your-username/Ogre-3D-WPF-Showcase.git
cd Ogre-3D-WPF-Showcase

Install required dependencies.

Build the project using Visual Studio or your preferred IDE.

Run the application.

🎮 How to Use

Camera Movement: Use the right mouse button to orbit the camera and the middle mouse button to move along the X and Y axes.

Add Entities: Click on the UI buttons to add objects.

Modify Terrain & Skybox: Use the list box to choose the one you prefer.

🖼️ Demonstration

![OgreShowcase1](https://github.com/user-attachments/assets/26cd8ebc-c63e-45f0-bd37-646c24e81f8f)

🚀 Customization & Expansion

Adding new entities: Modify SceneManager.cs to load additional models.

Changing the skybox: Replace textures in the assets/skybox folder.

Enhancing shadows: Experiment with different shadow techniques in Ogre 3D.

🛠️ Troubleshooting

Common Issues:

Application crashes on startup: Ensure the latest Microsoft Visual C++ Redistributable Version is installed and configured properly. (https://aka.ms/vs/17/release/vc_redist.x64.exe).

Black screen in rendering area: Verify DirectX is installed and compatible (DirectX 11).

🤝 Contributing

Contributions are welcome! To contribute:

Fork the repository.

Create a new branch (feature-branch).

Commit your changes.

Submit a pull request.

📜 License

This project is licensed under the MIT License.

🚀 Happy coding with Ogre 3D in WPF! 🎮
