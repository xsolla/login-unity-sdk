using UnityEditor;
using UnityEngine;

namespace Xsolla.Core
{
	public partial class XsollaSettingsEditor : UnityEditor.Editor
	{
		private bool XsollaGeneralSettings()
		{
			var changed = false;
			using (new EditorGUILayout.VerticalScope("box"))
			{
				GUILayout.Label("General SDK Settings", EditorStyles.boldLabel);
				XsollaSettings.InAppBrowserEnabled = EditorGUILayout.Toggle("Enable in-app browser?", XsollaSettings.InAppBrowserEnabled);
				var projectId = EditorGUILayout.TextField(new GUIContent("Project ID"),  XsollaSettings.PublisherProjectId);
				if (projectId != XsollaSettings.PublisherProjectId)
				{
					XsollaSettings.PublisherProjectId = projectId;
					changed = true;
				}
			}
			EditorGUILayout.Space();
			
			return changed;
		}
	}
}

