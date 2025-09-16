using UnityEngine;
using System.Collections;

/// <summary>
/// 飞剑残影对象池 - 专门管理飞剑残影效果的池化
/// </summary>
public class SwordAfterimagePool : MonoBehaviour
{
    [Header("池配置")]
    [SerializeField] private GameObject afterimagePrefab;      // 残影预制体
    [SerializeField] private int initialPoolSize = 30;        // 初始池大小
    [SerializeField] private int maxPoolSize = 100;           // 最大池大小
    [SerializeField] private bool allowPoolGrowth = true;     // 是否允许池增长
    [SerializeField] private bool enableDebugLog = false;     // 是否启用调试日志
    
    [Header("残影设置")]
    [SerializeField] private float defaultLifetime = 0.5f;    // 默认残影生命时长
    [SerializeField] private float defaultFadeSpeed = 3f;     // 默认淡出速度
    [SerializeField] private float defaultInitialAlpha = 0.6f; // 默认初始透明度
    
    // 单例模式
    public static SwordAfterimagePool Instance { get; private set; }
    
    // 对象池实例
    private ObjectPool<SwordAfterimage> afterimagePool;
    private Transform poolParent;
    
    // 活跃残影跟踪
    private System.Collections.Generic.List<SwordAfterimage> activeAfterimages = new System.Collections.Generic.List<SwordAfterimage>();
    private System.Collections.Generic.Dictionary<SwordAfterimage, Coroutine> lifetimeCoroutines = new System.Collections.Generic.Dictionary<SwordAfterimage, Coroutine>();
    
    // 统计信息
    public int TotalCount => afterimagePool?.TotalCount ?? 0;
    public int ActiveCount => afterimagePool?.ActiveCount ?? 0;
    public int AvailableCount => afterimagePool?.AvailableCount ?? 0;
    
    private void Awake()
    {
        // 单例模式实现
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 初始化对象池
    /// </summary>
    private void InitializePool()
    {
        // 创建池容器
        GameObject poolContainer = new GameObject("SwordAfterimagePool");
        poolContainer.transform.SetParent(transform);
        poolParent = poolContainer.transform;
        
        // 如果没有设置预制体，创建默认的
        if (afterimagePrefab == null)
        {
            CreateDefaultAfterimageePrefab();
        }
        
        // 验证预制体有SwordAfterimage组件
        if (afterimagePrefab.GetComponent<SwordAfterimage>() == null)
        {
            Debug.LogError("[SwordAfterimagePool] 预制体缺少SwordAfterimage组件");
            return;
        }
        
        // 创建对象池
        afterimagePool = new ObjectPool<SwordAfterimage>(
            prefab: afterimagePrefab,
            poolParent: poolParent,
            initialSize: initialPoolSize,
            maxSize: maxPoolSize,
            allowGrowth: allowPoolGrowth,
            createFunction: null, // 使用默认创建函数
            onGet: OnGetAfterimage,
            onRelease: OnReleaseAfterimage,
            onDestroy: OnDestroyAfterimage
        );
        
        if (enableDebugLog)
            Debug.Log($"[SwordAfterimagePool] 初始化完成: {GetPoolStats()}");
    }
    
    /// <summary>
    /// 创建飞剑残影
    /// </summary>
    /// <param name="swordSprite">飞剑精灵</param>
    /// <param name="swordColor">飞剑颜色</param>
    /// <param name="position">位置</param>
    /// <param name="rotation">旋转</param>
    /// <param name="scale">缩放</param>
    /// <param name="initialAlpha">初始透明度</param>
    /// <param name="lifetime">生命时长（-1使用默认）</param>
    /// <param name="fadeSpeed">淡出速度（-1使用默认）</param>
    /// <returns>残影对象</returns>
    public SwordAfterimage CreateAfterimage(
        Sprite swordSprite, 
        Color swordColor, 
        Vector3 position, 
        Quaternion rotation, 
        Vector3 scale,
        float initialAlpha = -1f,
        float lifetime = -1f,
        float fadeSpeed = -1f)
    {
        if (afterimagePool == null)
        {
            Debug.LogError("[SwordAfterimagePool] 对象池未初始化");
            return null;
        }
        
        SwordAfterimage afterimage = afterimagePool.Get();
        if (afterimage == null)
        {
            Debug.LogWarning("[SwordAfterimagePool] 无法从池中获取残影对象");
            return null;
        }
        
        // 设置变换
        afterimage.transform.position = position;
        afterimage.transform.rotation = rotation;
        afterimage.transform.localScale = scale;
        
        // 设置参数
        float actualAlpha = initialAlpha > 0 ? initialAlpha : defaultInitialAlpha;
        float actualLifetime = lifetime > 0 ? lifetime : defaultLifetime;
        float actualFadeSpeed = fadeSpeed > 0 ? fadeSpeed : defaultFadeSpeed;
        
        // 初始化残影
        afterimage.Initialize(swordSprite, swordColor, actualAlpha);
        
        // 设置残影参数
        SetAfterimageParameters(afterimage, actualLifetime, actualFadeSpeed);
        
        // 添加到活跃列表
        activeAfterimages.Add(afterimage);
        
        // 启动生命周期协程
        Coroutine lifetimeCoroutine = StartCoroutine(AfterimageLifetimeCoroutine(afterimage, actualLifetime));
        lifetimeCoroutines[afterimage] = lifetimeCoroutine;
        
        if (enableDebugLog)
            Debug.Log($"[SwordAfterimagePool] 创建残影: 位置={position}, 生命时长={actualLifetime}");
        
        return afterimage;
    }
    
    /// <summary>
    /// 设置残影参数
    /// </summary>
    private void SetAfterimageParameters(SwordAfterimage afterimage, float lifetime, float fadeSpeed)
    {
        // 使用SwordAfterimage的公开方法设置参数
        afterimage.SetParameters(lifetime, fadeSpeed);
        
        if (enableDebugLog)
            Debug.Log($"[SwordAfterimagePool] 设置残影参数: 生命时长={lifetime}, 淡出速度={fadeSpeed}");
    }
    
    /// <summary>
    /// 残影生命周期协程
    /// </summary>
    private IEnumerator AfterimageLifetimeCoroutine(SwordAfterimage afterimage, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        
        // 时间到了，回收残影
        if (afterimage != null && activeAfterimages.Contains(afterimage))
        {
            RecycleAfterimage(afterimage);
        }
    }
    
    /// <summary>
    /// 回收残影到对象池
    /// </summary>
    /// <param name="afterimage">要回收的残影</param>
    public void RecycleAfterimage(SwordAfterimage afterimage)
    {
        if (afterimage == null)
        {
            Debug.LogWarning("[SwordAfterimagePool] 尝试回收null残影");
            return;
        }
        
        // 从活跃列表移除
        activeAfterimages.Remove(afterimage);
        
        // 停止生命周期协程
        if (lifetimeCoroutines.TryGetValue(afterimage, out Coroutine coroutine))
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            lifetimeCoroutines.Remove(afterimage);
        }
        
        // 回收到对象池
        if (afterimagePool != null)
        {
            afterimagePool.Release(afterimage);
            
            if (enableDebugLog)
                Debug.Log($"[SwordAfterimagePool] 回收残影: {afterimage.name}");
        }
    }
    
    /// <summary>
    /// 获取残影时的回调
    /// </summary>
    private void OnGetAfterimage(SwordAfterimage afterimage)
    {
        if (afterimage == null) return;
        
        // 激活对象
        afterimage.gameObject.SetActive(true);
        
        // 重置状态
        afterimage.enabled = true;
        
        // 停止所有协程
        afterimage.StopAllCoroutines();
        
        if (enableDebugLog)
            Debug.Log($"[SwordAfterimagePool] OnGet: {afterimage.name}");
    }
    
    /// <summary>
    /// 释放残影时的回调
    /// </summary>
    private void OnReleaseAfterimage(SwordAfterimage afterimage)
    {
        if (afterimage == null) return;
        
        // 停止所有协程
        afterimage.StopAllCoroutines();
        
        // 重置到池父级
        afterimage.transform.SetParent(poolParent);
        afterimage.transform.localPosition = Vector3.zero;
        afterimage.transform.localRotation = Quaternion.identity;
        afterimage.transform.localScale = Vector3.one;
        
        // 重置残影状态
        afterimage.ResetState();
        
        // 禁用对象
        afterimage.gameObject.SetActive(false);
        
        if (enableDebugLog)
            Debug.Log($"[SwordAfterimagePool] OnRelease: {afterimage.name}");
    }
    
    /// <summary>
    /// 销毁残影时的回调
    /// </summary>
    private void OnDestroyAfterimage(SwordAfterimage afterimage)
    {
        if (afterimage == null) return;
        
        if (enableDebugLog)
            Debug.Log($"[SwordAfterimagePool] OnDestroy: {afterimage.name}");
    }
    
    /// <summary>
    /// 创建默认残影预制体
    /// </summary>
    private void CreateDefaultAfterimageePrefab()
    {
        Debug.Log("[SwordAfterimagePool] 创建默认残影预制体...");
        
        // 创建游戏对象
        GameObject defaultPrefab = new GameObject("SwordAfterimage");
        
        // 添加SpriteRenderer
        SpriteRenderer sr = defaultPrefab.AddComponent<SpriteRenderer>();
        sr.color = Color.white;
        sr.sortingOrder = -1;
        
        // 添加SwordAfterimage组件
        defaultPrefab.AddComponent<SwordAfterimage>();
        
        // 将其设为非激活状态，作为模板使用
        defaultPrefab.SetActive(false);
        afterimagePrefab = defaultPrefab;
        
        Debug.Log("[SwordAfterimagePool] 默认残影预制体创建完成");
    }
    
    /// <summary>
    /// 回收所有活跃残影
    /// </summary>
    public void RecycleAllAfterimages()
    {
        var afterimagesToRecycle = new System.Collections.Generic.List<SwordAfterimage>(activeAfterimages);
        foreach (SwordAfterimage afterimage in afterimagesToRecycle)
        {
            RecycleAfterimage(afterimage);
        }
        
        Debug.Log($"[SwordAfterimagePool] 已回收所有活跃残影: {afterimagesToRecycle.Count} 个");
    }
    
    /// <summary>
    /// 清空对象池
    /// </summary>
    public void ClearPool()
    {
        // 先回收所有活跃残影
        RecycleAllAfterimages();
        
        // 清空对象池
        if (afterimagePool != null)
        {
            afterimagePool.Clear();
            Debug.Log("[SwordAfterimagePool] 对象池已清空");
        }
    }
    
    /// <summary>
    /// 预加载残影对象
    /// </summary>
    /// <param name="count">预加载数量</param>
    public void PreloadAfterimages(int count)
    {
        if (afterimagePool != null)
        {
            afterimagePool.Preload(count);
            Debug.Log($"[SwordAfterimagePool] 预加载了 {count} 个残影对象");
        }
    }
    
    /// <summary>
    /// 获取池统计信息
    /// </summary>
    public string GetPoolStats()
    {
        if (afterimagePool != null)
        {
            return afterimagePool.GetPoolStats();
        }
        return "SwordAfterimagePool: 未初始化";
    }
    
    /// <summary>
    /// 验证池完整性
    /// </summary>
    public bool ValidatePool()
    {
        if (afterimagePool != null)
        {
            return afterimagePool.ValidatePool();
        }
        return false;
    }
    
    /// <summary>
    /// 设置残影预制体
    /// </summary>
    public void SetAfterimagePreafb(GameObject prefab)
    {
        if (prefab != null && prefab.GetComponent<SwordAfterimage>() != null)
        {
            afterimagePrefab = prefab;
            Debug.Log("[SwordAfterimagePool] 残影预制体已更新");
        }
        else
        {
            Debug.LogError("[SwordAfterimagePool] 无效的预制体：缺少SwordAfterimage组件");
        }
    }
    
    /// <summary>
    /// 设置默认参数
    /// </summary>
    public void SetDefaultParameters(float lifetime, float fadeSpeed, float initialAlpha)
    {
        defaultLifetime = Mathf.Max(0.1f, lifetime);
        defaultFadeSpeed = Mathf.Max(0.1f, fadeSpeed);
        defaultInitialAlpha = Mathf.Clamp01(initialAlpha);
        
        Debug.Log($"[SwordAfterimagePool] 默认参数已更新: 生命时长={defaultLifetime}, 淡出速度={defaultFadeSpeed}, 初始透明度={defaultInitialAlpha}");
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        
        // 清空池
        ClearPool();
    }
    
    // 调试方法
    [ContextMenu("显示池状态")]
    private void ShowPoolStatus()
    {
        Debug.Log($"[SwordAfterimagePool] {GetPoolStats()}");
        Debug.Log($"[SwordAfterimagePool] 活跃残影: {activeAfterimages.Count}");
    }
    
    [ContextMenu("验证池完整性")]
    private void ValidatePoolIntegrity()
    {
        bool isValid = ValidatePool();
        Debug.Log($"[SwordAfterimagePool] 池完整性验证: {(isValid ? "通过" : "失败")}");
    }
    
    [ContextMenu("回收所有残影")]
    private void RecycleAll()
    {
        RecycleAllAfterimages();
    }
}
