using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ��������
    #region
    public float moveSpeed = 10f;
    private float acceleration = 8f;   // ���ٶ�
    private float deceleration = 8f;   // ���ٶ�
    private Vector2 currentVelocity;    // ��ǰ�ٶ�
    private Vector2 moveDirection;
    private float moveX;
    private float moveY;

    // ��־����
    private bool isWalking;        // �Ƿ�������
    private bool mouseClick;       // �����
    private bool isAttacking;      // �Ƿ��ڹ���
    private bool isHurt;           // �Ƿ�����
    private bool isDead;           // �Ƿ�����
    private bool isDodging;        // �Ƿ�������

    // ��������
    private float dodgeDuration = 0.2f;         // ���ܳ���ʱ��
    private float dodgeCooldown = 0.4f;           // ������ȴʱ��
    private float lastDodgeTime = -1f;          // ��һ������ʱ��
    private Vector2 dodgeDirection;             // ���ܷ���
    private float dodgeSpeedMultiplier = 3f;    // �����ٶȱ���
    private float dodgeTimer = 0f;              // ���ܼ�ʱ��

    // ��������
    private int comboCount = 0;                 // ��������
    private float lastAttackEndTime = -1f;      // ��һ�ι�������ʱ��
    private const float comboInterval = 1f;     // ����ʱ����

    // �������
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    #endregion

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
        //��������ƶ�
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;
        mouseClick = Input.GetMouseButtonDown(0);  // ���������

        // ��������
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDodging && Time.time - lastDodgeTime > dodgeCooldown && !isAttacking && !isHurt && !isDead)
        {
            StartCoroutine(Dodge());
        }

        // ����
        if (Input.GetKeyDown(KeyCode.J))
        {
            isHurt = true;
            anim.SetTrigger("isHurt");
        }

        // ����״̬�²���Ӧ�ƶ�
        if (isAttacking)
        {
            return;
        }

        // ���ƽ�ɫ����
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

        // �������״̬
        isWalking = moveDirection.magnitude > 0.1f;
        anim.SetBool("isWalking", isWalking);

        // ��⹥��״̬
        if (mouseClick)
        {
            float timeSinceLastAttack = Time.time - lastAttackEndTime;
            if (comboCount == 0 || timeSinceLastAttack > comboInterval)
            {
                comboCount = 1;  // ������������
            }
            else
            {
                comboCount++;  // ������������
            }
                StartCoroutine(Attack());
        }
    }

    //ƽ���ƶ�
    private void FixedUpdate()
    {
        if (isDodging)
        {
            return;
        }

        if (isAttacking)
        {
            rb.velocity = Vector2.SmoothDamp(rb.velocity, Vector2.zero, ref currentVelocity, 1f / deceleration);
        }
        else if (isWalking)
        {
            // ����Ŀ���ٶ�
            Vector2 targetVelocity = moveDirection * moveSpeed;

            // ѡ��ƽ��ʱ�䣺���ٻ����
            float smoothTime = moveDirection.magnitude > 0.1f ? 1f / acceleration : 1f / deceleration;

            // Ӧ��ƽ���ƶ�
            rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, smoothTime);
        }
        else
        {
            // �����������״̬�����ٵ�0
            rb.velocity = Vector2.SmoothDamp(rb.velocity, Vector2.zero, ref currentVelocity, 1f / deceleration);
        }
    }

    // ����Э��
    private IEnumerator Dodge()
    {
        isDodging = true;
        lastDodgeTime = Time.time;
        dodgeDirection = moveDirection;

        spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f);     // ����ʱ�黯��ɫ

        while (dodgeTimer < dodgeDuration)
        {
            rb.velocity = dodgeDirection * moveSpeed * dodgeSpeedMultiplier;
            dodgeTimer += Time.deltaTime;       // ��¼����ʱ��
            yield return null;                  // �ȴ���һ֡
        }
        dodgeTimer = 0f;        // �������ܼ�ʱ��
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);       // �ָ�͸����
        isDodging = false;
    }

    // ����Э��
    private IEnumerator Attack()
    {
        isAttacking = true;
        anim.SetBool("isWalking", false);   //ǿ��idle
        if (comboCount < 3)
        {
            anim.SetTrigger("isNormalAttack");
        }else
        {
            anim.SetTrigger("isHeavyAttack");
        }
            yield return null;
    }

    // �������¼�������������ʱ����
    public void OnAttackEnd()
    {
        isAttacking = false;
        lastAttackEndTime = Time.time; // ������һ�ι�������ʱ��
        isWalking = moveDirection.magnitude > 0.1f;
        anim.SetBool("isWalking", isWalking);

        // ���comboCount�ѵ�3���ػ������´α�������
        if (comboCount >= 3)
        {
            comboCount = 0;
        }
    }

    // �������¼������ܵ��˺�ʱ����
    public void OnHurtEnd()
    {
        isHurt = false;
        isDead = true;
        anim.SetBool("isDead", isDead);
    }
}
