using UnityEngine;

/// <summary>
/// 摄像机跟随控制器 - 让摄像机平滑跟随玩家
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("跟随设置")]
    [SerializeField] private Transform target;           // 跟随目标（玩家）
    [SerializeField] private float smoothSpeed = 2f; // 跟随平滑速度
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10); // 摄像机偏移
    
    [Header("边界限制")]
    [SerializeField] private bool useBounds = false;     // 是否使用边界限制
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -10f;
    [SerializeField] private float maxY = 10f;
    
    private Camera cam;
    
    private void Start()
    {
        cam = GetComponent<Camera>();
        
        // 如果没有设置目标，尝试找到玩家
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }
    
    private void LateUpdate()
    {
        if (target == null) return;
        
        FollowTarget();
    }
    
    /// <summary>
    /// 跟随目标
    /// </summary>
    private void FollowTarget()
    {
        // 计算期望位置
        Vector3 desiredPosition = target.position + offset;
        
        // 应用边界限制
        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }
        
        // 平滑移动到期望位置
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
    
    /// <summary>
    /// 设置新的跟随目标
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    /// <summary>
    /// 立即传送到目标位置（用于重生等场景）
    /// </summary>
    public void TeleportToTarget()
    {
        if (target == null) return;
        
        Vector3 targetPosition = target.position + offset;
        
        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }
        
        transform.position = targetPosition;
    }
}
