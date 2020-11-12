## Xsolla Login Unity Asset

The Xsolla Login SDK allows you to use a ready-made server solution for authenticating users and managing the friend system and user accounts in apps based on [Unity](https://unity.com/).
After integration of Login SDK, you can use  [Xsolla Login](https://developers.xsolla.com/doc/login/). Main features:

*   authentication via username and password
*   authentication via social networks
*   authentication via Facebook and Google apps on Android devices
*   authentication via Steam session_ticket
*   signup
*   email confirmation
*   password reset
*   user attributes management
*   cross-platform account linking
*   token invalidation
*   OAuth 2.0 support
*   friend system 

## WebGL
Usefull links:
* [Unity WebGL docs](https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html)
* [Xsolla PayStation script docs](https://developers.xsolla.com/doc/pay-station/integration-guide/open-payment-ui/#pay_station_guide_pay_station_embed)
* [Xsolla PayStation additional info](https://developers.xsolla.com/doc/pay-station/features/paystation-analytics/#pay_station_features_analytics_ps_events)

To integrate WebGL:
1. Download branch called **feat/WebPaystation**.
2. Add scenes to build it if it does not exist.
3. Switch build settings to WebGL.
4. Build it and run as you want.


**NOTE:** Login API supports CORS. So you need to change CORS settings in [Publisher Account](https://publisher.xsolla.com/signup?store_type=sdk).

## System Requirements

* 64-bit OS
* Windows 7 SP1 and higher
* macOS 10.12 and higher
* A compatible version of Unity:
	* 2018.3.0f2
	* 2019.3.4f1

**NOTE:** We recommend you use the Mono compiler for creating a game build. You can use either Mono or IL2CPP compiler for creating APK.

## Target OS
* iOS
* Android
* macOS
* Windows 64-bit

## Prerequisites

1. [Download Unity](https://store.unity.com/download).
2. Pick a personal or professional Unity license based on your preferences.
3. Create a new Unity project.
4. Register an Xsolla [Publisher Account](https://publisher.xsolla.com/signup?store_type=sdk) and set up a new project. More instructions are on the [Xsolla Developers portal](https://developers.xsolla.com/sdk/game-engines/unity/#unity_sdk_use_xsolla_servers_prerequisites).
5. Go to the [Xsolla Developers portal](https://developers.xsolla.com/sdk/game-engines/unity/#unity_sdk_use_xsolla_servers_login_unity_sdk_integration) to learn how to integrate Xsolla products using  **Xsolla Login SDK for Unity**. 

## Additional resources
* [Xsolla website](http://xsolla.com/)
* [Unity SDKs documentation](https://developers.xsolla.com/sdk/game-engines/unity/)
* [Unity SDKs wiki](https://github.com/xsolla/login-unity-sdk/wiki/)