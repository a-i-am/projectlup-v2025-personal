using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace LUP
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        [SerializeField, ReadOnly]
        private List<AssetBundle> assetbundles = new List<AssetBundle>();




        [SerializeField, ReadOnly]
        private AssetBundle AB_Manifest;
        [SerializeField, ReadOnly]
        private AssetBundle AB_Video;
        [SerializeField, ReadOnly]
        private AssetBundle AB_Audio;
        [SerializeField, ReadOnly]
        private AssetBundle AB_Image;
        [SerializeField, ReadOnly]
        private AssetBundle AB_VFX;
        [SerializeField, ReadOnly]
        private AssetBundle AB_GUI;
        [SerializeField, ReadOnly]
        private AssetBundle AB_Model;
        [SerializeField, ReadOnly]
        private AssetBundle AB_Shader;
        [SerializeField, ReadOnly]
        private AssetBundle AB_Data;

        private static Dictionary<string, Object> _cache = new();
        private static T LoadResource<T>(string path) where T : Object
        {
            if (_cache.ContainsKey(path)) return _cache[path] as T;
            var obj = Resources.Load<T>(path);
            if (obj != null) _cache[path] = obj;
            return obj;
        }

        private void Awake()
        {
            base.Awake();
            if (Instance.assetbundles.Count==0)
            {
                LoadAssetBundles();
            }

        }

        public T LoadVideoClip<T>(string name) where T : Object
        {
            if (AB_Video == null)
            {
                LoadAssetBundles();
            }
            T videoClip = AB_Video.LoadAsset<T>(name);

            return videoClip;
        }

        public T LoadAudioBGM<T>(string name) where T : Object
        {
            if (AB_Audio == null)
            {
                LoadAssetBundles();
            }

            T audioClip = AB_Audio.LoadAsset<T>(name);

            return audioClip;
        }

        public T LoadAudioSFX<T>(string name) where T : Object
        {
            if (AB_Audio == null)
            {
                LoadAssetBundles();
            }
            T audio = AB_Audio.LoadAsset<T>(name);

            return audio;
        }

        public T LoadImage<T>(string name) where T : Object
        {
            if (AB_Image == null)
            {
                LoadAssetBundles();
            }
            T image = AB_Image.LoadAsset<T>(name);

            return image;
        }
        public T LoadVFX<T>(string name) where T : Object
        {
            if (AB_VFX == null)
            {
                LoadAssetBundles();
            }
            T vfx = AB_VFX.LoadAsset<T>(name);

            return vfx;
        }
        public T LoadGUI<T>(string name) where T : Object
        {
            if (AB_GUI == null)
            {
                LoadAssetBundles();
            }
            T gui = AB_GUI.LoadAsset<T>(name);

            return gui;
        }
        public T LoadModel<T>(string name) where T : Object
        {
            if (AB_Model == null)
            {
                LoadAssetBundles();
            }
            T model = AB_Model.LoadAsset<T>(name);

            return model;
        }
        public T LoadShader<T>(string name) where T : Object
        {
            if (AB_Shader == null)
            {
                LoadAssetBundles();
            }
            T shader = AB_Shader.LoadAsset<T>(name);

            return shader;
        }

        public List<BaseStaticDataLoader> LoadStaticData(Define.StageKind type, int stagetype)
        {
            List<BaseStaticDataLoader> dataList = new List<BaseStaticDataLoader>();
            string folderPath = "";

            switch (type)
            {
                case Define.StageKind.ST:
                    folderPath = "Data/Games/ST";
                    break;
                case Define.StageKind.DSG:
                    folderPath = "Data/Games/DSG";
                    break;
                case Define.StageKind.ES:
                    folderPath = "Data/Games/ES";
                    break;
                case Define.StageKind.RL:
                    folderPath = "Data/Games/RL";
                    break;
                case Define.StageKind.PCR:
                    folderPath = "Data/Games/PCR";
                    break;
                case Define.StageKind.Tutorial:
                    folderPath = "Data/Games/FW";
                    break;
            }

            if (!string.IsNullOrEmpty(folderPath))
            {
                BaseStaticDataLoader[] loadedData = Resources.LoadAll<BaseStaticDataLoader>(folderPath);
                dataList.AddRange(loadedData);
            }

            return dataList;
        }

        public void LoadAssetBundles()
        {
            assetbundles.Clear();
            {
                AB_Manifest = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, Path.Combine("LUP/assetbundles", "AssetBundles")));
                assetbundles.Add(AB_Manifest);
            }
            {
                AB_Video = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, Path.Combine("LUP/assetbundles", "videos")));
                assetbundles.Add(AB_Video);
            }
            {
                AB_Audio = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, Path.Combine("LUP/assetbundles", "audios")));
                assetbundles.Add(AB_Audio);
            }
            {
                AB_Image = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, Path.Combine("LUP/assetbundles", "image")));
                assetbundles.Add(AB_Image);
            }
            {
                AB_VFX = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, Path.Combine("LUP/assetbundles", "vfx")));
                assetbundles.Add(AB_VFX);
            }
            {
                AB_GUI = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, Path.Combine("LUP/assetbundles", "gui")));
                assetbundles.Add(AB_GUI);
            }
            {
                AB_Model = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, Path.Combine("LUP/assetbundles", "models")));
                assetbundles.Add(AB_Model);
            }
            {
                AB_Shader = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, Path.Combine("LUP/assetbundles", "shaders")));
                assetbundles.Add(AB_Shader);
            }
            {
                AB_Data = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath,Path.Combine("LUP/assetbundles", "data")));
                assetbundles.Add(AB_Data);
            }
        }

        public void UnLoadAssetBundles()
        {
            assetbundles.Clear();
            {
                if (AB_Video != null)
                {
                    AB_Video.Unload(true);
                    AB_Video = null;
                }

            }
            {
                if (AB_Audio != null)
                {
                    AB_Audio.Unload(true);
                    AB_Audio = null;
                }
            }
            {
                if (AB_Image != null)
                {
                    AB_Image.Unload(true);
                    AB_Image = null;
                }
            }
            {
                if (AB_VFX != null)
                {
                    AB_VFX.Unload(true);
                    AB_VFX = null;
                }
            }
            {
                if (AB_GUI != null)
                {
                    AB_GUI.Unload(true);
                    AB_GUI = null;
                }
            }
            {
                if (AB_Model != null)
                {
                    AB_Model.Unload(true);
                    AB_Model = null;
                }
            }
            {
                if (AB_Shader != null)
                {
                    AB_Shader.Unload(true);
                    AB_Shader = null;
                }
            }
            {
                if (AB_Data != null)
                {
                    AB_Data.Unload(true);
                    AB_Data = null;
                }
            }
            {
                if (AB_Manifest != null)
                {
                    AB_Manifest.Unload(true);
                    AB_Manifest = null;
                }
            }
        }

        public int GetAssetBundleSize()
        {
            return assetbundles.Count;
        }

        public AssetBundle GetAssetBundle(Define.AssetBundleKind assetBundleKind)
        {
            switch (assetBundleKind)
            {
                case Define.AssetBundleKind.Video:
                    return AB_Video;
                case Define.AssetBundleKind.Audio:
                    return AB_Audio;
                case Define.AssetBundleKind.Image:
                    return AB_Image;
                case Define.AssetBundleKind.VFX:
                    return AB_VFX;
                case Define.AssetBundleKind.GUI:
                    return AB_GUI;
                case Define.AssetBundleKind.Model:
                    return AB_Model;
                case Define.AssetBundleKind.Shader:
                    return AB_Shader;
                case Define.AssetBundleKind.Data:
                    return AB_Data;
                case Define.AssetBundleKind.Manifest:
                    return AB_Manifest;
            }
            return null;
        }
    }
}

