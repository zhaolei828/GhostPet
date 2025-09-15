using UnityEngine;
using System.Collections;

/// <summary>
/// 摄像机震动效果 - 提供屏幕震动反馈
/// </summary>
public class CameraShake : MonoBehaviour
{
    [Header("震动设置")]
    [SerializeField] private float defaultDuration = 0.2f;    // 默认震动时长
    [SerializeField] private float defaultIntensity = 0.3f;   // 默认震动强度
    [SerializeField] private bool enableScreenShake = true;   // 是否启用屏幕震动
    
    private Vector3 originalPosition;
    private bool isShaking = false;
    
    private void Start()
    {
        originalPosition = transform.position;
    }
    
    /// <summary>
    /// 触发震动效果
    /// </summary>
    /// <param name="duration">震动持续时间</param>
    /// <param name="intensity">震动强度</param>
    public void TriggerShake(float duration = -1f, float intensity = -1f)
    {
        if (!enableScreenShake) return;
        
        // 使用默认值
        if (duration <= 0) duration = defaultDuration;
        if (intensity <= 0) intensity = defaultIntensity;
        
        // 如果已经在震动，停止当前震动
        if (isShaking)
        {
            StopAllCoroutines();
            transform.position = originalPosition;
        }
        
        StartCoroutine(ShakeCoroutine(duration, intensity));
    }
    
    /// <summary>
    /// 轻微震动（玩家受伤）
    /// </summary>
    public void LightShake()
    {
        TriggerShake(0.15f, 0.1f);
    }
    
    /// <summary>
    /// 中等震动（敌人死亡）
    /// </summary>
    public void MediumShake()
    {
        TriggerShake(0.25f, 0.2f);
    }
    
    /// <summary>
    /// 强烈震动（爆炸等）
    /// </summary>
    public void HeavyShake()
    {
        TriggerShake(0.4f, 0.4f);
    }
    
    /// <summary>
    /// 震动协程
    /// </summary>
    private IEnumerator ShakeCoroutine(float duration, float intensity)
    {
        isShaking = true;
        float elapsed = 0f;
        
        // 记录起始位置（考虑摄像机可能在移动）
        Vector3 basePosition = transform.position;
        
        while (elapsed < duration)
        {
            // 获取当前应该的基础位置（如果摄像机在跟随移动）
            CameraFollow cameraFollow = GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                // 如果有摄像机跟随组件，使用其计算的目标位置作为基础位置
                basePosition.x = transform.position.x;
                basePosition.y = transform.position.y;
                basePosition.z = originalPosition.z; // 保持Z轴不变
            }
            
            // 计算震动强度衰减
            float currentIntensity = intensity * (1f - (elapsed / duration));
            
            // 生成随机偏移
            Vector3 randomOffset = new Vector3(
                Random.Range(-1f, 1f) * currentIntensity,
                Random.Range(-1f, 1f) * currentIntensity,
                0f
            );
            
            // 应用震动偏移
            transform.position = basePosition + randomOffset;
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 震动结束，恢复相对位置
        if (GetComponent<CameraFollow>() == null)
        {
            transform.position = originalPosition;
        }
        
        isShaking = false;
    }
    
    /// <summary>
    /// 停止震动
    /// </summary>
    public void StopShake()
    {
        if (isShaking)
        {
            StopAllCoroutines();
            
            if (GetComponent<CameraFollow>() == null)
            {
                transform.position = originalPosition;
            }
            
            isShaking = false;
        }
    }
    
    /// <summary>
    /// 设置是否启用屏幕震动
    /// </summary>
    public void SetShakeEnabled(bool enabled)
    {
        enableScreenShake = enabled;
        
        if (!enabled && isShaking)
        {
            StopShake();
        }
    }
    
    /// <summary>
    /// 更新原始位置（当摄像机位置发生变化时调用）
    /// </summary>
    public void UpdateOriginalPosition()
    {
        if (!isShaking)
        {
            originalPosition = transform.position;
        }
    }
}
