#import <AuthenticationServices/AuthenticationServices.h>

#import "XsollaSDKLoginKitUnity/XsollaSDKLoginKitUnity-Swift.h"
#import "XsollaUtils.h"

#pragma mark - C interface

typedef void(ActionVoidCallbackDelegate)(void *actionPtr);
typedef void(ActionStringCallbackDelegate)(void *actionPtr, const char *data);

extern "C" {

	void _authBySocialNetwork(char* platform, int clientId, char* state, char* redirectUri,
		ActionStringCallbackDelegate authSuccessCallback, void *authSuccessActionPtr,
		ActionStringCallbackDelegate errorCallback, void *errorActionPtr,
		ActionVoidCallbackDelegate cancelCallback, void *cancelActionPtr) {

		NSString* platformString = [XsollaUtils createNSStringFrom:platform];
		NSString* stateString = [XsollaUtils createNSStringFrom:state];
		NSString* redirectUriString = [XsollaUtils createNSStringFrom:redirectUri];

		OAuth2Params *oauthParams = [[OAuth2Params alloc] initWithClientId:clientId
															 state:stateString
															 scope:@"offline"
															 redirectUri:redirectUriString];

		JWTGenerationParams *jwtGenerationParams = [[JWTGenerationParams alloc] initWithGrantType:TokenGrantTypeAuthorizationCode
															 clientId:clientId
															 refreshToken:nil
															 clientSecret:nil
															 redirectUri:redirectUriString];

		if (@available(iOS 13.4, *)) {
			WebAuthenticationPresentationContextProvider* context = [[WebAuthenticationPresentationContextProvider alloc] initWithPresentationAnchor:UnityGetMainWindow()];

			[[LoginKitUnity shared] authBySocialNetwork:platformString oAuth2Params:oauthParams jwtParams:jwtGenerationParams presentationContextProvider:context completion:^(AccessTokenInfo * _Nullable accesTokenInfo, NSError * _Nullable error){

				if(error != nil) {
					NSLog(@"Error code: %ld", error.code);

					if(error.code == NSError.loginKitErrorCodeASCanceledLogin) {
						cancelCallback(cancelActionPtr);
						return;
					}

					errorCallback(errorActionPtr, [XsollaUtils createCStringFrom:error.localizedDescription]);
					return;
				}

				NSString* tokenInfoString = [XsollaUtils serializeTokenInfo:accesTokenInfo];
				authSuccessCallback(authSuccessActionPtr, [XsollaUtils createCStringFrom:tokenInfoString]);
			}];
		} else {
			NSLog(@"Authentication via social networks with Xsolla is not supported for current iOS version.");
		}
    }
}
