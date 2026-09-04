using System;
using System.Collections;
using InsideTheWalls.Application;
using InsideTheWalls.Characters;
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
        private int dutyProgress;
        private PrototypePlayerController player;
        private Vector3 stationPosition;

        private void Start()
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "-verificationCapture")
                {
                    StartCoroutine(CaptureVerification(args[i + 1]));
                    break;
                }
            }
        }

        private void Awake()
        {
            keyArt = Resources.Load<Texture2D>("UI/InsideTheWalls_KeyArt");
            panelTexture = MakeTexture(new Color(0.035f, 0.055f, 0.07f, 0.92f));
            accentTexture = MakeTexture(new Color(0.82f, 0.34f, 0.08f, 1f));
            masterVolume = PlayerPrefs.GetFloat("ITW.MasterVolume", 0.8f);
            uiScale = PlayerPrefs.GetFloat("ITW.UiScale", 1f);
            reducedMotion = PlayerPrefs.GetInt("ITW.ReducedMotion", 0) == 1;
            AudioListener.volume = masterVolume;
            splashStarted = Time.unscaledTime;
        }

        private IEnumerator CaptureVerification(string path)
        {
            yield return new WaitForSecondsRealtime(3.2f);
            state = ScreenState.Menu;
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(path);
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
            else if (state == ScreenState.Prototype)
            {
                UpdatePrototype();
            }
            else if (CancelPressed())
            {
                state = ScreenState.Menu;
            }
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
            GUI.Label(new Rect(70f, height - 78f, 500f, 32f), "v2.05 - PLAYABLE ALPHA", smallStyle);
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
                bool enabled = i != 1;
                GUI.enabled = enabled;
                GUIStyle style = i == selectedIndex ? buttonSelectedStyle : buttonStyle;
                if (GUI.Button(new Rect(panel.x + 30f, panel.y + 72f + i * 64f, 400f, 52f), menuLabels[i], style))
                {
                    ActivateMenu(i);
                }
                GUI.enabled = true;
            }

            GUI.Label(new Rect(panel.x + 34f, panel.y + 398f, 390f, 30f), "Continue: No saved session", smallStyle);
            GUI.Label(new Rect(panel.x + 34f, panel.y + 438f, 390f, 30f), "Nobody leaves unchanged.", labelStyle);
            GUI.Label(new Rect(width - 440f, height - 46f, 400f, 24f), "v2.05 - PLAYABLE ALPHA  |  LOCAL", smallStyle);
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
                "INSIDE THE WALLS\n\nCreated by Troublez905\nBuilt with Unity\n\nv2.05 - Playable Alpha\nReference art pending final rights verification.", labelStyle);
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
            GUI.DrawTexture(new Rect(28f, 28f, 470f, 170f), panelTexture);
            GUI.DrawTexture(new Rect(28f, 28f, 5f, 170f), accentTexture);
            GUI.Label(new Rect(54f, 46f, 410f, 32f), $"{role}  |  07:{30 + dutyProgress * 10:00}", smallStyle);
            GUI.Label(new Rect(54f, 80f, 410f, 44f), "THE MISSING TEN MINUTES", headingStyle);
            GUI.Label(new Rect(54f, 132f, 410f, 54f), prototypeMessage, smallStyle);
            GUI.Label(new Rect(width - 520f, height - 58f, 480f, 34f), "WASD / LEFT STICK: MOVE   •   E / A: INTERACT   •   ESC: MENU", smallStyle);
        }

        private void HandleMenuNavigation()
        {
            bool down = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)
                || Input.GetAxisRaw("Vertical") < -0.7f;
            bool up = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)
                || Input.GetAxisRaw("Vertical") > 0.7f;
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
            } while (candidate == 1);
            return candidate;
        }

        private void ActivateMenu(int index)
        {
            switch (index)
            {
                case 0: state = ScreenState.RoleSelect; break;
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
            role = selectedRole;
            state = ScreenState.Prototype;
            BuildPrototypeWorld();
        }

        private void BuildPrototypeWorld()
        {
            foreach (GameObject existing in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (existing != gameObject)
                {
                    Destroy(existing);
                }
            }

            RenderSettings.ambientLight = new Color(0.38f, 0.43f, 0.42f);
            var lightObject = new GameObject("Morning Light");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.25f;
            light.color = new Color(1f, 0.88f, 0.7f);
            lightObject.transform.rotation = Quaternion.Euler(42f, -34f, 0f);

            CreateBlock("Yard", new Vector3(0f, -0.5f, 0f), new Vector3(36f, 1f, 28f), new Color(0.28f, 0.34f, 0.32f));
            CreateBlock("Housing", new Vector3(-13f, 3f, 6f), new Vector3(8f, 7f, 16f), new Color(0.31f, 0.36f, 0.35f));
            CreateBlock("Officer Station", new Vector3(12f, 2f, 8f), new Vector3(7f, 5f, 8f), new Color(0.18f, 0.28f, 0.3f));
            CreateBlock("Fence North", new Vector3(0f, 2f, 14f), new Vector3(36f, 4f, 0.3f), new Color(0.18f, 0.2f, 0.2f));
            CreateBlock("Fence South", new Vector3(0f, 2f, -14f), new Vector3(36f, 4f, 0.3f), new Color(0.18f, 0.2f, 0.2f));
            CreateBlock("Fence East", new Vector3(18f, 2f, 0f), new Vector3(0.3f, 4f, 28f), new Color(0.18f, 0.2f, 0.2f));
            CreateBlock("Fence West", new Vector3(-18f, 2f, 0f), new Vector3(0.3f, 4f, 28f), new Color(0.18f, 0.2f, 0.2f));

            stationPosition = role == "INMATE" ? new Vector3(-7f, 0.75f, -5f) : new Vector3(8f, 0.75f, -4f);
            CreateBlock("Assignment Marker", stationPosition, new Vector3(1.6f, 1.5f, 1.6f), new Color(0.9f, 0.38f, 0.06f));

            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.08f, 0.12f, 0.14f);
            camera.fieldOfView = 62f;

            var playerObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerObject.name = $"{role} Player";
            playerObject.transform.position = new Vector3(0f, 1f, -9f);
            Destroy(playerObject.GetComponent<CapsuleCollider>());
            playerObject.AddComponent<CharacterController>();
            var renderer = playerObject.GetComponent<Renderer>();
            renderer.material.color = role == "INMATE" ? new Color(0.82f, 0.34f, 0.08f) : new Color(0.15f, 0.28f, 0.38f);
            player = playerObject.AddComponent<PrototypePlayerController>();
        }

        private void UpdatePrototype()
        {
            if (CancelPressed())
            {
                ReloadFrontend();
                return;
            }

            if (player != null && player.InteractionPressed && Vector3.Distance(player.transform.position, stationPosition) < 2.5f)
            {
                dutyProgress = Mathf.Min(dutyProgress + 1, 3);
                prototypeMessage = dutyProgress switch
                {
                    1 => role == "INMATE" ? "A permitted message needs delivering. The clock is moving." : "Movement is late. Observe first, then choose your response.",
                    2 => "You record what you actually witnessed — not what the institution assumes.",
                    _ => "Choice recorded. Tomorrow's routine will remember it. Prototype complete."
                };
            }
        }

        private void ReloadFrontend()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Destroy(gameObject);
        }

        private static GameObject CreateBlock(string name, Vector3 position, Vector3 scale, Color color)
        {
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = name;
            block.transform.SetPositionAndRotation(position, Quaternion.identity);
            block.transform.localScale = scale;
            block.GetComponent<Renderer>().material.color = color;
            return block;
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
