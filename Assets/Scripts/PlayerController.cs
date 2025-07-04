using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    private float acceleration = 8f;   // 加速度
    private float deceleration = 8f;   // 减速度
    private Vector2 currentVelocity;    // 当前速度
    private Vector2 moveDirection;
    private float moveX;
    private float moveY;

    // 标志变量
    private bool isWalking;         // 是否在行走
    private bool mouseClick;        // 鼠标点击
    private bool isAttacking;       // 是否在攻击
    private bool isHurt;           // 是否受伤
    private bool isDead;           // 是否死亡

    // 控制连击
    private int comboCount = 0;  // 连击计数
    private float lastAttackEndTime = -1f; // 上一次攻击结束时间
    private const float comboInterval = 1f; // 连击时间间隔

    // 组件引用
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //控制玩家移动
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;
        mouseClick = Input.GetMouseButtonDown(0);  // 鼠标左键点击

        if (Input.GetKeyDown(KeyCode.J))
        {
            isHurt = true;
            anim.SetTrigger("isHurt");
        }

        if (isAttacking)
        {
            return;
        }

        // 控制角色朝向
        #region 
        if (moveX < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveX > 0)
        {
            spriteRenderer.flipX = false;
        }
        #endregion

        // 检测行走状态
        isWalking = moveDirection.magnitude > 0.1f;
        anim.SetBool("isWalking", isWalking);

        // 检测攻击状态
        if (mouseClick)
        {
            float timeSinceLastAttack = Time.time - lastAttackEndTime;
            if (comboCount == 0 || timeSinceLastAttack > comboInterval)
            {
                comboCount = 1;  // 重置连击计数
            }
            else
            {
                comboCount++;  // 增加连击计数
            }
                StartCoroutine(Attack());
        }
    }

    //平滑移动
    private void FixedUpdate()
    {
        if (isAttacking)
        {
            rb.velocity = Vector2.SmoothDamp(rb.velocity, Vector2.zero, ref currentVelocity, 1f / deceleration);
        }
        else if (isWalking)
        {
            // 计算目标速度
            Vector2 targetVelocity = moveDirection * moveSpeed;

            // 选择平滑时间：加速或减速
            float smoothTime = moveDirection.magnitude > 0.1f ? 1f / acceleration : 1f / deceleration;

            // 应用平滑移动
            rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, smoothTime);
        }
        else
        {
            // 如果不在行走状态，减速到0
            rb.velocity = Vector2.SmoothDamp(rb.velocity, Vector2.zero, ref currentVelocity, 1f / deceleration);
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        anim.SetBool("isWalking", false);   //强制idle
        if (comboCount < 3)
        {
            anim.SetTrigger("isNormalAttack");
        }else
        {
            anim.SetTrigger("isHeavyAttack");
        }
            yield return null;
    }

    public void OnAttackEnd()
    {
        isAttacking = false;
        lastAttackEndTime = Time.time; // 更新上一次攻击结束时间
        isWalking = moveDirection.magnitude > 0.1f;
        anim.SetBool("isWalking", isWalking);

        // 如果comboCount已到3（重击），下次必须重置
        if (comboCount >= 3)
        {
            comboCount = 0;
        }
    }

    // 当受到伤害时调用
    public void OnHurtEnd()
    {
        isHurt = false;
        isDead = true;
        anim.SetBool("isDead", isDead);
    }
}
