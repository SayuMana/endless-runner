using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EndlessRunner.EditorTools
{
    public static class EndlessRunnerBuilder
    {
        const string ScenePath = "Assets/Scenes/Main.unity";
        const string BalanceDir = "Assets/Data";
        const string BalancePath = "Assets/Data/GameBalance.asset";
        const string PrefKey = "EndlessRunner.BuilderVersion";
        const int BuildVersion = 5;

        static readonly Color BackgroundColor = new Color(0.07f, 0.08f, 0.12f);
        static readonly Color TextColor = new Color(0.95f, 0.96f, 1f);
        static readonly Color HintColor = new Color(0.62f, 0.66f, 0.75f);

        [MenuItem("Tools/Endless Runner/Build Main Scene")]
        public static void BuildFromMenu()
        {
            if (Build()) EditorPrefs.SetInt(PrefKey, BuildVersion);
        }

        public static void BuildSceneAndWebGL()
        {
            if (Build()) EditorPrefs.SetInt(PrefKey, BuildVersion);
            WebGLBuilder.Build();
        }

        [InitializeOnLoadMethod]
        static void AutoBuild()
        {
            if (EditorPrefs.GetInt(PrefKey, 0) >= BuildVersion) return;

            EditorApplication.delayCall += () =>
            {
                if (Build()) EditorPrefs.SetInt(PrefKey, BuildVersion);
                else Debug.LogWarning("[Builder] build failed, will retry after next reload");
            };
        }

        static bool Build()
        {
            try
            {
                Debug.Log("[Builder] start");

                EnsureFolder(BalanceDir);
                GameBalance balance = LoadOrCreateBalance();

                EditorSceneManager.SaveOpenScenes();
                Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

                BuildCamera();
                LaneController player = BuildPlayer(balance);
                ObstacleSpawner spawner = BuildSpawner(balance);
                ScoreTracker score = BuildScoreTracker(balance);
                HudView hud = BuildHud();
                BuildManager(balance, player, spawner, score, hud);
                BuildGround();

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene, ScenePath);
                AddSceneToBuildSettings();
                ApplyWebGlSettings();
                AssetDatabase.SaveAssets();

                Debug.Log("[Builder] OK roots=" + scene.rootCount + " scene=" + ScenePath);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("[Builder] FAILED " + e);
                return false;
            }
        }

        // --- Scene objects ---

        static void BuildCamera()
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";

            var camera = go.AddComponent<UnityEngine.Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 6f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = BackgroundColor;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 50f;

            go.transform.position = new Vector3(0f, 0f, -10f);
            go.AddComponent<CameraFitter>();
        }

        static LaneController BuildPlayer(GameBalance balance)
        {
            var go = new GameObject("Player");
            go.tag = "Player";
            go.transform.position = new Vector3(balance.laneX[1], -4.2f, 0f);

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = 10;

            var body = go.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Dynamic;
            body.gravityScale = 0f;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var collider = go.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(28f / 32f, 40f / 32f) * balance.hitboxScale;

            var controller = go.AddComponent<LaneController>();
            Wire(controller, "balance", balance);
            return controller;
        }

        static ObstacleSpawner BuildSpawner(GameBalance balance)
        {
            var go = new GameObject("ObstacleSpawner");
            var spawner = go.AddComponent<ObstacleSpawner>();
            Wire(spawner, "balance", balance);
            return spawner;
        }

        static ScoreTracker BuildScoreTracker(GameBalance balance)
        {
            var go = new GameObject("ScoreTracker");
            var score = go.AddComponent<ScoreTracker>();
            Wire(score, "balance", balance);
            return score;
        }

        static void BuildManager(GameBalance balance, LaneController player, ObstacleSpawner spawner, ScoreTracker score, HudView hud)
        {
            var go = new GameObject("GameManager");
            var manager = go.AddComponent<GameManager>();
            Wire(manager, "balance", balance);
            Wire(manager, "player", player);
            Wire(manager, "spawner", spawner);
            Wire(manager, "score", score);
            Wire(manager, "hud", hud);
        }

        static void BuildGround()
        {
            var go = new GameObject("Ground");
            go.transform.position = new Vector3(0f, -5.4f, 0f);

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = -5;

            go.AddComponent<GroundView>();
        }

        // --- UI ---

        static HudView BuildHud()
        {
            var canvasGo = new GameObject("HUD");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(690f, 900f);
            scaler.matchWidthOrHeight = 0.5f;

            var hud = canvasGo.AddComponent<HudView>();

            Text scoreText = CreateText(canvasGo.transform, "Score", "0", 58, TextAnchor.UpperCenter,
                new Vector2(0f, -46f), new Vector2(460f, 80f), TextColor, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));

            GameObject menuPanel = CreatePanel(canvasGo.transform, "MenuPanel", new Color(0f, 0f, 0f, 0.72f));
            CreateWideText(menuPanel.transform, "Title", "ENDLESS RUNNER", 76, 0.5f, 150f, 120f, TextColor, TextAnchor.MiddleCenter);
            CreateWideText(menuPanel.transform, "Start", "TAP TO START", 44, 0.5f, 28f, 80f, TextColor, TextAnchor.MiddleCenter);
            CreateWideText(menuPanel.transform, "Hint", "SWIPE OR ARROW KEYS", 25, 0.5f, -56f, 60f, HintColor, TextAnchor.MiddleCenter);

            GameObject gameOverPanel = CreatePanel(canvasGo.transform, "GameOverPanel", new Color(0f, 0f, 0f, 0.78f));
            CreateWideText(gameOverPanel.transform, "Title", "GAME OVER", 72, 0.5f, 160f, 110f, TextColor, TextAnchor.MiddleCenter);
            Text finalScore = CreateWideText(gameOverPanel.transform, "FinalScore", "SCORE 0", 60, 0.5f, 56f, 90f, TextColor, TextAnchor.MiddleCenter);
            CreateWideText(gameOverPanel.transform, "Restart", "TAP TO RESTART", 38, 0.5f, -66f, 70f, HintColor, TextAnchor.MiddleCenter);

            gameOverPanel.SetActive(false);

            Wire(hud, "scoreText", scoreText);
            Wire(hud, "menuPanel", menuPanel);
            Wire(hud, "gameOverPanel", gameOverPanel);
            Wire(hud, "gameOverScoreText", finalScore);

            return hud;
        }

        static Text CreateWideText(Transform parent, string name, string content, int size, float anchorY,
            float offsetY, float height, Color color, TextAnchor alignment)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var text = go.AddComponent<Text>();
            text.font = BuiltinFont();
            text.text = content;
            text.fontSize = size;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Mathf.Max(12, size / 2);
            text.resizeTextMaxSize = size;

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, anchorY);
            rect.anchorMax = new Vector2(1f, anchorY);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(-80f, height);
            rect.anchoredPosition = new Vector2(0f, offsetY);
            return text;
        }

        static GameObject CreatePanel(Transform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.color = color;

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return go;
        }

        static Text CreateText(Transform parent, string name, string content, int size, TextAnchor alignment,
            Vector2 position, Vector2 boxSize, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var text = go.AddComponent<Text>();
            text.font = BuiltinFont();
            text.text = content;
            text.fontSize = size;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = boxSize;
            rect.anchoredPosition = position;
            return text;
        }

        static Font BuiltinFont()
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            if (font == null) Debug.LogError("[Builder] builtin font not found");
            return font;
        }

        // --- Assets and settings ---

        static GameBalance LoadOrCreateBalance()
        {
            var existing = AssetDatabase.LoadAssetAtPath<GameBalance>(BalancePath);
            if (existing != null) return existing;

            var created = ScriptableObject.CreateInstance<GameBalance>();
            AssetDatabase.CreateAsset(created, BalancePath);
            Debug.Log("[Builder] created " + BalancePath);
            return created;
        }

        static void AddSceneToBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
            Debug.Log("[Builder] build settings scenes=" + EditorBuildSettings.scenes.Length);
        }

        static void ApplyWebGlSettings()
        {
            PlayerSettings.companyName = "SayuMana";
            PlayerSettings.productName = "Endless Runner";
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
            PlayerSettings.WebGL.decompressionFallback = true;
            PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.WebGL, ManagedStrippingLevel.High);
            PlayerSettings.stripEngineCode = true;
            PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.WebGL, Il2CppCompilerConfiguration.Release);
            Debug.Log("[Builder] webgl settings applied");
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;

            string parent = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(parent)) parent = parent.Replace("\\", "/");
            string name = Path.GetFileName(path);

            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }

        static void Wire(UnityEngine.Object target, string fieldName, UnityEngine.Object value)
        {
            var serialized = new SerializedObject(target);
            var property = serialized.FindProperty(fieldName);

            if (property == null)
            {
                Debug.LogError("[Builder] missing field " + target.GetType().Name + "." + fieldName);
                return;
            }

            property.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
