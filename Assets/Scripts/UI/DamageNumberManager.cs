using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 伤害数字管理器 - 统一管理伤害数字的生成和显示
/// </summary>
public class DamageNumberManager : MonoBehaviour
{
    [Header("预制体设置")]
    [SerializeField] private GameObject damageNumberPrefab;   // 伤害数字预制体
    
    [Header("生成设置")]
    [SerializeField] private Vector3 spawnOffset = new Vector3(0, 1f, 0); // 生成偏移
    [SerializeField] private float randomRange = 0.5f;       // 随机位置范围
    
    // 单例模式
    public static DamageNumberManager Instance { get; private set; }
    
    private void Awake()
    {
        // 单例设置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // 如果没有设置预制体，创建一个默认的
        if (damageNumberPrefab == null)
        {
            CreateDefaultDamageNumberPrefab();
        }
    }
    
    /// <summary>
    /// 显示伤害数字
    /// </summary>
    public void ShowDamageNumber(Vector3 position, float damage, DamageType damageType = DamageType.EnemyDamage)
    {
        if (damageNumberPrefab == null) return;
        
        // 找到Canvas来显示UI伤害数字，优先使用现有的GameUICanvas
        Canvas canvas = GameObject.Find("GameUICanvas")?.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = FindFirstObjectByType<Canvas>();
        }
        
        if (canvas == null)
        {
            Debug.LogWarning("DamageNumberManager: 未找到Canvas，伤害数字将无法显示");
            return;
        }
        
        // 实例化伤害数字并设为Canvas的子对象
        GameObject damageObj = Instantiate(damageNumberPrefab, canvas.transform);
        
        // 将世界坐标转换为屏幕坐标，然后设置为UI位置
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(position + spawnOffset);
            
            // 添加随机偏移，避免重叠
            screenPos.x += Random.Range(-randomRange * 30, randomRange * 30);
            screenPos.y += Random.Range(-randomRange * 15, randomRange * 30);
            
            RectTransform rectTransform = damageObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.position = screenPos;
            }
        }
        
        DamageNumber damageNumber = damageObj.GetComponent<DamageNumber>();
        if (damageNumber != null)
        {
            damageNumber.Initialize(damage, damageType);
        }
        else
        {
            Debug.LogError("伤害数字预制体缺少 DamageNumber 组件！");
            Destroy(damageObj);
        }
    }
    
    /// <summary>
    /// 显示玩家受到的伤害
    /// </summary>
    public void ShowPlayerDamage(Vector3 position, float damage)
    {
        ShowDamageNumber(position, damage, DamageType.PlayerDamage);
    }
    
    /// <summary>
    /// 显示敌人受到的伤害
    /// </summary>
    public void ShowEnemyDamage(Vector3 position, float damage)
    {
        ShowDamageNumber(position, damage, DamageType.EnemyDamage);
    }
    
    /// <summary>
    /// 显示治疗数字
    /// </summary>
    public void ShowHealingNumber(Vector3 position, float healAmount)
    {
        ShowDamageNumber(position, healAmount, DamageType.Healing);
    }
    
    /// <summary>
    /// 显示自定义文本
    /// </summary>
    public void ShowCustomText(Vector3 position, string text, Color color)
    {
        if (damageNumberPrefab == null) return;
        
        Vector3 spawnPosition = position + spawnOffset;
        spawnPosition += new Vector3(
            Random.Range(-randomRange, randomRange),
            Random.Range(-randomRange * 0.5f, randomRange * 0.5f),
            0
        );
        
        GameObject damageObj = Instantiate(damageNumberPrefab, spawnPosition, Quaternion.identity);
        DamageNumber damageNumber = damageObj.GetComponent<DamageNumber>();
        
        if (damageNumber != null)
        {
            damageNumber.SetCustomText(text, color);
        }
        else
        {
            Destroy(damageObj);
        }
    }
    
    /// <summary>
    /// 创建默认的伤害数字预制体
    /// </summary>
    private void CreateDefaultDamageNumberPrefab()
    {
        // 创建一个功能完整的默认伤害数字预制体
        GameObject defaultPrefab = new GameObject("DamageNumber");
        
        // 添加RectTransform用于UI
        RectTransform rectTransform = defaultPrefab.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100, 50);
        
        // 创建子对象来包含文本
        GameObject textChild = new GameObject("Text");
        textChild.transform.SetParent(defaultPrefab.transform);
        
        // 添加TextMeshProUGUI组件到子对象
        TextMeshProUGUI textComponent = textChild.AddComponent<TextMeshProUGUI>();
        textComponent.text = "0";
        textComponent.fontSize = 24f;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.color = Color.white;
        
        // 设置文本的RectTransform
        RectTransform textRect = textChild.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        // 添加DamageNumber组件
        DamageNumber damageComponent = defaultPrefab.AddComponent<DamageNumber>();
        
        // 设置为DontDestroyOnLoad，避免在场景切换时丢失
        DontDestroyOnLoad(defaultPrefab);
        
        // 将其设为非激活状态，作为模板使用
        defaultPrefab.SetActive(false);
        damageNumberPrefab = defaultPrefab;
        
        Debug.Log("DamageNumberManager: 创建了默认UI伤害数字组件。");
    }
    
    /// <summary>
    /// 设置伤害数字预制体
    /// </summary>
    public void SetDamageNumberPrefab(GameObject prefab)
    {
        damageNumberPrefab = prefab;
    }
}
