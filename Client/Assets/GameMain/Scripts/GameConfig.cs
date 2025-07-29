using System;
using HoweFramework;
using Unity.VisualScripting;
using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// 游戏配置。
    /// </summary>
    [Serializable]
    public class GameConfig
    {
        /// <summary>
        /// 游戏配置实例。
        /// </summary>
        public static GameConfig Instance { get; set; }

        [Header("资源管线配置")]
        /// <summary>
        /// CDN地址。
        /// </summary>
        [InspectorName("CDN地址")]
        public string CDNUrl = "http://127.0.0.1:8080";

        /// <summary>
        /// CDN备用地址。
        /// </summary>
        [InspectorName("CDN备用地址")]
        public string CDNFallbackUrl;

        /// <summary>
        /// CDN是否启用版本路径。若启用，则CDN地址会自动添加App版本号。
        /// </summary>
        [InspectorName("CDN是否启用版本路径")]
        public bool EnableCDNVersionPath = true;

        /// <summary>
        /// 是否启用模拟资源管线模式。
        /// </summary>
        [InspectorName("是否启用模拟资源管线模式(仅在编辑器下有效)")]
        public bool EnableEditorSimulateMode = true;

        [Header("FairyGUI配置")]
        /// <summary>
        /// 是否开启预加载包模式。
        /// </summary>
        [InspectorName("是否开启预加载包模式(WebGL模式下强制开启)")]
        public bool EnablePreloadPackageMode = false;


        [Header("配置表配置")]
        /// <summary>
        /// 配置表加载模式。
        /// </summary>
        [InspectorName("配置表加载模式")]
        public DataTableLoadMode DataTableLoadMode = DataTableLoadMode.LazyLoadAndPreloadSync;
    }
}