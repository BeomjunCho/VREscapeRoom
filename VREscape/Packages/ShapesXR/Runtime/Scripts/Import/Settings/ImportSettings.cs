using System;

using UnityEngine;
using UnityEngine.Rendering;

namespace ShapesXR.Import.Settings
{
    [CreateAssetMenu(fileName = "ImportSettings", menuName = "ShapesXR/Import Settings")]
    public class ImportSettings : ScriptableObject, IImportSettings
    {
        private static ImportSettings? _instance;
        
        [SerializeField] private string _importedDataDirectory = "ImportedSpaces";
        [SerializeField] private MaterialImportMode _materialMode = MaterialImportMode.CombineSimilar;
        [SerializeField] private ShapesXr.MaterialMap _urpMaterialMap = null!;
        [SerializeField] private ShapesXr.MaterialMap _builtInMaterialMap = null!;

        // this property is from gltfast plugin shader
        [SerializeField] private string[] _gltfMainTextureProperties = { "baseColorTexture" };

        public static ImportSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = UnityEngine.Resources.Load<ImportSettings>("ImportSettings");

                    if (_instance.UserId.IsNullOrEmpty())
                        _instance.UserId = Guid.NewGuid().ToString();
                }

                return _instance;
            }
        }

        public string ImportedDataDirectory => Instance._importedDataDirectory;
        public MaterialImportMode MaterialMode => Instance._materialMode;
        public string[] GltfMainTextureProperties => Instance._gltfMainTextureProperties;
        // ReSharper disable once NotNullOrRequiredMemberIsNotInitialized
        public string UserId { get; private set; } = string.Empty;

        public ShapesXr.MaterialMap GetMaterialMap()
        {
            var renderPipeline = GraphicsSettings.defaultRenderPipeline;
            return renderPipeline == null ? _builtInMaterialMap : _urpMaterialMap;
        }
    }
}