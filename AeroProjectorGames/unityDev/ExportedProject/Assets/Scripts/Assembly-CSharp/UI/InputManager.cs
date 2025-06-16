using System.Diagnostics;
using Authentication;
using Security;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
	public class InputManager : MonoBehaviour
	{
		private const string ALT_MOD_GROUP = "'clear mod1'";

		private const string WIN_MOD_GROUP = "'clear mod4'";

		private const string BASH_EXECUTE_CMD_ARG = "-c";

		private const string COMMAND = "xmodmap";

		private const string EDIT_ARG = "-e";

		[SerializeField]
		private AuthenticationEventsSO authenticationEvents;

		[SerializeField]
		private SecurityModalEventsSO securityEvents;

		private void Awake()
		{
			if (Application.platform == RuntimePlatform.LinuxPlayer)
			{
				string arguments = "-c \"xmodmap -e 'clear mod4'\"";
				string arguments2 = "-c \"xmodmap -e 'clear mod1'\"";
				string arguments3 = "-c \"sed -i 's/Enabled=true/Enabled=false/' ~/.config/lxqt/globalkeyshortcuts.conf\"";
				Process.Start("/bin/bash", arguments);
				Process.Start("/bin/bash", arguments2);
				Process.Start("/bin/bash", arguments3);
			}
		}

		private void Update()
		{
			HandleDisplayAuthenticationHotkey();
			HandleExitHotkey();
		}

		private void HandleDisplayAuthenticationHotkey()
		{
			if (Keyboard.current.ctrlKey.isPressed && Keyboard.current.iKey.wasPressedThisFrame)
			{
				securityEvents.RaisePinAuthenticationRequest(authenticationEvents.RaiseInformationModalRequest);
			}
		}

		private void HandleExitHotkey()
		{
			if (Keyboard.current.ctrlKey.isPressed && Keyboard.current.cKey.wasPressedThisFrame)
			{
				securityEvents.RaisePinAuthenticationRequest(Application.Quit);
			}
		}
	}
}
