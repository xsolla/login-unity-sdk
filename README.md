# Login & Account System asset for Unity

Integrate the Login & Account System asset into your application to manage user access, connect them via the friend system, and use other features of [Xsolla Login](https://developers.xsolla.com/doc/login/).

[Try our demo to learn more](https://livedemo.xsolla.com/sdk/unity/webgl/).


![Login demo](https://i.imgur.com/0hFIFvh.png "Login demo")


<div style="background-color: WhiteSmoke">
<p><b>Note:</b></p>
<p>
Login & Account System asset is already included as part of the <a href="https://assetstore.unity.com/packages/tools/integration/xsolla-game-commerce-145141">Game Commerce</a> asset. You can download the Game Commerce if you need a broader set of features, but do <b>NOT</b> install these plugins separately.
</p>
</div>

For a better understanding of which Xsolla asset to choose, use the table:

<table>
  <tr>
   <td>
   </td>
   <td style="text-align: center"><b>Game Commerce asset</b>
   </td>
   <td style="text-align: center"><b>Login & Account System asset</b>
   </td>
   <td style="text-align: center"><b>Cross-Buy asset</b>
   </td>
  </tr>
  <tr>
   <td colspan="4" ><b>In-game store</sb>
   </td>
  </tr>
  <tr>
   <td>
    Virtual currency
   </td>
   <td>+
   </td>
   <td>
   </td>
   <td>
   </td>
  </tr>
  <tr>
   <td>
    Virtual items
   </td>
   <td>+
   </td>
   <td>
   </td>
   <td>
   </td>
  </tr>
  <tr>
   <td>
    Player inventory
   </td>
   <td>+
   </td>
   <td>
   </td>
   <td>+
   </td>
  </tr>
  <tr>
   <td>
    Bundles
   </td>
   <td>+
   </td>
   <td>
   </td>
   <td>
   </td>
  </tr>
  <tr>
   <td>
    Promotional campaigns
   </td>
   <td>+
   </td>
   <td>
   </td>
   <td>
   </td>
  </tr>
  <tr>
   <td colspan="4" ><b>Login</b>
   </td>
  </tr>
  <tr>
   <td>
    Authentication
   </td>
   <td>+
   </td>
   <td>+
   </td>
   <td>+
   </td>
  </tr>
  <tr>
   <td>
    User management
   </td>
   <td>+
   </td>
   <td>+
   </td>
   <td>+
   </td>
  </tr>
  <tr>
   <td><strong>Payment UI</strong>
   </td>
   <td>+
   </td>
   <td>
   </td>
   <td>
   </td>
  </tr>
  <tr>
  <td colspan="4" ><b>Additional features</b>
   </td>
  </tr>
  <tr>
   <td>
    UI builder
   </td>
   <td>
    +
   </td>
   <td>
    +
   </td>
   <td>
    +
   </td>
  </tr>
  <tr>
   <td>
    Battle pass
   </td>
   <td>
    +
   </td>
   <td>
   </td>
   <td>
   </td>
  </tr>
</table>


## Requirements


### System requirements

*   64-bit OS
*   Windows 7 SP1 and later
*   macOS 10.12 and later
*   The version of Unity not earlier than 2019.4.19f1


### Target OS

*   iOS
*   Android
*   macOS
*   Windows 64-bit

Additionally, the asset supports [creating WebGL build](https://developers.xsolla.com/sdk/unity/how-tos/application-build/#unity_sdk_how_to_build_webgl) to run your application in a browser.

<div style="background-color: WhiteSmoke">
<p><b>Note:</b> We recommend you use the Mono compiler for desktop platforms as it's compatible with the provided in-game browser. If you use other browser solutions, you can use the IL2CPP compiler instead. To create game builds for Android, you can use either Mono or IL2CPP compilers.</p>
</div>


## Integration

Before you integrate the asset, you need to sign up to [Publisher Account](https://publisher.xsolla.com/signup?store_type=sdk) and set up a new project.

More instructions are on the [Xsolla Developers portal](https://developers.xsolla.com/sdk/unity/login/).


## Usage 

To manage the features of Xsolla products, the asset contains a set of classes, methods, prefabs, etc. that let you make requests to the [Login API](https://developers.xsolla.com/login-api/). Use the [tutorials](https://developers.xsolla.com/sdk/unity/tutorials/) to learn how you can use the [asset methods](https://developers.xsolla.com/sdk-code-references/unity-store/).

## Known issues

### Conflict of multiple precompiled assemblies with Newtonsoft.json.dll

#### Issue description

The issue appears when importing the asset on Unity version 2020.3.10f1 and later. The following error message is displayed:

>Multiple precompiled assemblies with the same name Newtonsoft.json.dll included on the current platform. Only one assembly with the same name is allowed per platform.

The conflict arises because the `Newtonsoft.json.dll` library is included in both the Unity Editor and the asset. The library is included in the versions 2020.3.10f1 and later of the editor. And the asset includes the library to support the earlier versions of Unity Editor.

**Issue status:** Fixed in 0.6.4.4.


#### Workaround

1. Remove the `Newtonsoft.json.dll` library from the asset:
    1. Create a new Unity project.
    2. Install [Login & Account System asset](https://assetstore.unity.com/packages/slug/184991) from Unity Asset Store.
    3. Go to  `Assets\Xsolla\Core\Browser\XsollaBrowser\Plugins` directory.
    4. Remove `Newtonsoft.Json.dll` and `Newtonsoft.Json.dll.mdb` files.
2. Restart Unity Editor.

### Newtonsoft.json.dll could not be found

#### Issue description

The problem appears if you upgraded a pre-existing project to Unity version 2020.3.10f1 and later. Importing an asset from the [Unity Asset Store](https://assetstore.unity.com/publishers/12995) into such a project is accompanied by many error messages like this:

>The type or namespace name ‘Newtonsoft’ could not be found (are you missing a using directive or an assembly reference?)


The problem occurs because the `Newtonsoft.json.dll` library is not included in the asset for Unity version 2020.3.10f1 and later. As part of the editor, the library is supplied for versions 2020.3.10f1 and later, but when updating the project for these versions, the library requires manual installation.

**Issue status:** Fixed in 0.6.4.4.

#### Workaround

Install the `Newtonsoft.json.dll` library manually using the <a href="https://docs.unity3d.com/Packages/com.unity.package-manager-ui@1.8/manual/index.html">Unity Package Manager</a>.

### Unable to resolve reference UnityEditor.iOS.Extensions.Xcode

#### Issue description

The issue appears when using External Dependency Manager on Unity version 2020.1.0f1 and later.

When building the application, an error message is displayed:


>Assembly 'Packages/com.google.external-dependency-manager/ExternalDependencyManager/Editor/Google.IOSResolver_v1.2.161.dll' will not be loaded due to errors:
Unable to resolve reference 'UnityEditor.iOS.Extensions.Xcode'. Is the assembly missing or incompatible with the current platform?
Reference validation can be disabled in the Plugin Inspector.

**Issue status:** Fixed in 0.6.4.5.

#### Workaround

Install iOS Build Support module from Unity Hub.

### Error occurred running Unity content on page of WebGL build

#### Issue description
 The issue may appear when logging in WebGL build. The following error message is displayed:

![WebGL error message](https://i.imgur.com/me3ADT4.png "WebGL error message")

See details on cause of the issue on [Unity Issue Tracker](https://issuetracker.unity3d.com/issues/il2cpp-notsupportedexceptions-exception-is-thrown-in-build-with-newtonsoft-dot-json-plugin).

**Issue status:** Won’t fix.

## Legal info

[Explore legal information](https://developers.xsolla.com/sdk/unity/login/get-started/#sdk_legal_compliance) that helps you work with Xsolla.

Xsolla offers the necessary tools to help you build and grow your gaming business, including personalized support at every stage. The terms of payment are determined by the contract that you can sign in Xsolla Publisher Account.

---


### License

See the [LICENSE](https://github.com/xsolla/login-unity-sdk/blob/master/LICENSE.txt) file.

### Community

[Join our Discord server](https://discord.gg/auNFyzZx96) and connect with the Xsolla team and developers who use Xsolla products.

### Additional resources

*   [Xsolla official website](https://xsolla.com/)
*   [Developers documentation](https://developers.xsolla.com/sdk/unity/)
*   [Code reference](https://developers.xsolla.com/sdk-code-references/unity-store/)
*   [API reference](https://developers.xsolla.com/login-api/)