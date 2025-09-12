using System.Collections;
using UnityEngine;

/// <summary>
/// 飞剑残影管理器 - 负责生成和管理飞剑残影效果
/// </summary>
public class SwordAfterimageManager : MonoBehaviour
{
    [Header("残影生成设置")]
    [SerializeField] private GameObject afterimagePrefab; // 残影预制体
    [SerializeField] private float spawnInterval = 0.1f; // 生成间隔（秒）
    [SerializeField] private float minMoveDistance = 0.2f; // 最小移动距离才生成残影
    [SerializeField] private float initialAlpha = 0.6f; // 初始透明度
    [SerializeField] private bool enableAfterimage = false; // 是否启用残影
    
    [Header("残影外观")]
    [SerializeField] private Color afterimageColor = Color.cyan; // 残影颜色
    [SerializeField] private bool useOriginalColor = true; // 是否使用原始颜色
    
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
        
        // 在当前位置创建残影
        GameObject afterimage = CreateAfterimageObject();
        afterimage.transform.position = transform.position;
        afterimage.transform.rotation = transform.rotation;
        afterimage.transform.localScale = transform.localScale;
        
        // 初始化残影组件
        SwordAfterimage afterimageComponent = afterimage.GetComponent<SwordAfterimage>();
        if (afterimageComponent != null)
        {
            Color colorToUse = useOriginalColor ? swordSpriteRenderer.color : afterimageColor;
            afterimageComponent.Initialize(swordSpriteRenderer.sprite, colorToUse, initialAlpha);
        }
        
        // 更新记录
        lastAfterimagePosition = transform.position;
        lastSpawnTime = Time.time;
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
