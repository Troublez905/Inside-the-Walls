using System;
using System.Collections;
using InsideTheWalls.Application;
using InsideTheWalls.Persistence;
using InsideTheWalls.Presentation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InsideTheWalls.UI
{
    public sealed class FrontendController : MonoBehaviour
    {
        private enum ScreenState { Splash, Menu, Settings, Credits, RoleSelect, Prototype }

        private readonly string[] menuLabels = { "NEW GAME", "CONTINUE", "SETTINGS", "CREDITS", "QUIT" };
        private ScreenState state = ScreenState.Splash;
        private Texture2D keyArt;
        private Texture2D splashArt;
        private Texture2D panelTexture;
        private Texture2D accentTexture;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;
        private GUIStyle buttonStyle;
        private GUIStyle buttonSelectedStyle;
        private GUIStyle smallStyle;
        private GUIStyle headingStyle;
        private int selectedIndex;
        private float splashStarted;
        private float masterVolume = 0.8f;
        private float uiScale = 1f;
        private bool reducedMotion;
        private string role = string.Empty;
        private string prototypeMessage = "Report to your assigned station.";
        private PlayableDayController playableDay;
        private bool verticalAxisArmed = true;
        private LocalJsonSaveRepository saveRepository;
        private SaveCompatibilityResult saveCompatibility;
        private int lastSavedObjective = -1;
        private string persistenceStatus = string.Empty;
        private bool verificationSession;

        private void Start()
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (args[i] == "-verifyPopulation")
                {
                    BeginPopulationPreview();
                    gameObject.AddComponent<InsideTheWalls.Characters.PopulationVerification>().Begin(args[i + 1]);
                    return;
                }
                if (args[i] == "-verifyNoah")
                {
                    BeginNoahVerification(args[i + 1]);
                    return;
                }
#endif
                if (args[i] == "-verificationCapture")
                {
                    StartCoroutine(CaptureVerification(args[i + 1]));
                    break;
                }
            }
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public void BeginNoahVerification(string outputDirectory)
        {
            if (!UnityEngine.Application.isPlaying)
                throw new InvalidOperationException("Noah verification requires Play Mode.");
            if (playableDay != null || GetComponent<InsideTheWalls.Characters.NoahAnimationVerification>() != null)
                throw new InvalidOperationException("Start Noah verification from a fresh menu session, not an active game.");
            if (string.IsNullOrWhiteSpace(outputDirectory))
                throw new ArgumentException("A verification output directory is required.", nameof(outputDirectory));

            string fullPath = System.IO.Path.GetFullPath(outputDirectory);
            System.IO.Directory.CreateDirectory(fullPath);
            // Set this before Configure creates the first autosave checkpoint.
            verificationSession = true;
            StartPrototype("INMATE", null, false);
            gameObject.AddComponent<InsideTheWalls.Characters.NoahAnimationVerification>().Begin(fullPath);
        }

        public void BeginPopulationPreview()
        {
            if (!UnityEngine.Application.isPlaying || playableDay != null)
                throw new InvalidOperationException("Start the population preview from a fresh Play Mode menu session.");
            verificationSession = true;
            StartPrototype("INMATE");
            Debug.Log("POPULATION_PREVIEW_STARTED (saves disabled)");
        }
#endif

        private void Awake()
        {
            keyArt = Resources.Load<Texture2D>("UI/InsideTheWalls_KeyArt");
            splashArt = Resources.Load<Texture2D>("UI/BehindTheWalls_EnsembleSplash");
            panelTexture = MakeTexture(new Color(0.035f, 0.055f, 0.07f, 0.92f));
            accentTexture = MakeTexture(new Color(0.82f, 0.34f, 0.08f, 1f));
            masterVolume = PlayerPrefs.GetFloat("ITW.MasterVolume", 0.8f);
            uiScale = PlayerPrefs.GetFloat("ITW.UiScale", 1f);
            reducedMotion = PlayerPrefs.GetInt("ITW.ReducedMotion", 0) == 1;
            saveRepository = new LocalJsonSaveRepository(new PersistentDataSavePathProvider());
            RefreshSaveCompatibility();
            AudioListener.volume = masterVolume;
            splashStarted = Time.unscaledTime;
        }

        private IEnumerator CaptureVerification(string path)
        {
            yield return new WaitForSecondsRealtime(3.2f);
            state = ScreenState.Menu;
            yield return new WaitForEndOfFrame();
            UnityEngine.ScreenCapture.CaptureScreenshot(path);
            yield return new WaitForSecondsRealtime(1f);
            UnityEngine.Application.Quit();
        }

        private void Update()
        {
            if (state == ScreenState.Splash)
            {
                bool skip = Time.unscaledTime - splashStarted >= 0.75f && AnySubmitOrCancel();
                if (skip || Time.unscaledTime - splashStarted >= 2.75f)
                {
                    state = ScreenState.Menu;
                }
                return;
            }

            if (state == ScreenState.Menu)
            {
                HandleMenuNavigation();
            }
            else if (state != ScreenState.Prototype && CancelPressed())
            {
                state = ScreenState.Menu;
            }
        }

        private void LateUpdate()
        {
            // Player input is sampled in Update; consume this frame's interaction only afterwards.
            if (state == ScreenState.Prototype) UpdatePrototype();
        }

        private void OnGUI()
        {
            EnsureStyles();
            float scale = Mathf.Min(Screen.width / 1920f, Screen.height / 1080f) * uiScale;
            scale = Mathf.Clamp(scale, 0.68f, 1.5f);
            Matrix4x4 previous = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
            float width = Screen.width / scale;
            float height = Screen.height / scale;

            if (state != ScreenState.Prototype)
            {
                DrawBackground(width, height);
            }

            switch (state)
            {
                case ScreenState.Splash: DrawSplash(width, height); break;
                case ScreenState.Menu: DrawMenu(width, height); break;
                case ScreenState.Settings: DrawSettings(width, height); break;
                case ScreenState.Credits: DrawCredits(width, height); break;
                case ScreenState.RoleSelect: DrawRoleSelect(width, height); break;
                case ScreenState.Prototype: DrawPrototypeHud(width, height); break;
            }

            GUI.matrix = previous;
        }

        private void DrawBackground(float width, float height)
        {
            GUI.color = Color.white;
            if (state == ScreenState.Splash && splashArt != null)
            {
                GUI.color = Color.black;
                GUI.DrawTexture(new Rect(0f, 0f, width, height), Texture2D.whiteTexture);
                GUI.color = Color.white;
                GUI.DrawTexture(new Rect(0f, 0f, width, height), splashArt, ScaleMode.ScaleToFit);
                return;
            }
            if (keyArt != null)
            {
                float sourceAspect = (float)keyArt.width / keyArt.height;
                float targetAspect = width / height;
                Rect uv = targetAspect > sourceAspect
                    ? new Rect(0f, (1f - sourceAspect / targetAspect) * 0.5f, 1f, sourceAspect / targetAspect)
                    : new Rect((1f - targetAspect / sourceAspect) * 0.5f, 0f, targetAspect / sourceAspect, 1f);
                GUI.DrawTextureWithTexCoords(new Rect(0f, 0f, width, height), keyArt, uv);
            }
            else
            {
                GUI.DrawTexture(new Rect(0f, 0f, width, height), panelTexture);
            }

            GUI.color = new Color(0f, 0f, 0f, 0.18f);
            GUI.DrawTexture(new Rect(0f, 0f, width, height), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        private void DrawSplash(float width, float height)
        {
            GUI.Label(new Rect(70f, height - 78f, 560f, 32f), "v5.o - PRE-BETA RELEASE", smallStyle);
            GUI.Label(new Rect(width - 300f, height - 78f, 230f, 32f), "PRESS ANY BUTTON", smallStyle);
        }

        private void DrawMenu(float width, float height)
        {
            Rect panel = new Rect(64f, Mathf.Max(250f, height - 560f), 470f, 500f);
            GUI.DrawTexture(panel, panelTexture);
            GUI.DrawTexture(new Rect(panel.x, panel.y, 5f, panel.height), accentTexture);
            GUI.Label(new Rect(panel.x + 34f, panel.y + 28f, 380f, 30f), "OFFLINE PROTOTYPE", smallStyle);

            for (int i = 0; i < menuLabels.Length; i++)
            {
                bool enabled = i != 1 || saveCompatibility.CanContinue;
                GUI.enabled = enabled;
                GUIStyle style = i == selectedIndex ? buttonSelectedStyle : buttonStyle;
                if (GUI.Button(new Rect(panel.x + 30f, panel.y + 72f + i * 64f, 400f, 52f), menuLabels[i], style))
                {
                    ActivateMenu(i);
                }
                GUI.enabled = true;
            }

            string continueLabel = saveCompatibility.CanContinue ? "Continue: Saved session ready" : $"Continue: {saveCompatibility.Message}";
            GUI.Label(new Rect(panel.x + 34f, panel.y + 398f, 390f, 30f), continueLabel, smallStyle);
            GUI.Label(new Rect(panel.x + 34f, panel.y + 438f, 390f, 30f), "Nobody leaves unchanged.", labelStyle);
            GUI.Label(new Rect(width - 480f, height - 46f, 440f, 24f), "v5.o - PRE-BETA RELEASE  |  LOCAL", smallStyle);
        }

        private void DrawSettings(float width, float height)
        {
            Rect panel = CenterPanel(width, height, 720f, 610f);
            GUI.DrawTexture(panel, panelTexture);
            GUI.Label(new Rect(panel.x + 42f, panel.y + 32f, 620f, 60f), "SETTINGS", headingStyle);
            GUI.Label(new Rect(panel.x + 42f, panel.y + 118f, 300f, 34f), "MASTER VOLUME", labelStyle);
            masterVolume = GUI.HorizontalSlider(new Rect(panel.x + 42f, panel.y + 164f, 620f, 28f), masterVolume, 0f, 1f);
            AudioListener.volume = masterVolume;
            GUI.Label(new Rect(panel.x + 42f, panel.y + 220f, 300f, 34f), "UI SCALE", labelStyle);
            uiScale = GUI.HorizontalSlider(new Rect(panel.x + 42f, panel.y + 266f, 620f, 28f), uiScale, 0.75f, 1.5f);
            reducedMotion = GUI.Toggle(new Rect(panel.x + 42f, panel.y + 326f, 420f, 42f), reducedMotion, "  REDUCED MOTION", labelStyle);
            GUI.Label(new Rect(panel.x + 42f, panel.y + 382f, 620f, 60f), "Keyboard, mouse, and controller supported.\nInput remapping arrives with the controller slice.", smallStyle);
            if (GUI.Button(new Rect(panel.x + 42f, panel.y + 500f, 280f, 54f), "APPLY & BACK", buttonSelectedStyle))
            {
                SaveSettings();
                state = ScreenState.Menu;
            }
            if (GUI.Button(new Rect(panel.x + 382f, panel.y + 500f, 280f, 54f), "RESTORE DEFAULTS", buttonStyle))
            {
                masterVolume = 0.8f;
                uiScale = 1f;
                reducedMotion = false;
            }
        }

        private void DrawCredits(float width, float height)
        {
            Rect panel = CenterPanel(width, height, 720f, 500f);
            GUI.DrawTexture(panel, panelTexture);
            GUI.Label(new Rect(panel.x + 42f, panel.y + 32f, 620f, 60f), "CREDITS", headingStyle);
            GUI.Label(new Rect(panel.x + 42f, panel.y + 116f, 620f, 230f),
                "BEHIND THE WALLS\n\nCreated by Troublez905\nBuilt with Unity\n\nv5.o - Pre-Beta Release\nReference art pending final rights verification.", labelStyle);
            if (GUI.Button(new Rect(panel.x + 42f, panel.y + 396f, 280f, 54f), "BACK", buttonSelectedStyle))
            {
                state = ScreenState.Menu;
            }
        }

        private void DrawRoleSelect(float width, float height)
        {
            Rect panel = CenterPanel(width, height, 980f, 560f);
            GUI.DrawTexture(panel, panelTexture);
            GUI.Label(new Rect(panel.x + 42f, panel.y + 28f, 880f, 60f), "CHOOSE YOUR FIRST DAY", headingStyle);
            GUI.Label(new Rect(panel.x + 42f, panel.y + 96f, 880f, 50f), "The same institution. Two kinds of responsibility.", labelStyle);
            if (GUI.Button(new Rect(panel.x + 42f, panel.y + 176f, 420f, 230f), "INMATE\n\nLearn the routine. Build trust.\nDecide what a promise costs.", buttonSelectedStyle))
            {
                StartPrototype("INMATE");
            }
            if (GUI.Button(new Rect(panel.x + 518f, panel.y + 176f, 420f, 230f), "OFFICER\n\nHold the post. Read the room.\nChoose when procedure is enough.", buttonStyle))
            {
                StartPrototype("OFFICER");
            }
            if (GUI.Button(new Rect(panel.x + 42f, panel.y + 458f, 240f, 52f), "BACK", buttonStyle))
            {
                state = ScreenState.Menu;
            }
        }

        private void DrawPrototypeHud(float width, float height)
        {
            const float panelHeight = 292f;
            GUI.DrawTexture(new Rect(28f, 28f, 570f, panelHeight), panelTexture);
            GUI.DrawTexture(new Rect(28f, 28f, 5f, panelHeight), accentTexture);
            string time = playableDay != null ? playableDay.TimeLabel : "07:20";
            string objective = playableDay != null ? playableDay.ObjectiveTitle : "REPORT FOR ASSIGNMENT";
            string location = playableDay != null ? playableDay.LocationLabel : "INTAKE";
            string message = playableDay != null ? playableDay.StatusMessage : prototypeMessage;
            GUI.Label(new Rect(54f, 42f, 510f, 26f), $"{role}  |  {time}  |  {location}", smallStyle);
            GUI.Label(new Rect(54f, 72f, 510f, 40f), objective, headingStyle);
            GUI.Label(new Rect(54f, 118f, 510f, 48f), message, smallStyle);
            float nextY = 174f;
            if (!string.IsNullOrEmpty(persistenceStatus))
            {
                GUI.Label(new Rect(54f, nextY, 510f, 26f), persistenceStatus, smallStyle);
                nextY += 30f;
            }

            if (playableDay != null)
            {
                GUI.Label(new Rect(54f, nextY, 510f, 28f), playableDay.ProgressLabel, smallStyle);
                if (!string.IsNullOrEmpty(playableDay.PopulationMessage))
                {
                    Rect dialogue = new Rect(width - 590f, 28f, 560f, 160f);
                    GUI.DrawTexture(dialogue, panelTexture);
                    GUI.DrawTexture(new Rect(dialogue.x, dialogue.y, 5f, dialogue.height), accentTexture);
                    GUI.Label(new Rect(dialogue.x + 22f, dialogue.y + 14f, 516f, 132f), playableDay.PopulationMessage, smallStyle);
                }
                GUI.Label(new Rect(54f, 334f, 680f, 64f), playableDay.IsGuarding
                    ? "GUARDING  |  R / Y: CALM DOWN\nF / X: SHOVE  |  RELEASE G / LB: LOWER GUARD"
                    : "E / A: TALK  |  R / Y: CALM DOWN\nF / X: SHOVE  |  HOLD G / LB: GUARD", smallStyle);
                if (!string.IsNullOrEmpty(playableDay.ProximityPrompt))
                {
                    Rect prompt = new Rect((width - 720f) * 0.5f, height - 148f, 720f, 62f);
                    GUI.DrawTexture(prompt, panelTexture);
                    GUI.DrawTexture(new Rect(prompt.x, prompt.y, 5f, prompt.height), accentTexture);
                    GUI.Label(new Rect(prompt.x + 24f, prompt.y + 14f, prompt.width - 48f, 38f), playableDay.ProximityPrompt, labelStyle);
                }

                if (playableDay.IsComplete)
                {
                    Rect summary = CenterPanel(width, height, 760f, 360f);
                    GUI.DrawTexture(summary, panelTexture);
                    GUI.DrawTexture(new Rect(summary.x, summary.y, 5f, summary.height), accentTexture);
                    GUI.Label(new Rect(summary.x + 38f, summary.y + 32f, 680f, 52f), "DAY COMPLETE", headingStyle);
                    GUI.Label(new Rect(summary.x + 38f, summary.y + 98f, 680f, 160f), playableDay.Summary, labelStyle);
                    GUI.Label(new Rect(summary.x + 38f, summary.y + 286f, 680f, 34f), "ESC / B: RETURN TO MENU", smallStyle);
                }
            }
            GUI.Label(new Rect(width - 520f, height - 58f, 480f, 34f), "WASD / LEFT STICK: MOVE   •   E / A: INTERACT   •   ESC: MENU", smallStyle);
        }

        private void HandleMenuNavigation()
        {
            float vertical = Input.GetAxisRaw("Vertical");
            if (Mathf.Abs(vertical) < 0.3f) verticalAxisArmed = true;
            bool down = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)
                || (verticalAxisArmed && vertical < -0.7f);
            bool up = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)
                || (verticalAxisArmed && vertical > 0.7f);
            if (down || up) verticalAxisArmed = false;
            if (down) selectedIndex = NextEnabled(selectedIndex, 1);
            if (up) selectedIndex = NextEnabled(selectedIndex, -1);
            if (SubmitPressed()) ActivateMenu(selectedIndex);
        }

        private int NextEnabled(int current, int direction)
        {
            int candidate = current;
            do
            {
                candidate = (candidate + direction + menuLabels.Length) % menuLabels.Length;
            } while (candidate == 1 && !saveCompatibility.CanContinue);
            return candidate;
        }

        private void ActivateMenu(int index)
        {
            switch (index)
            {
                case 0: state = ScreenState.RoleSelect; break;
                case 1: ContinuePrototype(); break;
                case 2: state = ScreenState.Settings; break;
                case 3: state = ScreenState.Credits; break;
                case 4:
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    UnityEngine.Application.Quit();
#endif
                    break;
            }
        }

        private void StartPrototype(string selectedRole)
        {
            StartPrototype(selectedRole, null);
        }

        private void StartPrototype(string selectedRole, PlayableDaySnapshot snapshot, bool includePopulation = true)
        {
            role = selectedRole;
            state = ScreenState.Prototype;
            playableDay = gameObject.AddComponent<PlayableDayController>();
            playableDay.Configure(selectedRole == "INMATE" ? InsideTheWalls.Simulation.PlayerRole.Inmate : InsideTheWalls.Simulation.PlayerRole.Officer, includePopulation);
            if (snapshot != null && !playableDay.Restore(snapshot))
            {
                prototypeMessage = "The saved session could not be restored safely.";
                ReloadFrontend();
                return;
            }

            lastSavedObjective = playableDay.ObjectiveIndex;
            if (snapshot == null) SaveCurrentProgress();
        }

        private void ContinuePrototype()
        {
            SaveLoadResult load = saveRepository.Load();
            if (!load.Succeeded || load.Snapshot == null || !load.Snapshot.TryGetRole(out InsideTheWalls.Simulation.PlayerRole savedRole))
            {
                saveCompatibility = new SaveCompatibilityResult(load.Status, load.Message);
                selectedIndex = 0;
                return;
            }

            StartPrototype(savedRole == InsideTheWalls.Simulation.PlayerRole.Inmate ? "INMATE" : "OFFICER", load.Snapshot);
        }

        private void UpdatePrototype()
        {
            if (CancelPressed())
            {
                ReloadFrontend();
                return;
            }

            if (playableDay == null) return;
            playableDay.Tick();
            if (playableDay.ObjectiveIndex != lastSavedObjective)
            {
                SaveCurrentProgress();
            }
        }

        private void SaveCurrentProgress()
        {
            if (playableDay == null) return;
            if (verificationSession)
            {
                lastSavedObjective = playableDay.ObjectiveIndex;
                persistenceStatus = "VERIFICATION SESSION - SAVES DISABLED";
                return;
            }
            SaveWriteResult result = saveRepository.Save(playableDay.CreateSnapshot());
            if (result.Succeeded)
            {
                lastSavedObjective = playableDay.ObjectiveIndex;
                saveCompatibility = saveRepository.Inspect();
                persistenceStatus = "PROGRESS SAVED";
            }
            else
            {
                // Latch this checkpoint so a storage failure does not retry and log every frame.
                lastSavedObjective = playableDay.ObjectiveIndex;
                persistenceStatus = $"SAVE FAILED: {result.Message}";
                Debug.LogError(result.Message);
            }
        }

        private void RefreshSaveCompatibility()
        {
            saveCompatibility = saveRepository.Inspect();
        }

        private void ReloadFrontend()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Destroy(gameObject);
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetFloat("ITW.MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("ITW.UiScale", uiScale);
            PlayerPrefs.SetInt("ITW.ReducedMotion", reducedMotion ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void EnsureStyles()
        {
            if (buttonStyle != null) return;
            Font font = Font.CreateDynamicFontFromOSFont("Bahnschrift", 28) ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleStyle = NewStyle(font, 54, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);
            headingStyle = NewStyle(font, 32, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);
            labelStyle = NewStyle(font, 22, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.92f, 0.93f, 0.91f));
            smallStyle = NewStyle(font, 17, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.78f, 0.82f, 0.8f));
            buttonStyle = NewStyle(font, 25, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.78f, 0.82f, 0.8f));
            buttonStyle.padding = new RectOffset(24, 16, 8, 8);
            buttonStyle.normal.background = panelTexture;
            buttonStyle.hover.background = accentTexture;
            buttonStyle.hover.textColor = Color.white;
            buttonSelectedStyle = new GUIStyle(buttonStyle);
            buttonSelectedStyle.normal.background = accentTexture;
            buttonSelectedStyle.normal.textColor = Color.white;
        }

        private static GUIStyle NewStyle(Font font, int size, FontStyle style, TextAnchor anchor, Color color)
        {
            return new GUIStyle(GUI.skin.label)
            {
                font = font,
                fontSize = size,
                fontStyle = style,
                alignment = anchor,
                normal = { textColor = color },
                wordWrap = true
            };
        }

        private static Texture2D MakeTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        private static Rect CenterPanel(float width, float height, float panelWidth, float panelHeight)
        {
            return new Rect((width - panelWidth) * 0.5f, (height - panelHeight) * 0.5f, panelWidth, panelHeight);
        }

        private static bool AnySubmitOrCancel() => SubmitPressed() || CancelPressed() || Input.GetMouseButtonDown(0);
        private static bool SubmitPressed() => Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.JoystickButton0);
        private static bool CancelPressed() => Input.GetKeyDown(KeyCode.Escape)
            || Input.GetKeyDown(KeyCode.JoystickButton1);
    }
}
