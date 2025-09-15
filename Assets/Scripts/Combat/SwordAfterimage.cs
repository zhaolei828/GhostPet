using UnityEngine;

/// <summary>
/// 飞剑残影效果 - 单个残影的生命周期管理
/// </summary>
public class SwordAfterimage : MonoBehaviour
{
    [Header("残影设置")]
    [SerializeField] private float fadeSpeed = 3f; // 淡出速度
    [SerializeField] private float lifetime = 0.5f; // 残影持续时间
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float currentAlpha;
    private float timer;
    private MaterialPropertyBlock propBlock;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propBlock = new MaterialPropertyBlock();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            currentAlpha = originalColor.a;
        }
    }
    
    private void Update()
    {
        FadeOut();
        
        // 计时销毁
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            DestroyAfterimage();
        }
    }
    
    /// <summary>
    /// 初始化残影
    /// </summary>
    public void Initialize(Sprite swordSprite, Color swordColor, float initialAlpha = 0.6f)
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        // 设置残影外观
        spriteRenderer.sprite = swordSprite;
        originalColor = swordColor;
        currentAlpha = initialAlpha;
        
        // 设置初始透明度
        Color afterimageColor = originalColor;
        afterimageColor.a = currentAlpha;
        spriteRenderer.color = afterimageColor;
        
        // 设置渲染层级（稍微靠后）
        spriteRenderer.sortingOrder = -1;
        
        timer = 0f;
    }
    
    /// <summary>
    /// 淡出效果
    /// </summary>
    private void FadeOut()
    {
        if (spriteRenderer == null) return;
        
        // 逐渐降低透明度
        currentAlpha -= fadeSpeed * Time.deltaTime;
        currentAlpha = Mathf.Max(0f, currentAlpha);
        
        // 使用MaterialPropertyBlock更新颜色，避免创建材质实例
        spriteRenderer.GetPropertyBlock(propBlock);
        Color newColor = originalColor;
        newColor.a = currentAlpha;
        propBlock.SetColor("_Color", newColor);
        spriteRenderer.SetPropertyBlock(propBlock);
        
        // 如果完全透明则销毁
        if (currentAlpha <= 0f)
        {
            DestroyAfterimage();
        }
    }
    
    /// <summary>
    /// 销毁残影
    /// </summary>
    private void DestroyAfterimage()
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
