# Login Unity SDK

Login Unity SDK is used to integrate [Xsolla Login API](https://developers.xsolla.com/api/v2/login/) methods with apps based on [Unity](https://unity.com/). 

This solution works for:
* storing user data at Xsolla’s side
* authentication via email/username and password

Target OS:
* iOS
* Android
* Linux
* Mac OS
* Windows 32-bit
* Windows 64-bit

**Info:** The integration demo is available in the **Assets > Xsolla > AuthDemo** folder. You can use it as an example.

## System Requirements

* 64-bit OS
* Windows 7 SP1 and higher
* Mac OS 10.12 and higher
* DirectX 10
* Visual Studio 2015
* Unity 2017.4.22 and higher

## Integration Flow

1. Register an [Xsolla Publisher Account](https://publisher.xsolla.com).
2. [Create a project](#creating-a-project) in your Publisher Account.
3. [Set up Login](#setting-up-login) in your Publisher Account.
4. [Install and set up](#installing-and-setting-up-the-plugin) the plugin for the Unity project.

### Creating a Project

1. Log in to Publisher Account.
2. Go to **Projects** and click **Create new project**.
3. In setup mode, add **Project name** and click **Create**.
4. Go to **Project settings > Integration settings** and check that **Tokenless integration** is disabled.

### Setting up Login

1. Create a new Login in your Publisher Account and specify **Login name**.
2. Go to **General settings > URL block**:
    1. Specify the **Callback URL** to redirect the user to after authentication.
    2. Select **Xsolla storage** in the **User data storage** block.

### Installing and Setting up the Plugin

You can complete the following settings using the demo project or your Unity project.

1. [Download the plugin](https://github.com/xsolla/login-unity-sdk).
2. Unpack the archive.
3. Add the package to your project in Unity. Open the *XsollaLogin_v0.2.unitypackage* file from the unpacked archive and click **Import**.
4. To configure the demo project:
    1. Go to the unpacked archive > **Assets > Xsolla > AuthDemo**.
    2. Open *XsollaAuthentication.prefab* in **Inspector** and fill in the **Login project ID** field. 
5. To configure your Unity project:
    1. Go to your Unity project > **Assets > Xsolla** and place *XsollaAuthentication.prefab* to the scene.
    2. Open *XsollaAuthentication.prefab* in **Inspector** and fill in the **Login project ID** field. 
    3. Design the user interface following the [tutorials](https://unity3d.com/ru/learn/tutorials/s/user-interface-ui). 
    4. Set up [events processing](#setting-up-events-processing).

| Field  | Description |
| :--- | :--- |
| Login project ID | Login ID from your Publisher Account. **Required.** |
| JWT validation | Whether the JWT will be validated on your side. |
| JWT validation URL | Server URL to validate the JWT on your side. **Required if the JWT validation box is ticked.** |
| Callback URL | URL to redirect the user to after registration/authentication/password reset. Must be identical to **Callback URL** specified in Publisher Account in Login settings. **Required if there are several Callback URLs.** |
