using UnityEngine;
using System.Collections;

/// <summary>
/// 受击特效系统 - 处理角色被攻击时的视觉反馈
/// </summary>
public class HitEffect : MonoBehaviour
{
    [Header("闪烁设置")]
    [SerializeField] private float flashDuration = 0.2f;          // 闪烁持续时间
    [SerializeField] private int flashCount = 3;                  // 闪烁次数
    [SerializeField] private Color flashColor = Color.red;        // 闪烁颜色
    
    [Header("震动设置")]
    [SerializeField] private float shakeDuration = 0.15f;         // 震动持续时间
    [SerializeField] private float shakeIntensity = 0.1f;         // 震动强度
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector3 originalPosition;
    private bool isFlashing = false;
    private bool isShaking = false;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        originalPosition = transform.position;
    }
    
    private void Start()
    {
        // 订阅血量系统的受伤事件
        HealthSystem healthSystem = GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.OnDamageTaken += PlayHitEffect;
        }
    }
    
    /// <summary>
    /// 播放受击特效
    /// </summary>
    public void PlayHitEffect(float damageAmount = 0f)
    {
        // 播放闪烁效果
        if (!isFlashing)
        {
            StartCoroutine(FlashEffect());
        }
        
        // 播放震动效果
        if (!isShaking)
        {
            StartCoroutine(ShakeEffect());
        }
        
        // 触发屏幕震动
        CameraShake cameraShake = Camera.main?.GetComponent<CameraShake>();
        if (cameraShake != null)
        {
            cameraShake.TriggerShake(shakeDuration, shakeIntensity);
        }
    }
    
    /// <summary>
    /// 闪烁效果
    /// </summary>
    private IEnumerator FlashEffect()
    {
        if (spriteRenderer == null) yield break;
        
        isFlashing = true;
        float flashInterval = flashDuration / (flashCount * 2); // 每次闪烁的间隔
        
        for (int i = 0; i < flashCount; i++)
        {
            // 变为闪烁颜色
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashInterval);
            
            // 恢复原色
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashInterval);
        }
        
        // 确保恢复原色
        spriteRenderer.color = originalColor;
        isFlashing = false;
    }
    
    /// <summary>
    /// 震动效果
    /// </summary>
    private IEnumerator ShakeEffect()
    {
        isShaking = true;
        Vector3 startPosition = transform.position;
        float elapsed = 0f;
        
        while (elapsed < shakeDuration)
        {
            // 生成随机偏移
            Vector3 randomOffset = Random.insideUnitCircle * shakeIntensity;
            transform.position = startPosition + randomOffset;
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 恢复原位置
        transform.position = startPosition;
        isShaking = false;
    }
    
    /// <summary>
    /// 立即停止所有效果
    /// </summary>
    public void StopAllEffects()
    {
        StopAllCoroutines();
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        
        transform.position = originalPosition;
        isFlashing = false;
        isShaking = false;
    }
    
    private void OnDestroy()
    {
        // 取消事件订阅
        HealthSystem healthSystem = GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.OnDamageTaken -= PlayHitEffect;
        }
    }
    
    /// <summary>
    /// 设置闪烁颜色
    /// </summary>
    public void SetFlashColor(Color color)
    {
        flashColor = color;
    }
    
    /// <summary>
    /// 设置震动强度
    /// </summary>
    public void SetShakeIntensity(float intensity)
    {
        shakeIntensity = intensity;
    }
}
