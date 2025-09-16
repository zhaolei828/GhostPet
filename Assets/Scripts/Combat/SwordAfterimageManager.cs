using System.Collections;
using UnityEngine;

/// <summary>
/// 飞剑残影管理器 - 负责生成和管理飞剑残影效果（使用对象池优化）
/// </summary>
public class SwordAfterimageManager : MonoBehaviour
{
    [Header("残影生成设置")]
    [SerializeField] private GameObject afterimagePrefab; // 残影预制体（用于传统模式）
    [SerializeField] private float spawnInterval = 0.1f; // 生成间隔（秒）
    [SerializeField] private float minMoveDistance = 0.2f; // 最小移动距离才生成残影
    [SerializeField] private float initialAlpha = 0.6f; // 初始透明度
    [SerializeField] private bool enableAfterimage = false; // 是否启用残影
    
    [Header("残影外观")]
    [SerializeField] private Color afterimageColor = Color.cyan; // 残影颜色
    [SerializeField] private bool useOriginalColor = true; // 是否使用原始颜色
    [SerializeField] private float afterimageLifetime = 0.5f; // 残影生命时长
    [SerializeField] private float afterimageFadeSpeed = 3f; // 残影淡出速度
    
    [Header("对象池设置")]
    [SerializeField] private bool useObjectPool = true; // 是否使用对象池
    [SerializeField] private bool enableDebugLog = false; // 是否启用调试日志
    
    private SpriteRenderer swordSpriteRenderer;
    private Vector3 lastAfterimagePosition;
    private float lastSpawnTime;
    private Coroutine afterimageCoroutine;
    
    private void Awake()
    {
        swordSpriteRenderer = GetComponent<SpriteRenderer>();
        lastAfterimagePosition = transform.position;
        
        // 如果没有设置残影预制体，创建一个简单的
        if (afterimagePrefab == null)
        {
            CreateAfterimageTemplate();
        }
    }
    
    /// <summary>
    /// 启用残影效果
    /// </summary>
    public void EnableAfterimage()
    {
        if (!enableAfterimage)
        {
            enableAfterimage = true;
            lastAfterimagePosition = transform.position;
            lastSpawnTime = Time.time;
            
            if (afterimageCoroutine == null)
            {
                afterimageCoroutine = StartCoroutine(AfterimageSpawnCoroutine());
            }
        }
    }
    
    /// <summary>
    /// 禁用残影效果
    /// </summary>
    public void DisableAfterimage()
    {
        enableAfterimage = false;
        
        if (afterimageCoroutine != null)
        {
            StopCoroutine(afterimageCoroutine);
            afterimageCoroutine = null;
        }
    }
    
    /// <summary>
    /// 残影生成协程
    /// </summary>
    private IEnumerator AfterimageSpawnCoroutine()
    {
        while (enableAfterimage)
        {
            // 检查是否应该生成残影
            if (ShouldSpawnAfterimage())
            {
                SpawnAfterimage();
            }
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    /// <summary>
    /// 检查是否应该生成残影
    /// </summary>
    private bool ShouldSpawnAfterimage()
    {
        // 检查移动距离
        float distanceMoved = Vector3.Distance(transform.position, lastAfterimagePosition);
        
        // 检查时间间隔
        bool timeIntervalReached = Time.time - lastSpawnTime >= spawnInterval;
        
        return distanceMoved >= minMoveDistance && timeIntervalReached;
    }
    
    /// <summary>
    /// 生成残影
    /// </summary>
    private void SpawnAfterimage()
    {
        if (swordSpriteRenderer == null || swordSpriteRenderer.sprite == null)
            return;
        
        Color colorToUse = useOriginalColor ? swordSpriteRenderer.color : afterimageColor;
        
        SwordAfterimage afterimageComponent = null;
        
        // 使用对象池或传统方式创建残影
        if (useObjectPool && SwordAfterimagePool.Instance != null)
        {
            afterimageComponent = CreateAfterimageFromPool(colorToUse);
        }
        else
        {
            afterimageComponent = CreateAfterimageTraditional(colorToUse);
        }
        
        if (afterimageComponent != null && enableDebugLog)
        {
            Debug.Log($"[SwordAfterimageManager] 生成残影: 位置={transform.position}, 使用对象池={useObjectPool}");
        }
        
        // 更新记录
        lastAfterimagePosition = transform.position;
        lastSpawnTime = Time.time;
    }
    
    /// <summary>
    /// 使用对象池创建残影
    /// </summary>
    private SwordAfterimage CreateAfterimageFromPool(Color color)
    {
        if (SwordAfterimagePool.Instance == null)
        {
            Debug.LogError("[SwordAfterimageManager] SwordAfterimagePool未初始化，切换到传统模式");
            return CreateAfterimageTraditional(color);
        }
        
        // 从对象池创建残影
        SwordAfterimage afterimage = SwordAfterimagePool.Instance.CreateAfterimage(
            swordSpriteRenderer.sprite,
            color,
            transform.position,
            transform.rotation,
            transform.localScale,
            initialAlpha,
            afterimageLifetime,
            afterimageFadeSpeed
        );
        
        if (enableDebugLog && afterimage != null)
            Debug.Log($"[SwordAfterimageManager] 从对象池创建残影: {afterimage.name}");
        
        return afterimage;
    }
    
    /// <summary>
    /// 传统方式创建残影
    /// </summary>
    private SwordAfterimage CreateAfterimageTraditional(Color color)
    {
        // 在当前位置创建残影
        GameObject afterimageObj = CreateAfterimageObject();
        afterimageObj.transform.position = transform.position;
        afterimageObj.transform.rotation = transform.rotation;
        afterimageObj.transform.localScale = transform.localScale;
        
        // 初始化残影组件
        SwordAfterimage afterimageComponent = afterimageObj.GetComponent<SwordAfterimage>();
        if (afterimageComponent != null)
        {
            afterimageComponent.Initialize(swordSpriteRenderer.sprite, color, initialAlpha);
            
            // 设置参数
            afterimageComponent.SetParameters(afterimageLifetime, afterimageFadeSpeed);
            
            if (enableDebugLog)
                Debug.Log($"[SwordAfterimageManager] 传统方式创建残影: {afterimageComponent.name}");
        }
        
        return afterimageComponent;
    }
    
    /// <summary>
    /// 创建残影对象
    /// </summary>
    private GameObject CreateAfterimageObject()
    {
        if (afterimagePrefab != null)
        {
            return Instantiate(afterimagePrefab);
        }
        else
        {
            // 创建简单的残影对象
            GameObject afterimage = new GameObject("SwordAfterimage");
            
            // 添加SpriteRenderer组件 - 修复警告
            SpriteRenderer spriteRenderer = afterimage.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = null; // 可以后续设置
            spriteRenderer.color = Color.white;
            
            afterimage.AddComponent<SwordAfterimage>();
            return afterimage;
        }
    }
    
    /// <summary>
    /// 创建残影模板
    /// </summary>
    private void CreateAfterimageTemplate()
    {
        // 这里可以在运行时创建一个简单的残影预制体
        // 或者让用户手动设置
        Debug.Log("请在Inspector中设置 afterimagePrefab，或者系统将自动创建简单残影");
    }
    
    /// <summary>
    /// 立即生成一个残影（用于特殊时刻）
    /// </summary>
    public void SpawnImmediateAfterimage()
    {
        if (swordSpriteRenderer != null && swordSpriteRenderer.sprite != null)
        {
            SpawnAfterimage();
        }
    }
    
    /// <summary>
    /// 设置残影参数
    /// </summary>
    public void SetAfterimageSettings(float interval, float alpha, Color color)
    {
        spawnInterval = interval;
        initialAlpha = alpha;
        afterimageColor = color;
    }
    
    private void OnDestroy()
    {
        DisableAfterimage();
    }
}
