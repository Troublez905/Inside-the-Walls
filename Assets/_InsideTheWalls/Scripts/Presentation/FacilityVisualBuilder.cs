using System.Collections.Generic;
using UnityEngine;

namespace InsideTheWalls.Presentation
{
    internal static class FacilityVisualBuilder
    {
        // Palette sampled from Docs/Art next-needed reference boards (reference only; not production textures).
        private static readonly Color Wall = new Color(0.48f, 0.55f, 0.54f);
        private static readonly Color WallBase = new Color(0.14f, 0.16f, 0.17f);
        private static readonly Color FloorVinyl = new Color(0.44f, 0.49f, 0.46f);
        private static readonly Color Metal = new Color(0.12f, 0.17f, 0.18f);
        private static readonly Color Blue = new Color(0.18f, 0.32f, 0.38f);
        private static readonly Color PropertyBin = new Color(0.22f, 0.40f, 0.55f);
        private static readonly Color DoorLeaf = new Color(0.30f, 0.38f, 0.44f);
        private static readonly Color Linen = new Color(0.86f, 0.87f, 0.84f);
        private static readonly Color Orange = new Color(0.78f, 0.3f, 0.07f);
        private static readonly Color Wood = new Color(0.35f, 0.25f, 0.16f);
        private static readonly Color Safety = new Color(0.88f, 0.63f, 0.08f);
        private static readonly Color Light = new Color(1f, 0.82f, 0.51f);
        private static readonly Color Asphalt = new Color(0.28f, 0.32f, 0.31f);
        private static readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>();
        private static readonly Dictionary<bool, Texture2D> SurfaceTextures = new Dictionary<bool, Texture2D>();

        public static void Build()
        {
            ResetMaterials();
            Block("Facility Ground", new Vector3(0f, -0.55f, 0f), new Vector3(40f, 1f, 30f), new Color(0.18f, 0.24f, 0.2f));
            Block("Main Walk", new Vector3(0f, 0.01f, 0f), new Vector3(30f, 0.06f, 3f), FloorVinyl);
            Block("West Walk", new Vector3(-12f, 0.02f, 0f), new Vector3(3f, 0.06f, 20f), FloorVinyl);
            Block("East Walk", new Vector3(12f, 0.02f, 0f), new Vector3(3f, 0.06f, 20f), FloorVinyl);
            AddRouteMarkings();

            Room("Intake", new Vector3(-14f, 0f, -9f), new Vector2(9f, 7f), FloorVinyl, "INTAKE / PROPERTY");
            Block("Intake Desk", new Vector3(-14f, 0.55f, -7.5f), new Vector3(4.8f, 1.1f, 0.8f), Blue);
            DecorativeBlock("Intake Counter Cap", new Vector3(-14f, 1.15f, -7.5f), new Vector3(5.1f, 0.12f, 1f), Metal);
            DecorativeBlock("Intake Utility Conduit", new Vector3(-18.15f, 2.15f, -9f), new Vector3(0.12f, 0.12f, 5.4f), Metal);
            AddPropertyBinStack(new Vector3(-17.1f, 0f, -8.4f));
            AddPropertyBinStack(new Vector3(-17.1f, 0f, -9.6f));

            Room("Housing A", new Vector3(-13f, 0f, 7f), new Vector2(11f, 9f), FloorVinyl, "HOUSING A / COUNT");
            for (int i = 0; i < 3; i++)
            {
                float z = 4.5f + i * 2.1f;
                Block($"Bunk {i + 1} Lower", new Vector3(-16.7f, 0.35f, z), new Vector3(2.1f, 0.35f, 0.85f), Blue);
                Block($"Bunk {i + 1} Upper", new Vector3(-16.7f, 1.35f, z), new Vector3(2.1f, 0.25f, 0.85f), Blue);
                DecorativeBlock($"Bunk {i + 1} Frame", new Vector3(-17.65f, 0.85f, z), new Vector3(0.10f, 1.8f, 0.92f), Metal);
            }
            Block("Dayroom Table", new Vector3(-10.4f, 0.45f, 7.4f), new Vector3(2.3f, 0.25f, 1.4f), Metal);

            Room("Dining", new Vector3(-3f, 0f, 9f), new Vector2(7f, 7f), FloorVinyl, "DINING / SERVICE");
            AddDiningTable(new Vector3(-1.2f, 0f, 9f));
            AddDiningTable(new Vector3(-4.8f, 0f, 10.5f));
            Block("Service Edge", new Vector3(-3f, 0.65f, 11.6f), new Vector3(4.8f, 1.3f, 0.55f), Metal);
            DecorativeBlock("Service Canopy", new Vector3(-3f, 2.25f, 11.65f), new Vector3(5.2f, 0.12f, 0.9f), Metal);

            Room("Laundry", new Vector3(13f, 0f, 8f), new Vector2(9f, 8f), FloorVinyl, "LAUNDRY / WORK DETAIL");
            for (int i = 0; i < 3; i++)
            {
                AddLaundryCart(new Vector3(10.7f + i * 2.3f, 0f, 9.6f), loaded: i == 1);
                DecorativeDisc($"Laundry Machine Door {i + 1}", new Vector3(10.7f + i * 2.3f, 0.65f, 9.02f));
            }
            Block("Sorting Table", new Vector3(13f, 0.6f, 5.8f), new Vector3(4.6f, 0.25f, 1.4f), Metal);

            Room("Officer Station", new Vector3(13f, 0f, -7f), new Vector2(9f, 7f), FloorVinyl, "UNIT A / OFFICER POST");
            Block("Movement Board", new Vector3(13f, 1.25f, -4.2f), new Vector3(4.8f, 2.1f, 0.2f), Metal);
            for (int i = -2; i <= 2; i++)
                DecorativeBlock($"Movement Board Line {i + 3}", new Vector3(13f, 1.25f + i * 0.3f, -4.08f), new Vector3(4.1f, 0.025f, 0.02f), new Color(0.53f, 0.68f, 0.64f));
            Block("Report Desk", new Vector3(13f, 0.55f, -5.4f), new Vector3(3.8f, 1.1f, 1.1f), Wood);
            // Fixed service-door dressing on the existing back wall, not the controlled opening.
            AddImportedModel("SecureDoor", new Vector3(16f, 0.08f, -3.72f), Quaternion.identity);

            Block("Yard Surface", new Vector3(0f, 0.04f, -4f), new Vector3(10f, 0.08f, 8f), Asphalt);
            AddYardMarkings();
            AddImportedBench("Yard Bench West", -3.2f);
            AddImportedBench("Yard Bench East", 3.2f);
            AddImportedYardTiles();
            AddExerciseRail(new Vector3(0f, 0f, -6.9f));
            DecorativeBlock("Yard Center Line", new Vector3(0f, 0.095f, -4f), new Vector3(0.08f, 0.015f, 7f), Safety);
            Sign("CENTRAL YARD", new Vector3(0f, 2.5f, -7.7f), 0f);

            AddSecureDoor(new Vector3(3f, 0f, 1f));
            Sign("CONTROLLED MOVEMENT", new Vector3(3f, 3.35f, 1f), 0f);

            Block("Perimeter North", new Vector3(0f, 2f, 15f), new Vector3(40f, 4f, 0.25f), Metal);
            Block("Perimeter South", new Vector3(0f, 2f, -15f), new Vector3(40f, 4f, 0.25f), Metal);
            Block("Perimeter East", new Vector3(20f, 2f, 0f), new Vector3(0.25f, 4f, 30f), Metal);
            Block("Perimeter West", new Vector3(-20f, 2f, 0f), new Vector3(0.25f, 4f, 30f), Metal);
            for (int x = -18; x <= 18; x += 6)
                DecorativeBlock($"North Fence Post {x}", new Vector3(x, 2.25f, 14.72f), new Vector3(0.18f, 4.5f, 0.18f), Metal);
            DecorativeBlock("North Security Rail", new Vector3(0f, 4.15f, 14.72f), new Vector3(38f, 0.10f, 0.10f), Safety);
            for (int x = -15; x <= 15; x += 3)
                AddImportedModel("YardFence", new Vector3(x, 2.4f, 14.68f), Quaternion.identity);
            AddFacilityLighting();
        }

        private static void Room(string name, Vector3 center, Vector2 size, Color floor, string sign)
        {
            Block($"{name} Floor", center + Vector3.up * 0.03f, new Vector3(size.x, 0.08f, size.y), floor);
            Block($"{name} Back Wall", center + new Vector3(0f, 1.5f, size.y * 0.5f), new Vector3(size.x, 3f, 0.25f), Wall);
            Block($"{name} Left Wall", center + new Vector3(-size.x * 0.5f, 1.5f, 0f), new Vector3(0.25f, 3f, size.y), Wall);
            Block($"{name} Right Wall", center + new Vector3(size.x * 0.5f, 1.5f, 0f), new Vector3(0.25f, 3f, size.y), Wall);
            DecorativeBlock($"{name} Base Trim Back", center + new Vector3(0f, 0.18f, size.y * 0.5f - 0.02f), new Vector3(size.x - 0.2f, 0.36f, 0.08f), WallBase);
            DecorativeBlock($"{name} Base Trim Left", center + new Vector3(-size.x * 0.5f + 0.02f, 0.18f, 0f), new Vector3(0.08f, 0.36f, size.y - 0.2f), WallBase);
            DecorativeBlock($"{name} Base Trim Right", center + new Vector3(size.x * 0.5f - 0.02f, 0.18f, 0f), new Vector3(0.08f, 0.36f, size.y - 0.2f), WallBase);
            DecorativeBlock($"{name} Wall Band", center + new Vector3(0f, 2.15f, size.y * 0.5f - 0.14f), new Vector3(size.x - 0.35f, 0.32f, 0.05f), Blue);
            DecorativeBlock($"{name} Front Beam", center + new Vector3(0f, 2.92f, -size.y * 0.5f), new Vector3(size.x, 0.18f, 0.22f), Metal);
            DecorativeBlock($"{name} Back Coping", center + new Vector3(0f, 3.02f, size.y * 0.5f), new Vector3(size.x + 0.2f, 0.12f, 0.42f), WallBase);
            for (int side = -1; side <= 1; side += 2)
            {
                Vector3 fixture = center + new Vector3(side * size.x * 0.32f, 2.55f, size.y * 0.5f - 0.2f);
                if (AddImportedModel("WallLight", fixture - Vector3.up * 0.18f, Quaternion.identity) == null)
                {
                    DecorativeBlock($"{name} Bulkhead Housing {side}", fixture, new Vector3(0.6f, 0.28f, 0.18f), Metal);
                    DecorativeBlock($"{name} Bulkhead Lens {side}", fixture + new Vector3(0f, 0f, -0.1f), new Vector3(0.44f, 0.12f, 0.035f), Light);
                }
            }
            AddImportedModel("WallPanel", center + new Vector3(0f, 0.4f, size.y * 0.5f - 0.18f), Quaternion.identity);
            Sign(sign, center + new Vector3(0f, 2.7f, -size.y * 0.5f + 0.2f), 0f);
        }

        private static void AddYardMarkings()
        {
            // Flat paint stays inside the existing slab and leaves every route unchanged.
            Color paint = new Color(0.72f, 0.74f, 0.64f);
            for (int side = -1; side <= 1; side += 2)
            {
                DecorativeBlock($"Recreation Sideline {side}", new Vector3(side * 4.3f, 0.092f, -2.6f), new Vector3(0.06f, 0.008f, 3.8f), paint);
                DecorativeBlock($"Recreation End Line {side}", new Vector3(0f, 0.092f, -2.6f + side * 1.9f), new Vector3(8.6f, 0.008f, 0.06f), paint);
            }
            for (int lane = 0; lane < 3; lane++)
                DecorativeBlock($"Recreation Exercise Mark {lane}", new Vector3(-2.8f + lane * 2.8f, 0.093f, -3.8f), new Vector3(0.7f, 0.008f, 0.06f), paint);
        }

        private static GameObject AddImportedModel(string asset, Vector3 position, Quaternion rotation)
        {
            GameObject prefab = Resources.Load<GameObject>("Environment/NewResources01/" + asset);
            if (prefab == null)
            {
                Debug.LogWarning("Environment prefab unavailable; retaining gray-box surface: " + asset);
                return null;
            }
            return Object.Instantiate(prefab, position, rotation);
        }

        private static void AddImportedBench(string name, float x)
        {
            GameObject collision = Block(name, new Vector3(x, 0.45f, -5.5f), new Vector3(2.8f, 0.35f, 0.65f), Wood);
            GameObject bench = AddImportedModel("Bench", new Vector3(x, 0.08f, -5.5f), Quaternion.identity);
            if (bench != null)
            {
                // Uniform fit keeps the supplied bench inside the original 0.65 m depth.
                bench.transform.localScale = Vector3.one * 0.62f;
                collision.GetComponent<Renderer>().enabled = false;
            }
        }

        private static void AddImportedYardTiles()
        {
            // A shallow visual apron: original yard slab remains the collision surface.
            for (int i = 0; i < 4; i++)
            {
                GameObject tile = AddImportedModel(i % 2 == 0 ? "OutdoorTileA" : "OutdoorTileB",
                    new Vector3(-3f + i * 2f, 0f, -7f), Quaternion.identity);
                if (tile == null) continue;
                Renderer[] renderers = tile.GetComponentsInChildren<Renderer>();
                if (renderers.Length == 0) continue;
                Bounds bounds = renderers[0].bounds;
                foreach (Renderer renderer in renderers) bounds.Encapsulate(renderer.bounds);
                tile.transform.position += Vector3.up * (0.12f - bounds.max.y);
            }
        }

        private static void AddPropertyBinStack(Vector3 origin)
        {
            // Reference: 0.6 x 0.4 x 0.32 m stackable intake bins.
            for (int i = 0; i < 2; i++)
            {
                float y = 0.16f + i * 0.32f;
                Block($"Property Bin {origin.z:0.0}-{i + 1}", origin + new Vector3(0f, y, 0f), new Vector3(0.6f, 0.32f, 0.4f), PropertyBin);
                DecorativeBlock($"Property Bin Lid {origin.z:0.0}-{i + 1}", origin + new Vector3(0f, y + 0.17f, 0f), new Vector3(0.62f, 0.04f, 0.42f), Blue);
            }
        }

        private static void AddLaundryCart(Vector3 origin, bool loaded)
        {
            // Reference: ~1.1 x 0.7 x 0.9 m tubular cart with blue liner.
            DecorativeBlock($"Laundry Cart Frame {origin.x:0.0}", origin + new Vector3(0f, 0.45f, 0f), new Vector3(1.05f, 0.08f, 0.68f), Metal);
            DecorativeBlock($"Laundry Cart Handle {origin.x:0.0}", origin + new Vector3(0f, 0.85f, -0.42f), new Vector3(0.7f, 0.08f, 0.08f), Metal);
            Block($"Laundry Cart Liner {origin.x:0.0}", origin + new Vector3(0f, 0.5f, 0f), new Vector3(0.95f, 0.7f, 0.6f), PropertyBin);
            for (int i = 0; i < 4; i++)
            {
                float x = i % 2 == 0 ? -0.38f : 0.38f;
                float z = i < 2 ? -0.24f : 0.24f;
                DecorativeDisc($"Laundry Cart Caster {origin.x:0.0}-{i + 1}", origin + new Vector3(x, 0.08f, z));
            }

            if (loaded)
            {
                DecorativeBlock($"Laundry Load {origin.x:0.0}", origin + new Vector3(0f, 0.95f, 0f), new Vector3(0.7f, 0.22f, 0.45f), Linen);
            }
        }

        private static void AddDiningTable(Vector3 origin)
        {
            Block($"Dining Tabletop {origin.x:0.0}", origin + new Vector3(0f, 0.72f, 0f), new Vector3(1.8f, 0.08f, 1.0f), Metal);
            DecorativeBlock($"Dining Table Frame {origin.x:0.0}", origin + new Vector3(0f, 0.36f, 0f), new Vector3(0.18f, 0.72f, 0.18f), Metal);
            for (int i = 0; i < 4; i++)
            {
                float x = i % 2 == 0 ? -0.55f : 0.55f;
                float z = i < 2 ? -0.35f : 0.35f;
                DecorativeBlock($"Dining Stool {origin.x:0.0}-{i + 1}", origin + new Vector3(x, 0.35f, z), new Vector3(0.34f, 0.08f, 0.34f), Metal);
                DecorativeBlock($"Dining Stool Post {origin.x:0.0}-{i + 1}", origin + new Vector3(x, 0.18f, z), new Vector3(0.08f, 0.36f, 0.08f), Metal);
            }
        }

        private static void AddExerciseRail(Vector3 origin)
        {
            Block("Exercise Rail Bar", origin + new Vector3(0f, 1.05f, 0f), new Vector3(3f, 0.12f, 0.12f), Metal);
            DecorativeBlock("Exercise Rail Grip", origin + new Vector3(0f, 1.05f, 0f), new Vector3(1.4f, 0.14f, 0.14f), Safety);
            DecorativeBlock("Exercise Rail Left Post", origin + new Vector3(-1.4f, 0.55f, 0f), new Vector3(0.14f, 1.1f, 0.14f), Metal);
            DecorativeBlock("Exercise Rail Right Post", origin + new Vector3(1.4f, 0.55f, 0f), new Vector3(0.14f, 1.1f, 0.14f), Metal);
            DecorativeBlock("Exercise Rail Left Foot", origin + new Vector3(-1.4f, 0.05f, 0f), new Vector3(0.4f, 0.1f, 0.4f), WallBase);
            DecorativeBlock("Exercise Rail Right Foot", origin + new Vector3(1.4f, 0.05f, 0f), new Vector3(0.4f, 0.1f, 0.4f), WallBase);
        }

        private static void AddSecureDoor(Vector3 origin)
        {
            // Keeps the controlled opening clear for traversal while reading as a secure door frame + leaf.
            Block("Secure Door Frame Left", origin + new Vector3(-1.15f, 1.5f, 0f), new Vector3(0.28f, 3f, 0.35f), Metal);
            Block("Secure Door Frame Right", origin + new Vector3(1.15f, 1.5f, 0f), new Vector3(0.28f, 3f, 0.35f), Metal);
            Block("Secure Door Header", origin + new Vector3(0f, 2.9f, 0f), new Vector3(2.6f, 0.28f, 0.35f), Metal);
            DecorativeBlock("Secure Door Leaf", origin + new Vector3(0.55f, 1.35f, -0.08f), new Vector3(1.0f, 2.5f, 0.08f), DoorLeaf);
            DecorativeBlock("Secure Door Stripe", origin + new Vector3(0.55f, 1.35f, -0.13f), new Vector3(1.0f, 0.12f, 0.02f), Safety);
            DecorativeBlock("Secure Door Vision", origin + new Vector3(0.55f, 1.85f, -0.14f), new Vector3(0.28f, 0.34f, 0.02f), new Color(0.55f, 0.62f, 0.66f));
            DecorativeBlock("Secure Door Handle", origin + new Vector3(0.15f, 1.2f, -0.16f), new Vector3(0.18f, 0.05f, 0.08f), Metal);
        }

        private static void Sign(string text, Vector3 position, float yaw)
        {
            DecorativeBlock($"Sign Panel: {text}", position + new Vector3(0f, 0f, 0.05f), new Vector3(Mathf.Clamp(text.Length * 0.16f, 2.6f, 5.4f), 0.48f, 0.05f), Metal);
            DecorativeBlock($"Sign Accent: {text}", position + new Vector3(0f, -0.205f, 0f), new Vector3(Mathf.Clamp(text.Length * 0.16f, 2.6f, 5.4f), 0.055f, 0.06f), Orange);
            var sign = new GameObject($"Sign: {text}");
            sign.transform.SetPositionAndRotation(position, Quaternion.Euler(0f, yaw, 0f));
            var label = sign.AddComponent<TextMesh>();
            label.text = text;
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.fontSize = 48;
            label.characterSize = 0.08f;
            label.color = new Color(0.95f, 0.86f, 0.62f);
        }

        private static void AddRouteMarkings()
        {
            for (int x = -12; x <= 12; x += 4)
                DecorativeBlock($"Main Walk Dash {x}", new Vector3(x, 0.065f, 0f), new Vector3(1.8f, 0.012f, 0.07f), Safety);
            DecorativeBlock("West Route Stripe", new Vector3(-10.55f, 0.075f, 0f), new Vector3(0.08f, 0.012f, 19f), Orange);
            DecorativeBlock("East Route Stripe", new Vector3(10.55f, 0.075f, 0f), new Vector3(0.08f, 0.012f, 19f), Blue);
        }

        private static void AddFacilityLighting()
        {
            Vector3[] fixtures =
            {
                new Vector3(-14f, 2.78f, -9f), new Vector3(-13f, 2.78f, 7f),
                new Vector3(-3f, 2.78f, 9f), new Vector3(13f, 2.78f, 8f),
                new Vector3(13f, 2.78f, -7f)
            };
            for (int i = 0; i < fixtures.Length; i++)
            {
                DecorativeBlock($"Ceiling Light Housing {i + 1}", fixtures[i] + Vector3.up * 0.06f, new Vector3(2f, 0.08f, 0.48f), Metal);
                DecorativeBlock($"Ceiling Light {i + 1}", fixtures[i], new Vector3(1.8f, 0.07f, 0.32f), Light);
            }

            CreatePointLight("Central Security Light", new Vector3(2f, 3.8f, 2f), 9f, 0.8f);
            CreatePointLight("Yard Security Light", new Vector3(0f, 4.2f, -12f), 10f, 1.1f);
        }

        private static void CreatePointLight(string name, Vector3 position, float range, float intensity)
        {
            var lightObject = new GameObject(name);
            lightObject.transform.position = position;
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = range;
            light.intensity = intensity;
            light.color = Light;
            light.shadows = LightShadows.None;
        }

        private static void DecorativeBlock(string name, Vector3 position, Vector3 scale, Color color)
        {
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = name;
            block.transform.SetPositionAndRotation(position, Quaternion.identity);
            block.transform.localScale = scale;
            Object.Destroy(block.GetComponent<Collider>());
            ApplyMaterial(block.GetComponent<Renderer>(), color);
        }

        private static void DecorativeDisc(string name, Vector3 position)
        {
            GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            disc.name = name;
            disc.transform.SetPositionAndRotation(position, Quaternion.Euler(90f, 0f, 0f));
            disc.transform.localScale = new Vector3(0.62f, 0.08f, 0.62f);
            Object.Destroy(disc.GetComponent<Collider>());
            ApplyMaterial(disc.GetComponent<Renderer>(), new Color(0.18f, 0.25f, 0.26f));
        }

        private static GameObject Block(string name, Vector3 position, Vector3 scale, Color color)
        {
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = name;
            block.transform.SetPositionAndRotation(position, Quaternion.identity);
            block.transform.localScale = scale;
            ApplyMaterial(block.GetComponent<Renderer>(), color);
            return block;
        }

        private static void ApplyMaterial(Renderer target, Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit")
                ?? Shader.Find("Universal Render Pipeline/Unlit")
                ?? Shader.Find("Sprites/Default");
            if (shader == null)
            {
                Debug.LogError("No supported runtime shader is available for the facility material.");
                return;
            }

            string key = shader.name;
            if (!Materials.TryGetValue(key, out Material material) || material == null)
            {
                material = new Material(shader) { name = "Facility Shared Surface" };
                if (material.HasProperty("_EmissionColor")) material.EnableKeyword("_EMISSION");
                Materials[key] = material;
            }
            target.sharedMaterial = material;

            var properties = new MaterialPropertyBlock();
            if (material.HasProperty("_BaseColor")) properties.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) properties.SetColor("_Color", color);
            if (material.HasProperty("_Smoothness")) properties.SetFloat("_Smoothness", color == Metal ? 0.52f : 0.12f);
            if (material.HasProperty("_Metallic")) properties.SetFloat("_Metallic", color == Metal ? 0.7f : 0f);
            if (material.HasProperty("_EmissionColor")) properties.SetColor("_EmissionColor", color == Light ? color * 1.6f : Color.black);
            if (color == Wall || color == FloorVinyl || color == Asphalt)
            {
                Texture2D texture = GetSurfaceTexture(color == Asphalt);
                Vector3 scale = target.transform.localScale;
                // Cubes share UVs: use the two largest dimensions for floors and wall faces.
                float longest = Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z));
                float shortest = Mathf.Min(scale.x, Mathf.Min(scale.y, scale.z));
                float middle = scale.x + scale.y + scale.z - longest - shortest;
                Vector4 tiling = new Vector4(longest * 0.5f, middle * 0.5f, 0f, 0f);
                if (material.HasProperty("_BaseMap"))
                {
                    properties.SetTexture("_BaseMap", texture);
                    properties.SetVector("_BaseMap_ST", tiling);
                }
                if (material.HasProperty("_MainTex"))
                {
                    properties.SetTexture("_MainTex", texture);
                    properties.SetVector("_MainTex_ST", tiling);
                }
            }
            target.SetPropertyBlock(properties);
        }

        private static Texture2D GetSurfaceTexture(bool asphalt)
        {
            if (SurfaceTextures.TryGetValue(asphalt, out Texture2D existing) && existing != null)
                return existing;

            const int size = 64;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, true)
            {
                name = asphalt ? "Facility Asphalt Grain" : "Facility Concrete Grain",
                wrapMode = TextureWrapMode.Repeat,
                filterMode = FilterMode.Bilinear
            };
            var pixels = new Color32[size * size];
            // Local deterministic noise avoids altering gameplay's Unity random state.
            var noise = new System.Random(asphalt ? 173 : 71);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int grain = noise.Next(asphalt ? 192 : 223, 256);
                    if (!asphalt && (x == 0 || y == 0)) grain -= 18;
                    byte value = (byte)grain;
                    pixels[y * size + x] = new Color32(value, value, value, 255);
                }
            }
            texture.SetPixels32(pixels);
            texture.Apply(true, true);
            SurfaceTextures[asphalt] = texture;
            return texture;
        }

        private static void ResetMaterials()
        {
            foreach (Material material in Materials.Values)
            {
                if (material != null) Object.Destroy(material);
            }
            Materials.Clear();
            foreach (Texture2D texture in SurfaceTextures.Values)
            {
                if (texture != null) Object.Destroy(texture);
            }
            SurfaceTextures.Clear();
        }
    }
}
