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
    3. Use [JWT validation](#jwt-validation) to make authentication more secure (optional).
5. To configure your Unity project:
    1. Go to your Unity project > **Assets > Xsolla** and place *XsollaAuthentication.prefab* to the scene.
    2. Open *XsollaAuthentication.prefab* in **Inspector** and fill in the **Login project ID** field. 
    3. Use [JWT validation](#jwt-validation) to make authentication more secure (optional).
    4. Design the user interface following the [tutorials](https://unity3d.com/ru/learn/tutorials/s/user-interface-ui). 
    5. Set up [events processing](#setting-up-events-processing).

| Field  | Description |
| :--- | :--- |
| Login project ID | Login ID from your Publisher Account. **Required.** |
| JWT validation | Whether the JWT will be validated on your side. |
| JWT validation URL | Server URL to validate the JWT on your side. **Required if the JWT validation box is ticked.** |
| Callback URL | URL to redirect the user to after registration/authentication/password reset. Must be identical to **Callback URL** specified in Publisher Account in Login settings. **Required if there are several Callback URLs.** |

### JWT Validation

A [JWT](https://jwt.io/introduction/) is generated for each successfully authenticated user. This value is signed by the secret key encrypted according to the SHA-256 algorithm. You can set up JWT validation using Firebase. Follow the instructions below:
1. Create the [Firebase](https://firebase.google.com/) project.
2. Install [Node.JS](https://nodejs.org/en/).
3. Install the **firebase-tools** package:
```
$ npm install -g firebase-tools
```
4. Open console, go to the Unity project > **Assets > Xsolla > TokenVerificator** and run the following command: 
```
$ firebase login
```
5. Specify your Firebase authentication data in a browser.
6. Go to the Unity project > **Assets > Xsolla > TokenVerificator**, open the *.firebaserc* file, and check that the Firebase Project ID is correct. **Note:** If you could not find the *.firebaserc* file, set up the display of hidden files on your PC.
7. Go to the Unity project > **Assets > Xsolla > TokenVerificator > functions**.
    1. Open the *config.json* file and paste your secret key. You can find it in your **Publisher Account > Login settings > General settings**.  
    2. Install the xsolla-jwt script to the Firebase project:
    ```
    $ npm install
    $ npm run deploy
    ```
    3. Copy the URL from the console.
9. Open *XsollaAuthentication.prefab* in **Inspector** and paste the copied URL to the **JWT validation URL** field.

**Note:** Login SDK automatically validates and updates the JWT value on the Xsolla’s server. 

### Setting up Events Processing

Processing of events is already set up in the demo project. You can use and change demo settings in **AuthDemo > Assets > Scripts** or set up your own project’s events. The following scripts are available:
* *ChangePasswordPage.cs* – changing user password,
* *CreateAccountPage.cs* – user registration,
* *LoginPage.cs* – user authentication via the email/username and password.

The instructions for setting up an event are given below (here the event is user registration):
1. Initialize events processing using the following command: 
```
XsollaAuthentication.Instance.SignIn(_login_Text.text, _password_Text.text);
```
2. Initialize the event described in the table below and set your name for this event. Example of initializing successful user authentication:
```
XsollaAuthentication.Instance.OnSuccessfulSignIn += {your_name_for_successful_sign_in};
```
3. Add a script to process the event as you want.

Other events are set up in the same way.

All Login Unity SDK event names in response to user actions:

| Event name  | Description |
| :--- | :--- |
| OnSuccessfulRegistration | Request for the user sign up is successfully sent. |
| OnSuccessfulSignIn | Request to authenticate the user is successfully sent. |
| OnSuccessfulResetPassword | Request to reset the password is successfully sent. |
| OnValidToken | JWT is successfully validated. The event is processed if the **JWT validation** field is ticked in prefab settings. |
| OnInvalidToken | JWT validation is failed. The event is processed if the **JWT validation** field is ticked in prefab settings. |
| OnVerificationException | The 422 status code is returned during JWT validation. The event is processed if the **JWT validation** field is ticked in prefab settings. |
| OnCaptchaRequiredException | The user is blocked for 24 hours because there have been 5 unsuccessful authentication attempts made in a row. |
| OnUserIsNotActivated | User email is not confirmed. |
| OnPassworResetingNotAllowedForProject | Password reset is disabled for this Login in Publisher Account. |
| OnRegistrationNotAllowedException | User registration is disabled for this Login in Publisher Account. |
| OnUsernameIsTaken | This username is already taken. |
| OnEmailIsTaken | This email is already taken. |
| OnInvalidProjectSettings | Login with the specified Login ID is not found. |
| OnInvalidLoginOrPassword | Incorrect username or password. |
| OnMultipleLoginUrlsException | Callback URL is not specified in prefab settings for Login with several Callback URLs. |
| OnSubmittedLoginUrlNotFoundException | Callback URL specified in prefab settings is not found for this Login. |
| OnNetworkError | Network error has occurred. |
| OnIdentifiedError | Unidentified error has occurred. |
    
