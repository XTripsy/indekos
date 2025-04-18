﻿using UnityEngine;
using UnityEngine.SceneManagement;

namespace AC
{

	[HelpURL("https://www.adventurecreator.org/scripting-guide/class_a_c_1_1_multi_scene_checker.html")]
	public class MultiSceneChecker : MonoBehaviour
	{

		#region Variables
		
		[SerializeField] protected CallStartupProcess callStartupProcess = CallStartupProcess.Start;
		[SerializeField] protected bool forceAsMain;
		protected enum CallStartupProcess { Start, FirstFrameUpdate };
		protected bool runStart = false;

		#endregion


		#region UnityStandards

		protected void Awake ()
		{
			if (!IsMain)
			{
				RegisterAsSubScene ();
				return;
			}

			if (!TestManagerPresence ())
			{
				return;
			}

			#if UNITY_EDITOR
			if (!TestOwnComponents ())
			{
				return;
			}
			#endif

			RegisterAsMain ();
		}


		protected void Start ()
		{
			if (callStartupProcess == CallStartupProcess.Start)
			{
				RunStartProcess ();
			}
		}


		protected void Update ()
		{
			if (callStartupProcess == CallStartupProcess.FirstFrameUpdate)
			{
				RunStartProcess ();
			}
		}


		private void OnEnable ()
		{
			if (IsMain && KickStarter.stateHandler)
			{
				KickStarter.stateHandler.Register (GetComponent<KickStarter> ());
			}
		}


		private void OnDisable ()
		{
			if (KickStarter.stateHandler)
			{
				KickStarter.stateHandler.Unregister (GetComponent<KickStarter> ());
			}
		}

		#endregion


		#region PublicFunctions

		public void RegisterAsMain ()
		{
			GetComponent <KickStarter>().Initialise ();
			runStart = true; // This is necessary because switching the active scene will cause Start to be re-run
		}


		public void RegisterAsSubScene ()
		{
			GameObject subSceneOb = new GameObject ();
			SubScene newSubScene = subSceneOb.AddComponent <SubScene>();
			newSubScene.Initialise (this);
		}

		#endregion


		#region StaticFunctions

		public static MultiSceneChecker GetSceneInstance (Scene scene)
		{
			MultiSceneChecker[] multiSceneCheckers = UnityVersionHandler.FindObjectsOfType<MultiSceneChecker>();
			foreach (MultiSceneChecker multiSceneChecker in multiSceneCheckers)
			{
				if (multiSceneChecker.gameObject.scene == scene)
				{
					return multiSceneChecker;
				}
			}

			return null;
		}

		#endregion


		#region ProtectedFunctions

		protected void RunStartProcess ()
		{
			if (!runStart) return;
			runStart = false;

			if (IsMain && KickStarter.settingsManager && KickStarter.saveSystem)
			{
				if (KickStarter.settingsManager.IsInLoadingScene ())
				{
					ACDebug.Log ("Bypassing regular AC startup because the current scene is the 'Loading' scene.");
					return;
				}

				KickStarter.saveSystem.InitAfterLoad ();
			}
		}


		protected bool TestManagerPresence ()
		{
			References references = Resource.References;
			if (references)
			{
				SceneManager sceneManager = KickStarter.sceneManager;
				SettingsManager settingsManager = KickStarter.settingsManager;
				ActionsManager actionsManager = KickStarter.actionsManager;
				InventoryManager inventoryManager = KickStarter.inventoryManager;
				VariablesManager variablesManager = KickStarter.variablesManager;
				SpeechManager speechManager = KickStarter.speechManager;
				CursorManager cursorManager = KickStarter.cursorManager;
				MenuManager menuManager = KickStarter.menuManager;

				string missingManagers = string.Empty;
				if (sceneManager == null)
				{
					if (!string.IsNullOrEmpty (missingManagers)) missingManagers += ", ";
					missingManagers += "Scene";
				}
				if (settingsManager == null)
				{
					if (!string.IsNullOrEmpty (missingManagers)) missingManagers += ", ";
					missingManagers += "Settings";
				}
				if (actionsManager == null)
				{
					if (!string.IsNullOrEmpty (missingManagers)) missingManagers += ", ";
					missingManagers += "Actions";
				}
				if (variablesManager == null)
				{
					if (!string.IsNullOrEmpty (missingManagers)) missingManagers += ", ";
					missingManagers += "Variables";
				}
				if (inventoryManager == null)
				{
					if (!string.IsNullOrEmpty (missingManagers)) missingManagers += ", ";
					missingManagers += "Inventory";
				}
				if (speechManager == null)
				{
					if (!string.IsNullOrEmpty (missingManagers)) missingManagers += ", ";
					missingManagers += "Speech";
				}
				if (cursorManager == null)
				{
					if (!string.IsNullOrEmpty (missingManagers)) missingManagers += ", ";
					missingManagers += "Cursor";
				}
				if (menuManager == null)
				{
					if (!string.IsNullOrEmpty (missingManagers)) missingManagers += ", ";
					missingManagers += "Menu";
				}

				if (!string.IsNullOrEmpty (missingManagers))
				{
					ACDebug.LogError ("Unassigned AC Manager(s): " + missingManagers + " - all Managers must be assigned in the AC Game Editor window for AC to initialise");
					return false;
				}
			}
			else
			{
				ACDebug.LogError ("No References object found. Please set one using the main Adventure Creator window");
				return false;
			}

			return true;
		}

		#endregion


		#if UNITY_EDITOR

		protected bool TestOwnComponents ()
		{
			bool testResult = true;

			if (GetComponent<MenuSystem> () == null)
			{
				ACDebug.LogError (name + " has no MenuSystem component attached.", this);
				testResult = false;
			}
			if (GetComponent<Dialog> () == null)
			{
				ACDebug.LogError (name + " has no Dialog component attached.", this);
				testResult = false;
			}
			if (GetComponent<PlayerInput> () == null)
			{
				ACDebug.LogError (name + " has no PlayerInput component attached.", this);
				testResult = false;
			}
			if (GetComponent<PlayerInteraction> () == null)
			{
				ACDebug.LogError (name + " has no PlayerInteraction component attached.", this);
				testResult = false;
			}
			if (GetComponent<PlayerMovement> () == null)
			{
				ACDebug.LogError (name + " has no PlayerMovement component attached.", this);
				testResult = false;
			}
			if (GetComponent<PlayerCursor> () == null)
			{
				ACDebug.LogError (name + " has no PlayerCursor component attached.", this);
				testResult = false;
			}
			if (GetComponent<PlayerQTE> () == null)
			{
				ACDebug.LogError (name + " has no PlayerQTE component attached.", this);
				testResult = false;
			}
			if (GetComponent<SceneSettings> () == null)
			{
				ACDebug.LogError (name + " has no SceneSettings component attached.", this);
				testResult = false;
			}
			if (GetComponent<NavigationManager> () == null)
			{
				ACDebug.LogError (name + " has no NavigationManager component attached.", this);
				testResult = false;
			}
			if (GetComponent<ActionListManager> () == null)
			{
				ACDebug.LogError (name + " has no ActionListManager component attached.", this);
				testResult = false;
			}
			if (GetComponent<EventManager> () == null)
			{
				ACDebug.LogError (name + " has no EventManager component attached.", this);
				testResult = false;
			}

			return testResult;
		}


		/**
		 * <summary>Allows the Scene and Variables Managers to show UI controls for the currently-active scene, if multiple scenes are being edited.</summary>
		 * <returns>The name of the currently-open scene.</summary>
		 */
		public static string EditActiveScene ()
		{
			string openScene = UnityVersionHandler.GetCurrentSceneName ();

			if (!string.IsNullOrEmpty (openScene) && !Application.isPlaying)
			{
				KickStarter kickStarter = UnityVersionHandler.FindObjectOfType<KickStarter> ();
				if (kickStarter)
				{
					kickStarter.ClearVariables ();
				}
			}

			return openScene;
		}

		#endif


		#region GetSet

		public bool IsMain => forceAsMain || UnityVersionHandler.ObjectIsInActiveScene (gameObject);

		#endregion
		
	}

}